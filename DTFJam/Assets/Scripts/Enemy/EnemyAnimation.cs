using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Invector.vCharacterController;
using UnityEngine.AI;

public class EnemyAnimation : MonoBehaviour
{
	[Header("Audio")]
	[Space]
	[SerializeField] AudioClip[] stepSonds;
	int currStepSound = 0;

	//[Header("Refs")]
	//[Space]
	//[SerializeField] Transform mouseRaycastTransform;
	//[SerializeField] Camera mainCamera;
	//[SerializeField] Health health;
	//[SerializeField] Collider collider;

	[Header("This Refs")]
	[Space]
	[SerializeField] vThirdPersonController cc;
	public Animator anim;
	[SerializeField] Rigidbody rb;
	[SerializeField] private DealDamageOnTriggerEnter _swordAttackBox;
	private Weapon _weapon;
	private NavMeshAgent _navAgent;
	private Camera _mainCamera;
	public Transform playerTransform;

	bool isAttackMelee;
	bool isCurrentlyDashing = false;

	float defaultRunSpeed;
	Vector3 startPos;

	const float minStepTime = 0.1f;
	float currStepTime = 0.0f;

	void Awake()
	{
		_weapon = GetComponentInChildren<Weapon>();
		_navAgent = GetComponent<NavMeshAgent>();
		_mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
		defaultRunSpeed = cc.strafeSpeed.runningSpeed;
		startPos = transform.position;
	}

	void Start()
	{
		cc.Init();
		cc.rotateTarget = transform;
	}

	void FixedUpdate()
	{
		cc.UpdateMotor();               // updates the ThirdPersonMotor methods
		cc.ControlLocomotionType();     // handle the controller locomotion type and movespeed
		cc.ControlRotationType();       // handle the controller rotation type
		Vector3 moveVector = Vector3.Normalize(_navAgent.velocity);
		cc.input.x = moveVector.x;
		cc.input.z = moveVector.z;
		cc.UpdateMoveDirection(_mainCamera.transform);
	}

	void Update()
	{
		currStepTime += Time.deltaTime;
		cc.UpdateAnimator();            // updates the Animator Parameters
	}

	void OnAnimatorMove()
	{
		cc.ControlAnimatorRootMotion(); // handle root motion animations 
	}

    #region Animators callback
    public void EnableAttackCollider()
    {
		if(_weapon.attackType == AttackType.Melee)
			_swordAttackBox.AttackStart();
    }

    public void DisableAttackCollider()
    {
		if (_weapon.attackType == AttackType.Melee)
			_swordAttackBox.AttackEnd();
    }

    public void AttackStart()
    {
		if (_weapon.attackType == AttackType.Melee)
			_swordAttackBox.CheckHit();
    }

    public void AttackEnd()
    {

    }

    void StepSound()
	{
		if (currStepTime > minStepTime)
		{
			currStepTime = 0;
			AudioManager.Instance.Play(stepSonds[currStepSound++], channel: AudioManager.AudioChannel.Sound);
			if (currStepSound >= stepSonds.Length)
				currStepSound = 0;
		}
	}
	#endregion
}
