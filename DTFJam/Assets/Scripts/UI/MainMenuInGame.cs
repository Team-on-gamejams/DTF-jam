using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuInGame : MonoBehaviour {
	[Header("Audio")]
	[Space]
	[SerializeField] AudioClip mainMenuAmbient;
	[SerializeField] AudioClip battleAmbient;

	[Header("Refs")]
	[Space]
	[SerializeField] Button firstButton;
	[SerializeField] Button newGameBtn;
	[SerializeField] Button continueGame;
	[SerializeField] CanvasGroup cg;
	[SerializeField] Image startFader;
	[SerializeField] Animator animCamera;

	private void Start() {
		GameManager.Instance.levelsManager.LoadFirstLevel();
		OnRespawnEnd();

		animCamera.SetTrigger("IsLoadingNewLevel");
		LeanTween.value(startFader.color.a, 0.0f, 0.5f)
			.setDelay(0.5f)
			.setOnUpdate((float a) => {
				Color c = startFader.color;
				c.a = a;
				startFader.color = c;
			});

		GameManager.Instance.player.mover.onRespawnEnd += OnRespawnEnd;
	}

	private void OnApplicationFocus(bool focus) {
		if(focus && cg.interactable)
			firstButton.Select();
	}

	public void NewGame() {
		LeanTweenEx.ChangeCanvasGroupAlpha(cg, 0.0f, 0.2f);
		cg.blocksRaycasts = cg.interactable = false;
		GameManager.Instance.player.dialog.StartDialogue(OnDialogueEnd);
	}

	public void ExitGame() {
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBGL
#else
		Application.Quit();
#endif
	}

	public void StartGameOnNewLevel() {
		GameManager.Instance.player.dialog.StartDialogue(OnDialogueEnd);
	}

	void OnDialogueEnd() {
		Debug.Log("Starting game");

		if (newGameBtn.gameObject.activeSelf) {
			newGameBtn.gameObject.SetActive(false);
			continueGame.gameObject.SetActive(true);
			firstButton = continueGame;
		}

		float crossTime = 3.0f;

		if (GameManager.Instance.ambient != null) {
			AudioManager.Instance.FadeVolume(GameManager.Instance.ambient, 0.0f, crossTime);
			Destroy(GameManager.Instance.ambient.gameObject, crossTime + 1.0f);
		}
		GameManager.Instance.ambient = AudioManager.Instance.PlayFaded(battleAmbient, fadeTime: crossTime, channel: AudioManager.AudioChannel.Music);

		GameManager.Instance.isPlaying = true;
	}

	void OnRespawnEnd() {
		GameManager.Instance.ambient = AudioManager.Instance.PlayLoopFaded(mainMenuAmbient, channel: AudioManager.AudioChannel.Music);

		LeanTweenEx.ChangeCanvasGroupAlpha(cg, 1.0f, 0.5f)
			.setDelay(1.0f);
		cg.blocksRaycasts = cg.interactable = true;

		firstButton.Select();
	}
}
