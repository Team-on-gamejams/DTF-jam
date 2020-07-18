using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour {
	

	[SerializeField] TextMeshProUGUI damageTextField;

	public void StartSequence(int currCombo) {
		damageTextField.text = $"x{currCombo}";

		PlayerRageBar.SetComboColor(currCombo, damageTextField);
		PlayerRageBar.SetComboSize(currCombo, damageTextField);
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
