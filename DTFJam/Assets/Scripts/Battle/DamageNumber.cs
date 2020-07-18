using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour {
	const float minFontSize = 30.0f;
	const float maxFontSize = 50.0f;
	const float minCombo = 1.0f;
	const float maxCombo = 10.0f - minCombo;

	readonly Color x10Color = Color.red;
	readonly Color x5Color = Color.yellow;
	readonly Color defaultColor = new Color(0.4f, 0.4f, 0.4f);

	[SerializeField] TextMeshProUGUI damageTextField;

	public void StartSequence(int currCombo) {
		damageTextField.text = $"x{currCombo}";

		if(currCombo >= 10)
			damageTextField.color = x10Color;
		else if (currCombo >= 5)
			damageTextField.color = x5Color;
		else
			damageTextField.color =  defaultColor;

		damageTextField.fontSize = Mathf.Lerp(minFontSize, maxFontSize, (currCombo - minCombo) / maxCombo);

		damageTextField.transform.localScale = Vector3.one * 0.7f;

		Destroy(gameObject, 0.7f);
		LeanTweenEx.StayWorldPosAndMoveUp(damageTextField.gameObject, 0.7f, 0.8f, Vector3.zero);
		LeanTween.value(gameObject, damageTextField.transform.localScale.x, 1.0f, 0.1f)
			.setOnUpdate((float scale) => {
				damageTextField.transform.localScale = Vector3.one * scale;
			})
			.setOnComplete(() => {
				LeanTween.value(gameObject, damageTextField.transform.localScale.x, 0.7f, 0.3f)
				.setOnUpdate((float scale) => {
					damageTextField.transform.localScale = Vector3.one * scale;
				})
				.setOnComplete(() => {
					LeanTween.value(gameObject, damageTextField.color.a, 0.0f, 0.3f)
					.setOnUpdate((float a) => {
						Color c = damageTextField.color;
						c.a = a;
						damageTextField.color = c;
					});
				});
			});
	}
}
