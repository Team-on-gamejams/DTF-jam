﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Enemy : MonoBehaviour
{
    protected enum States { Idle, Chase, Attack, Dead }

    protected States _state = States.Idle;

    [Header("Main Properties")]
    [SerializeField] protected float _chaseSpeed = 8f;
    [SerializeField] protected float _searchRadius = 20f;
    [SerializeField] private LayerMask _playerLayer;

    [Header("Attack Properties")]
    private float _curAttackCD;

    private UnityEvent _attackEvent = new UnityEvent();
    private UnityAction _attackAction;

    // References
    [SerializeField] private DealDamageOnTriggerEnter _swordAttackBox;
    protected Rigidbody _rigidBody;
    protected Weapon _weapon;
    protected Transform _myTransform;
    protected Transform _playerTransform;
    protected NavMeshAgent _navAgent;

    private void Awake()
    {
        foreach (Transform t in transform)
        {
            if (t.name == "EnemyBody")
                _myTransform = t;
        }
        _rigidBody = GetComponentInChildren<Rigidbody>();
        _weapon = GetComponentInChildren<Weapon>();
        _navAgent = GetComponentInChildren<NavMeshAgent>();

        _attackAction = new UnityAction(StartAttack);
        _attackEvent.AddListener(_attackAction);
    }

    private void FixedUpdate()
    {
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
        }
    }

    protected void SwitchState(States newState)
    {
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
            _rigidBody.velocity = Vector3.zero;
            SwitchState(States.Attack);
            _attackEvent.Invoke();
        }
    }

    protected virtual void Attack()
    {
        if (_curAttackCD <= Time.time)
        {
            if(_weapon.attackType == AttackType.Melee)
                DisableAttackCollider();

            if (Vector3.Distance(_myTransform.position, _playerTransform.position) > _weapon.attackRange)// && _weapon.curAttackNumber >= _weapon.attacksNumber)
            {
                SwitchState(States.Chase);
            }
            else
            {
                _myTransform.forward = GetForward();
                _attackEvent.Invoke();
            }
        }
    }

    private void PlayerSearch()
    {
        if (_playerTransform != null)
            return;

        Collider[] hitColliders = Physics.OverlapSphere(_myTransform.position, _searchRadius, (int)Mathf.Log(_playerLayer.value, 2));

        foreach (Collider col in hitColliders)
        {
            //_playerStats = col.GetComponent<CharacterStats>();
            _playerTransform = col.transform;
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
            AttackStart();
            EnableAttackCollider();
        }
        else
            _weapon.StartCoroutine(_weapon.Shoot());

        _curAttackCD = _weapon.attackCD + Time.time;
    }

    public void EnableAttackCollider()
    {
        _swordAttackBox.AttackStart();
    }

    public void DisableAttackCollider()
    {
        _swordAttackBox.AttackEnd();
    }

    public void AttackStart()
    {
        //isCurrentlyAttack = true;
        _swordAttackBox.CheckHit();
    }

    public void AttackEnd()
    {
        //isCurrentlyAttack = false;
    }
}