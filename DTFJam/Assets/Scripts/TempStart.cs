using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TempStart : MonoBehaviour {
	[SerializeField] AudioClip mainMenuAmbient;
	[SerializeField] AudioClip battleAmbient;
	[SerializeField] Button btn;

	AudioSource ambient;

	private void Start() {
		ambient = AudioManager.Instance.PlayLoopFaded(mainMenuAmbient, channel: AudioManager.AudioChannel.Music);
	}

	public void StartGame() {
		Debug.Log("Starting game");

		float crossTime = 3.0f;

		AudioManager.Instance.FadeVolume(ambient, 0.0f, crossTime);
		ambient = AudioManager.Instance.PlayLoopFaded(battleAmbient, fadeTime: crossTime, channel: AudioManager.AudioChannel.Music);

		btn.gameObject.SetActive(false);

		GameManager.Instance.isPlaying = true;
	}
}
