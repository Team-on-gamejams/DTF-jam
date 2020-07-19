using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelsManager : MonoBehaviour {
	[SerializeField] Level[] levels;
	int currLevel = 0;

	private void Awake() {
		GameManager.Instance.levelsManager = this;
	}

	private void OnDestroy() {
		if(GameManager.Instance.levelsManager == this)
			GameManager.Instance.levelsManager = null;
	}

	public void LoadFirstLevel() {
		for (int i = 0; i < levels.Length; ++i) {
			if (i == currLevel) {
				levels[i].InitLevel();
			}
			else {
				levels[i].SetAsUnusedAtInit();
			}
		}
	}
}
