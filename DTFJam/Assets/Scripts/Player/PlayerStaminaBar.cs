using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Action = System.Action;

public class PlayerStaminaBar : MonoBehaviour {
	public float regenerationMultiplier = 1.0f;

	[SerializeField] float minStamina = 0;
	[SerializeField] float maxStamina = 100;
	[Space]
	[SerializeField] float staminaIncreaseTimeout = 0.5f;
	[SerializeField] float staminaIncreaseTimeoutIfConsumeAll = 1.0f;
	[SerializeField] float staminaIncreaseSpeed = 10.0f;

	[Header("UI")]
	[Space]
	[SerializeField] Slider barFirst = null;
	[SerializeField] Slider barSecond = null;
	[Header("UI Tweens time")]
	[Space]
	[SerializeField] float firstBarTime = 0.2f;
	[SerializeField] float secondBarTime = 1.0f;

	float currStamina;
	float currStaminaIncreaseTimeout;

	private void Awake() {
		currStamina = maxStamina;
		currStaminaIncreaseTimeout = staminaIncreaseTimeout;

		barFirst.minValue = minStamina;
		barFirst.maxValue = maxStamina;
		barFirst.value = currStamina;

		barSecond.minValue = minStamina;
		barSecond.maxValue = maxStamina;
		barSecond.value = currStamina;
	}

	private void Update() {
		if (!GameManager.Instance.isPlaying)
			return;

		if (currStaminaIncreaseTimeout == staminaIncreaseTimeout) {
			if (currStamina != maxStamina) {
				currStamina += staminaIncreaseSpeed * regenerationMultiplier * Time.deltaTime;
				if (currStamina > maxStamina)
					currStamina = maxStamina;
				UpdateBarForce();
			}
		}
		else {
			currStaminaIncreaseTimeout += Time.deltaTime;
			if (currStaminaIncreaseTimeout >= staminaIncreaseTimeout)
				currStaminaIncreaseTimeout = staminaIncreaseTimeout;
		}
	}

	public bool IsEnoughStamina(float value) {
		return currStamina != 0;
	}

	public void DecreaseStamina(float value) {
		currStamina -= value;
		
		if (currStamina <= 0) {
			currStamina = 0;
			currStaminaIncreaseTimeout = -(staminaIncreaseTimeoutIfConsumeAll - staminaIncreaseTimeout);
		}
		else {
			currStaminaIncreaseTimeout = 0.0f;
		}

		UpdateBar();
	}

	void UpdateBarForce() {
		LeanTween.cancel(barFirst.gameObject, false);
		barFirst.value = currStamina;
		if (barFirst.value >= barSecond.value)
			barSecond.value = currStamina;
	}

	void UpdateBar() {
		LeanTween.cancel(barFirst.gameObject, false);
		LeanTween.value(barFirst.gameObject, barFirst.value, currStamina, firstBarTime)
		.setOnUpdate((float stamina) => {
			barFirst.value = stamina;
		});

		LeanTween.cancel(barSecond.gameObject, false);
		LeanTween.value(barSecond.gameObject, barSecond.value, currStamina, secondBarTime)
		.setEase(LeanTweenType.easeInQuad)
		.setOnUpdate((float stamina) => {
			barSecond.value = stamina;
		});
	}
}
