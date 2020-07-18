using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TempStart : MonoBehaviour {
	[SerializeField] AudioClip mainMenuAmbient;
	[SerializeField] AudioClip battleAmbient;
	[SerializeField] Button btn;

	private void Start() {
		OnRespawnEnd();

		GameManager.Instance.player.mover.onRespawnEnd += OnRespawnEnd;
	}

	public void StartGame() {
		Debug.Log("Starting game");

		float crossTime = 3.0f;

		AudioManager.Instance.FadeVolume(GameManager.Instance.ambient, 0.0f, crossTime);
		Destroy(GameManager.Instance.ambient, crossTime + 1.0f);
		GameManager.Instance.ambient = AudioManager.Instance.PlayFaded(battleAmbient, fadeTime: crossTime, channel: AudioManager.AudioChannel.Music);

		btn.gameObject.SetActive(false);

		GameManager.Instance.isPlaying = true;
	}

	void OnRespawnEnd() {
		GameManager.Instance.ambient = AudioManager.Instance.PlayLoopFaded(mainMenuAmbient, channel: AudioManager.AudioChannel.Music);
		btn.gameObject.SetActive(true);
	}
}
