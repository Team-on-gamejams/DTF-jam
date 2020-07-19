using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Enemy : MonoBehaviour
{
    protected enum States { Idle, Chase, Attack, Escape, Dead }

    protected States _state = States.Idle;

    [Header("Main Properties")]
    [SerializeField] protected float _chaseSpeed = 8f;
    [SerializeField] protected float _searchRadius = 20f;
    [SerializeField] private LayerMask _playerLayer;

    [Header("Attack Properties")]
    private float _curAttackCD;

    [Header("Escape Properties")]
    [SerializeField] private float _escapeDurationMin;
    [SerializeField] private float _escapeDurationMax;
    [SerializeField] private float _escapeDistanceMin;
    [SerializeField] private float _escapeDistanceMax;
    private float _curEscapeDuration;

    private UnityEvent _attackEvent = new UnityEvent();
    private UnityAction _attackAction;

    // References
    [Header("Refs")]
    [Space]
    [SerializeField] Rigidbody _rigidBody;
    [SerializeField] Weapon _weapon;
    [SerializeField] Transform _myTransform;
    [SerializeField] NavMeshAgent _navAgent;
    [SerializeField] EnemyAnimation _animation;
    Transform _playerTransform;

    private void Awake()
    {
        _attackAction = new UnityAction(StartAttack);
        _attackEvent.AddListener(_attackAction);
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.isPlaying == false)
            return;

        PlayerSearch();

        switch (_state)
        {
            case States.Idle:
                Idle();
                break;
            case States.Chase:
                Chase();
                break;
            case States.Attack:
                Attack();
                break;
            case States.Escape:
                Escape();
                break;
        }
    }

    protected void SwitchState(States newState)
    {
        _curEscapeDuration = 0f;
        _state = newState;
    }

    private void Idle()
    {

    }

    private void Chase()
    {
        _myTransform.forward = GetForward();
        //_rigidBody.velocity = new Vector3(chaseDirection.x * _chaseSpeed, _rigidBody.velocity.y, chaseDirection.z * _chaseSpeed);
        _navAgent.speed = _chaseSpeed;
        _navAgent.SetDestination(_playerTransform.position);

        if (Vector3.Distance(_myTransform.position, _playerTransform.position) <= _weapon.attackRange)
        {
            _navAgent.ResetPath();
            _navAgent.velocity = Vector3.zero;
            SwitchState(States.Attack);
            _attackEvent.Invoke();
        }
    }

    protected virtual void Attack()
    {
        if (_curAttackCD <= Time.time)
        {
            if (Vector3.Distance(_myTransform.position, _playerTransform.position) > _weapon.attackRange)// && _weapon.curAttackNumber >= _weapon.attacksNumber)
            {
                SwitchState(States.Chase);
            }
            else
            {
                if(_weapon.attackType == AttackType.Range && Vector3.Distance(_playerTransform.position, _myTransform.position) < 5f)
                {
                    SwitchState(States.Escape);
                    return;
                }

                _myTransform.forward = GetForward();
                _attackEvent.Invoke();
            }
        }
    }

    protected void Escape()
    {
        if (_curEscapeDuration <= 0f)
        {
            _myTransform.forward = -GetForward();
            _navAgent.SetDestination(_myTransform.forward * Random.Range(_escapeDistanceMin, _escapeDistanceMax));
            _curEscapeDuration = Random.Range(_escapeDurationMin, _escapeDurationMax) + Time.time;
        }
        else if(_curEscapeDuration <= Time.time)
        {
            _navAgent.ResetPath();
            SwitchState(States.Chase);
        }
    }

    private void PlayerSearch()
    {
        if (_playerTransform != null)
            return;

        Collider[] hitColliders = Physics.OverlapSphere(_myTransform.position, _searchRadius, _playerLayer);

        foreach (Collider col in hitColliders)
        {
            _playerTransform = col.transform;
            _animation.playerTransform = _playerTransform;
            SwitchState(States.Chase);
            break;
        }
    }

    private Vector3 GetForward()
    {
        Vector3 chaseDirection = _playerTransform.position - _myTransform.position;
        chaseDirection.y = 0f;
        chaseDirection = Vector3.Normalize(chaseDirection);
        return chaseDirection;
    }

    private void StartAttack()
    {
        if (_weapon.attackType == AttackType.Melee)
        {
            Invoke("SetAnimAttack", _weapon.prepareTime);
        }
        else
        {
            SetAnimAttack();
            _weapon.StartCoroutine(_weapon.Shoot(_playerTransform.position));
        }

        _curAttackCD = _weapon.attackCD + Time.time;
    }

    private void SetAnimAttack()
    {
        _animation.anim.SetTrigger("IsAttack");
    }
}
