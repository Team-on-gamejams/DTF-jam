using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Invector.vCharacterController;
using UnityForge.PropertyDrawers;

public class PlayerMover : MonoBehaviour {
	[Header("Mouse pointer")][Space]
	[SerializeField] /*[GameObjectLayer]*/ int mouseRaycastLayer;
	RaycastHit[] mouseRaycastHits = new RaycastHit[5];

	[Header("Attack values")]
	[Space]
	[SerializeField] Collider swordAttackBoxCollider;
	bool isCurrentlyAttack = false;

	[Header("Refs")][Space]
	[SerializeField] Transform mouseRaycastTransform;
	[SerializeField] Camera mainCamera;

	[Header("This Refs")][Space]
	[SerializeField] vThirdPersonController cc;
	[SerializeField] Animator anim;


	Vector3 moveInput;
	Vector3 mousePos;
	Vector3 lookInput;
	bool isAttackMelee;
	bool isDash;

	void Awake() {
		mouseRaycastLayer = 1 << mouseRaycastLayer;
	}

	void Start() {
		cc.Init();
		cc.rotateTarget = mouseRaycastTransform;
	}

	void FixedUpdate() {
		cc.UpdateMotor();               // updates the ThirdPersonMotor methods
		cc.ControlLocomotionType();     // handle the controller locomotion type and movespeed
		cc.ControlRotationType();       // handle the controller rotation type
	}

	void Update() {
		ProcessInput();
		cc.UpdateAnimator();            // updates the Animator Parameters
	}

	void OnAnimatorMove() {
		cc.ControlAnimatorRootMotion(); // handle root motion animations 
	}

	void ProcessInput() {
		cc.input.x = moveInput.x;
		cc.input.z = moveInput.y;

		cc.UpdateMoveDirection(mainCamera.transform);

		Ray ray = mainCamera.ScreenPointToRay(mousePos);
		if (Physics.Raycast(ray, out RaycastHit hit, 100, mouseRaycastLayer)) {
			mouseRaycastTransform.position = hit.point;
		}
	}

	#region Animators callback
	public void EnableAttackCollider() {
		swordAttackBoxCollider.enabled = true;
	}

	public void DisableAttackCollider() {
		swordAttackBoxCollider.enabled = false;
	}
	
	public void AttackStart() {
		isCurrentlyAttack = true;
	}

	public void AttackEnd() {
		isCurrentlyAttack = false;
	}

	#endregion

	#region Input handling
	public void OnMove(InputAction.CallbackContext context) {
		moveInput = context.ReadValue<Vector2>();

	}

	public void OnMousePos(InputAction.CallbackContext context) {
		mousePos = context.ReadValue<Vector2>();

	}

	public void OnLook(InputAction.CallbackContext context) {
		lookInput = context.ReadValue<Vector2>();

	}

	public void OnAttackMelee(InputAction.CallbackContext context) {
		isAttackMelee = context.ReadValueAsButton();

		if (!isCurrentlyAttack && isAttackMelee && context.phase == InputActionPhase.Started) {
			anim.SetTrigger("IsAttack");
		}
	}

	public void OnDash(InputAction.CallbackContext context) {
		isDash = context.ReadValueAsButton();

	}
	#endregion
}
