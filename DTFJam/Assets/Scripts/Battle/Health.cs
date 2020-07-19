using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Health : MonoBehaviour {
	public bool isUnderPlayerAttack = false;
	public bool isCanTakeDamage = true;

	[Header("Hp")] [Space]
	[SerializeField] float maxHits = 1;
	float currHit;

	[Header("UI")] [Space]
	[SerializeField] bool needShowOnInit = false;
	[SerializeField] bool boolNeedToFindUI = false;
	[SerializeField] Slider barFirst = null;
	[SerializeField] Slider barSecond = null;
	[SerializeField] TextMeshProUGUI hpTextField = null;
	[Header("UI Tweens time")] [Space]
	[SerializeField] float firstBarTime = 0.2f;
	[SerializeField] float secondBarTime = 1.0f;

	[Header("Prefabs")] [Space]
	[SerializeField] GameObject healthCanvasPrefab = null;
	[SerializeField] GameObject damageNumberPrefab = null;

	[Header("Refs")] [Space]
	[SerializeField] Transform healthBarPos = null;

	protected virtual void Awake() {
		Init();

		if (barFirst == null && boolNeedToFindUI) {
			FindBars();

			if (barFirst == null && healthCanvasPrefab != null) {
				GameObject healthCanvas = Instantiate(healthCanvasPrefab, healthBarPos.position, Quaternion.identity, healthBarPos);
				healthCanvas.layer = LayerMask.NameToLayer("UI");

				FindBars();
			}

			if(barFirst == null) {
				Debug.LogWarning($"There are no HP bar for {transform.name}");
			}
		}

		void FindBars() {
			Slider[] sliders = GetComponentsInChildren<Slider>();

			if (sliders.Length == 1) {
				barFirst = sliders[0];
			}
			else if (sliders.Length >= 2) {
				barFirst = sliders[1];
				barSecond = sliders[0];
			}

			hpTextField = GetComponentInChildren<TextMeshProUGUI>();
		}
	}

	void Start() {
		if (barFirst != null) {
			barFirst.gameObject.SetActive(needShowOnInit);
			barFirst.minValue = 0.0f;
			barFirst.maxValue = maxHits;
			barFirst.value = maxHits;

			if (barSecond != null) {
				barSecond.gameObject.SetActive(needShowOnInit);
				barSecond.minValue = 0.0f;
				barSecond.maxValue = maxHits;
				barSecond.value = maxHits;
			}
		}

		if (hpTextField != null) {
			hpTextField.gameObject.SetActive(needShowOnInit);
			hpTextField.text = $"{Mathf.RoundToInt(currHit)}/{Mathf.RoundToInt(maxHits)}";
		}
	}

	public void Init() {
		currHit = maxHits;
	}

	public void ShowBarIfHided() {
		if (barFirst != null && !barFirst.gameObject.activeSelf) {
			ShowBar();
		}
	}

	public void ShowBar() {
		if (barFirst != null) {
			barFirst.gameObject.SetActive(true);
			if (barSecond != null)
				barSecond.gameObject.SetActive(true);
		}
		if (hpTextField != null)
			hpTextField.gameObject.SetActive(true);
	}

	public void HideBar() {
		if (barFirst != null) {
			barFirst.gameObject.SetActive(false);
			if (barSecond != null)
				barSecond.gameObject.SetActive(false);
		}
		if (hpTextField != null)
			hpTextField.gameObject.SetActive(false);
	}

	public void GetHit() {
		if (currHit == 0 || !isCanTakeDamage)
			return;

		ShowBarIfHided();

		TakeHit();
	}

	void TakeHit() {
		--currHit;
		if (currHit <= 0) {
			currHit = 0;
		}

		UpdateHpBar();

		if (currHit == 0)
			DieInternal();
	}

	virtual protected void DieInternal() {
		if (isUnderPlayerAttack) {
			ShowComboNumber(GameManager.Instance.player.IncreaseComboCounter(), healthBarPos.position);
		}

		SendMessage("Die", SendMessageOptions.DontRequireReceiver);
	}

	GameObject ShowComboNumber(int currCombo, Vector3 pos) {
		GameObject damageNumber = Instantiate(damageNumberPrefab, pos, Quaternion.identity, null);
		damageNumber.transform.localPosition += Vector3.up * 1.2f;
		damageNumber.GetComponent<DamageNumber>().StartSequence(currCombo);
		return damageNumber;
	}

	void UpdateHpBar() {
		if (barFirst != null) {
			LeanTween.cancel(barFirst.gameObject, false);
			LeanTween.value(barFirst.gameObject, barFirst.value, currHit, firstBarTime)
			.setOnUpdate((float hp) => {
				barFirst.value = hp;
			});

			if (barSecond != null) {
				LeanTween.cancel(barSecond.gameObject, false);
				LeanTween.value(barSecond.gameObject, barSecond.value, currHit, secondBarTime)
				.setEase(LeanTweenType.easeInQuad)
				.setOnUpdate((float hp) => {
					barSecond.value = hp;
				});
			}
		}

		if (hpTextField != null)
			hpTextField.text = $"{Mathf.RoundToInt(currHit)}/{Mathf.RoundToInt(maxHits)}";
	}
}
