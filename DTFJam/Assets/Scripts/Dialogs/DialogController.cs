using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using System;

public class DialogController : MonoBehaviour
{
    [SerializeField] private List<Dialog> _dialogs = new List<Dialog>();
    [SerializeField] private List<Dialog> _dialogs2 = new List<Dialog>();

    [Space]
    [SerializeField] private Image _characterImage = null;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private TextMeshProUGUI _textName;
    [SerializeField] private Animator cameraAnimator;
    [SerializeField] private AudioClip noiceClip;
    private int _curDialogNumber;

    [Header("Parameters")]
    [SerializeField] private int _textDelay = 5;

    private CanvasGroup canvasGroup;
    bool _switchDialog = false;
    bool _isClosing = false;
    bool isShowed = false;

    Action onEndDialogue;
    int currDialogId = 0;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        if (_isClosing)
        {
            canvasGroup.alpha -= Time.deltaTime * 2f;

            if (canvasGroup.alpha <= 0) {
                _isClosing = false;
            }
        }
        else if (isShowed && canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime;
        }

        if (!isShowed)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame || 
            Mouse.current.rightButton.wasPressedThisFrame ||
            Keyboard.current.anyKey.wasPressedThisFrame ||
            Gamepad.current.buttonEast.wasPressedThisFrame ||
            Gamepad.current.buttonNorth.wasPressedThisFrame ||
            Gamepad.current.buttonSouth.wasPressedThisFrame ||
            Gamepad.current.buttonWest.wasPressedThisFrame ||
            Gamepad.current.leftShoulder.wasPressedThisFrame ||
            Gamepad.current.rightShoulder.wasPressedThisFrame
            )
            _switchDialog = Next();
    }

    public void StartDialogue(Action onEndDialogue) {
        if(currDialogId >= 2 && GameManager.Instance.levelsManager.CurrLevel == 0) {
            onEndDialogue?.Invoke();
        }
        else {
            isShowed = true;
            this.onEndDialogue = onEndDialogue;
            StartCoroutine(MainCoroutine());
        }
    }

    private IEnumerator MainCoroutine()
    {
        List<Dialog> currDialog = currDialogId == 0 ? _dialogs : _dialogs2;

        for (_curDialogNumber = 0; _curDialogNumber < currDialog.Count; _curDialogNumber++)
        {

            _switchDialog = false;
            _text.maxVisibleCharacters = 1;
            _characterImage.sprite = currDialog[_curDialogNumber].character;
            _text.text = currDialog[_curDialogNumber].text;
            _textName.text = currDialog[_curDialogNumber].name;

            int delayCounter = 0;

            if (_curDialogNumber == 0 && currDialogId == 0) {
                cameraAnimator.SetTrigger("Dialog1Glitch");
                AudioSource source = null;
                LeanTween.delayedCall(0.1f, () => { 
                    source = AudioManager.Instance.PlayFaded(noiceClip,fadeTime: 0.2f, channel: AudioManager.AudioChannel.Sound);
                });
                LeanTween.delayedCall(1f, () => {
                    AudioManager.Instance.FadeVolume(source, 0.0f, 0.2f);
                });
            }

            while (_switchDialog == false)
            {
                if (delayCounter >= _textDelay && _text.maxVisibleCharacters < currDialog[_curDialogNumber].text.Length)
                {
                    delayCounter = 0;
                    _text.maxVisibleCharacters++;
                }

                delayCounter++;

                yield return new WaitForEndOfFrame();
            }
        }

        _isClosing = true;
        isShowed = false;
        _switchDialog = false;
        ++currDialogId;

        onEndDialogue?.Invoke();
        onEndDialogue = null;
    }

    private bool Next()
    {
        List<Dialog> currDialog = currDialogId == 0 ? _dialogs : _dialogs2;
      
        if (_curDialogNumber < currDialog.Count && _text.maxVisibleCharacters < currDialog[_curDialogNumber].text.Length)
        {
            _text.maxVisibleCharacters = currDialog[_curDialogNumber].text.Length;
            return false;
        }
        else
        {
            return true;
        }
    }
}

[Serializable]
public struct Dialog
{
    public string name;
    public string text;
    public Sprite character;
}