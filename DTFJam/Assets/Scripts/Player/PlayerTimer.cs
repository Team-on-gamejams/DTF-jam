using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using NaughtyAttributes;

public class PlayerTimer : MonoBehaviour {
	[SerializeField] float maxTimer = 60;
	[SerializeField] float minTimer = 0;
	[Space]
	[SerializeField] float halfValue = 0.5f;
	[SerializeField] float almostEmptyValue = 0.08f;
	[SerializeField] Color halfColor = Color.yellow;
	[SerializeField] Color almostEmptyColor = Color.red;
	[SerializeField] Color defaultColor = Color.white;
	[Space]
	[SerializeField] [MinMaxSlider(0, 12, false)] Vector2 shakeAmount = new Vector2(1, 3);

	[Header("Refs")]
	[Space]
	[SerializeField] Image fillderImage;
	[SerializeField] TextMeshProUGUI textField;

	[Header("Debug")]
	[Space]
	[SerializeField] Slider slider;

	float currTime;
	StringBuilder sb = new StringBuilder(5);
	Vector2 defaultTextFieldPos;

	private void Awake() {
		currTime = maxTimer;

		halfValue = Mathf.RoundToInt(maxTimer * halfValue);
		almostEmptyValue = Mathf.RoundToInt(maxTimer * almostEmptyValue);

		defaultTextFieldPos = textField.rectTransform.anchoredPosition;

		slider.minValue = minTimer;
		slider.maxValue = maxTimer;
		slider.SetValueWithoutNotify(currTime);
	}

	private void Update() {
		if (currTime != minTimer)
			return;

		currTime -= Time.unscaledDeltaTime; //TODO: not sure is we need to slow-mo timer? Discuss it
		if (currTime <= minTimer) {
			currTime = minTimer;
			//TODO: end of game
		}

		slider.SetValueWithoutNotify(currTime);  //DEBUG

		fillderImage.fillAmount = currTime / maxTimer;
		sb.Clear();
		sb.Append(((int)currTime).ToString("00"));
		sb.Append(":");
		sb.Append((currTime % 1 * 100).ToString("00"));
		textField.text = sb.ToString();

		if(currTime <= almostEmptyValue) {
			textField.color = fillderImage.color = almostEmptyColor;

			//Quaternion prevRot = textField.transform.rotation;
			//float z = Random.value * 20 - (10);
			//textField.transform.eulerAngles = new Vector3(prevRot.x, prevRot.y, prevRot.z + z);

			textField.rectTransform.anchoredPosition = defaultTextFieldPos + Random.insideUnitCircle * Mathf.Lerp(shakeAmount.x, shakeAmount.y, 1.0f - Mathf.Clamp01(currTime / almostEmptyValue));
		}
		else if (currTime <= halfValue) {
			textField.color = fillderImage.color = halfColor;
			textField.rectTransform.anchoredPosition = defaultTextFieldPos;
		}
		else {
			textField.color = fillderImage.color = defaultColor;
			textField.rectTransform.anchoredPosition = defaultTextFieldPos;
		}
	}

	public void OnSliderValueChange(float val) {
		currTime = val;
	}
}
