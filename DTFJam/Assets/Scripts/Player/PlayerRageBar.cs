using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Action = System.Action;

public class PlayerRageBar : MonoBehaviour {
	static readonly Color x10Color = Color.red;
	static readonly Color x5Color = Color.yellow;
	static readonly Color defaultColor = new Color(0.4f, 0.4f, 0.4f);
	const float minFontSize = 30.0f;
	const float maxFontSize = 50.0f;
	const float minCombo = 1.0f;
	const float maxCombo = 10.0f - minCombo;

	public float CurrRage => currRage;
	public float CurrRagePersent => currRage / maxRage;

	public Action onRageValueChange = null;

	[SerializeField] float minRage = 0;
	[SerializeField] float maxRage = 100;
	[SerializeField] float ragePerCombo = 10;
	[Space]
	[SerializeField] float rageDecreaseTimeout = 3.0f;
	[SerializeField] float rageDecreaseSpeed = 10.0f;

	[Header("UI")]
	[Space]
	[SerializeField] RectTransform barParent = null;
	[SerializeField] Slider barFirst = null;
	[SerializeField] Slider barSecond = null;
	[SerializeField] TextMeshProUGUI comboTextField = null;
	[Header("UI Tweens time")]
	[Space]
	[SerializeField] float firstBarTime = 0.2f;
	[SerializeField] float secondBarTime = 1.0f;
	[SerializeField] [MinMaxSlider(0, 50, false)] Vector2 shakeAmount = new Vector2(0, 5);

	float currRage;
	float currRageDecreaseTimeout;
	int comboCounter = 0;

	Vector2 defaultBarPos;
	Vector2 defaultBarScale;
	Vector2 defaultTextPos;

	private void Awake() {
		barFirst.minValue = minRage;
		barFirst.maxValue = maxRage;

		barSecond.minValue = minRage;
		barSecond.maxValue = maxRage;

		defaultBarPos = barParent.anchoredPosition;
		defaultBarScale = barParent.localScale;
		defaultTextPos = comboTextField.rectTransform.anchoredPosition;

		Init();
	}

	private void Update() {
		if (!GameManager.Instance.isPlaying)
			return;

		if (currRageDecreaseTimeout == rageDecreaseTimeout) {
			if(currRage != minRage) {
				currRage -= rageDecreaseSpeed * Time.deltaTime;
				if (currRage < minRage)
					currRage = minRage;
				UpdateBarForce();
				onRageValueChange.Invoke();
			}

			barParent.anchoredPosition = defaultBarPos;
			barParent.localScale = defaultBarScale;
			comboTextField.rectTransform.anchoredPosition = defaultTextPos;
		}
		else {
			currRageDecreaseTimeout += Time.deltaTime;
			if (currRageDecreaseTimeout >= rageDecreaseTimeout) {
				currRageDecreaseTimeout = rageDecreaseTimeout;
				ResetComboCounter();
			}

			Vector2 offset = new Vector2(0, Random.Range(-1.0f, 1.0f));
			barParent.anchoredPosition = defaultBarPos + offset * Mathf.Lerp(shakeAmount.x, shakeAmount.y, currRage / maxRage);

			offset.x = Random.Range(
				1.0f, 
				Mathf.Lerp(1.0f, 1.025f, currRage / maxRage)
			);
			offset.y = 1.0f;
			barParent.localScale = offset;

			comboTextField.rectTransform.anchoredPosition = defaultTextPos + Random.insideUnitCircle * Mathf.Lerp(shakeAmount.x, shakeAmount.y, currRage / maxRage);
		}
	}

	public void Init() {
		currRage = 0.0f;
		currRageDecreaseTimeout = 0.0f;
		barFirst.value = currRage;
		barSecond.value = currRage;
		comboTextField.text = " ";

		onRageValueChange?.Invoke();
	}

	public void AddRage(float value) {
		currRageDecreaseTimeout = 0.0f;

		currRage += value;
		if (currRage > maxRage)
			currRage = maxRage;

		UpdateBar();
		onRageValueChange.Invoke();
	}

	public int IncreaseComboCounter() {
		++comboCounter;
		SetComboColor(comboCounter, comboTextField);
		comboTextField.text = $"x{comboCounter}";

		AddRage(ragePerCombo);
		return comboCounter;
	}

	void ResetComboCounter() {
		comboCounter = 0;
		comboTextField.text = $" ";
	}

	void UpdateBarForce() {
		LeanTween.cancel(barFirst.gameObject, false);
		LeanTween.cancel(barSecond.gameObject, false);
		barSecond.value = barFirst.value = currRage;
	}

	void UpdateBar() {
		LeanTween.cancel(barFirst.gameObject, false);
		LeanTween.value(barFirst.gameObject, barFirst.value, currRage, firstBarTime)
		.setOnUpdate((float rage) => {
			barFirst.value = rage;
		});

		LeanTween.cancel(barSecond.gameObject, false);
		LeanTween.value(barSecond.gameObject, barSecond.value, currRage, secondBarTime)
		.setEase(LeanTweenType.easeInQuad)
		.setOnUpdate((float rage) => {
			barSecond.value = rage;
		});
	}

	public static void SetComboColor(int currCombo, TextMeshProUGUI textField) {
		if (currCombo >= 10)
			textField.color = x10Color;
		else if (currCombo >= 5)
			textField.color = x5Color;
		else
			textField.color = defaultColor;
	}
	
	public static void SetComboSize(int currCombo, TextMeshProUGUI textField) {
		textField.fontSize = Mathf.Lerp(minFontSize, maxFontSize, (currCombo - minCombo) / maxCombo);
	}
}
