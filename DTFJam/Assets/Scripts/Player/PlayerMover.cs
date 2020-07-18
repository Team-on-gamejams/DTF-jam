using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Invector.vCharacterController;

public class PlayerMover : MonoBehaviour {
	public Action<bool> onChangeControls;   // 0 - keyboard, 1 - gamepad

	[Header("Mouse pointer")][Space]
	[SerializeField] /*[GameObjectLayer]*/ int mouseRaycastLayer;
	RaycastHit[] mouseRaycastHits = new RaycastHit[5];

	[Header("Attack values")]
	[Space]
	[SerializeField] DealDamageOnTriggerEnter swordAttackBox;
	bool isCurrentlyAttack = false;

	[Header("Refs")][Space]
	[SerializeField] Transform mouseRaycastTransform;
	[SerializeField] Camera mainCamera;

	[Header("This Refs")][Space]
	[SerializeField] vThirdPersonController cc;
	[SerializeField] Animator anim;

	bool isUseGamepadControl;
	bool isGamepadLookInput;
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

		if (isUseGamepadControl) {
			mouseRaycastTransform.position = transform.position + new Vector3(lookInput.x, 0, lookInput.y);
		}
		else {
			Ray ray = mainCamera.ScreenPointToRay(mousePos);
			if (Physics.Raycast(ray, out RaycastHit hit, 100, mouseRaycastLayer)) {
				mouseRaycastTransform.position = hit.point;
			}
		}
	}

	#region Animators callback
	public void EnableAttackCollider() {
		swordAttackBox.AttackStart();
	}

	public void DisableAttackCollider() {
		swordAttackBox.AttackEnd();
	}

	public void AttackStart() {
		isCurrentlyAttack = true;
		swordAttackBox.CheckHit();
	}

	public void AttackEnd() {
		isCurrentlyAttack = false;
	}

	#endregion

	#region Input handling
	public void OnMove(InputAction.CallbackContext context) {
		CheckIsUseGamepad(context.control.device);

		if (!GameManager.Instance.isPlaying)
			return;

		moveInput = context.ReadValue<Vector2>();

		if (isUseGamepadControl && !isGamepadLookInput)
			lookInput = moveInput;
	}

	public void OnMousePos(InputAction.CallbackContext context) {
		CheckIsUseGamepad(context.control.device);
		isGamepadLookInput = false;

		if (!GameManager.Instance.isPlaying)
			return;

		mousePos = context.ReadValue<Vector2>();
	}

	public void OnLook(InputAction.CallbackContext context) {
		CheckIsUseGamepad(context.control.device);
		isGamepadLookInput = true;

		if (!GameManager.Instance.isPlaying)
			return;

		lookInput = context.ReadValue<Vector2>();
	}

	public void OnAttackMelee(InputAction.CallbackContext context) {
		CheckIsUseGamepad(context.control.device);

		if (!GameManager.Instance.isPlaying)
			return;

		isAttackMelee = context.ReadValueAsButton();

		if (!isCurrentlyAttack && isAttackMelee && context.phase == InputActionPhase.Started) {
			anim.SetTrigger("IsAttack");
		}
	}

	public void OnDash(InputAction.CallbackContext context) {
		CheckIsUseGamepad(context.control.device);

		if (!GameManager.Instance.isPlaying)
			return;

		isDash = context.ReadValueAsButton();

	}

	void CheckIsUseGamepad(InputDevice device) {
		bool isGamepadInput = device is Gamepad;
		if (isGamepadInput != isUseGamepadControl) {
			isUseGamepadControl = isGamepadInput;
			onChangeControls?.Invoke(isUseGamepadControl);
		}
	}
	#endregion
}
