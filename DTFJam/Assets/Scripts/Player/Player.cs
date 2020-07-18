using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	[Header("Refs")]
	[Space]
	[SerializeField] Health playerHealth;
	[SerializeField] PlayerTimer timer;

	int comboCounter = 0;

	private void Awake() {
		GameManager.Instance.player = this;
	}

	private void OnDestroy() {
		if(GameManager.Instance.player == this)
			GameManager.Instance.player = null;
	}

	public int IncreaseComboCounter() {
		return ++comboCounter;
	}
}
