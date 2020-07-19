using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewLevelOnTriggerEnter : MonoBehaviour {
	[SerializeField] [Layer] int playerLayer;

	private void OnTriggerEnter(Collider other) {
		if(other.gameObject.layer == playerLayer) {
			GameManager.Instance.player.TransitToNewLevel(GameManager.Instance.levelsManager.LoadNextLevel);
		}
	}
}
