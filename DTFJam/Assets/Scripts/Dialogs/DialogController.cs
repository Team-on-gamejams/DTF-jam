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
    [SerializeField] private List<Dialog> _dialogs3 = new List<Dialog>();

    [Space]
    [SerializeField] private Image _characterImage = null;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private TextMeshProUGUI _textName;
    [SerializeField] private Animator cameraAnimator;
    [SerializeField] private AudioClip noiceClip;
    [SerializeField] private AudioClip textTyping;
    [SerializeField] private AudioClip bossClip;
    private int _curDialogNumber;

    [Header("Parameters")]
    [SerializeField] private int _textDelay = 5;

    private CanvasGroup canvasGroup;
    bool _switchDialog = false;
    bool _isClosing = false;
    bool isShowed = false;

    Action onEndDialogue;
    int currDialogId = 0;
    AudioSource typing;

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

        if ((Mouse.current?.leftButton?.wasPressedThisFrame ?? false) || 
            (Mouse.current?.rightButton?.wasPressedThisFrame ?? false) ||
            (Keyboard.current?.anyKey?.wasPressedThisFrame ?? false) ||
            (Gamepad.current?.buttonEast?.wasPressedThisFrame ?? false) ||
            (Gamepad.current?.buttonNorth?.wasPressedThisFrame ?? false) ||
            (Gamepad.current?.buttonSouth?.wasPressedThisFrame ?? false) ||
            (Gamepad.current?.buttonWest?.wasPressedThisFrame ?? false) ||
            (Gamepad.current?.leftShoulder?.wasPressedThisFrame ?? false) ||
            (Gamepad.current?.rightShoulder?.wasPressedThisFrame ?? false)
            )
            _switchDialog = Next();
    }

    public void StartDialogue(Action onEndDialogue) {
        if((currDialogId == 0 || currDialogId == 1) && GameManager.Instance.levelsManager.CurrLevel == 0) {
            isShowed = true;
            this.onEndDialogue = onEndDialogue;
            StartCoroutine(MainCoroutine());
        }
       else if (GameManager.Instance.levelsManager.CurrLevel == 1) {
            currDialogId = 2;
            isShowed = true;
            this.onEndDialogue = onEndDialogue;
            StartCoroutine(MainCoroutine());
        }
        else {
            onEndDialogue?.Invoke();
        }
    }

    private IEnumerator MainCoroutine()
    {
        List<Dialog> currDialog = GetDialog();

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
            else if (_curDialogNumber == 0 && currDialogId == 2) {
                AudioManager.Instance.PlayFaded(bossClip, fadeTime: 0.2f, channel: AudioManager.AudioChannel.Sound);
            }

            typing = AudioManager.Instance.PlayLoop(textTyping, channel: AudioManager.AudioChannel.Sound);
            while (_switchDialog == false)
            {
                if (delayCounter >= _textDelay && _text.maxVisibleCharacters < currDialog[_curDialogNumber].text.Length)
                {
                    delayCounter = 0;
                    _text.maxVisibleCharacters++;

                    if (_text.maxVisibleCharacters > currDialog[_curDialogNumber].text.Length - 5) {
                        if(typing != null)
                            AudioManager.Instance.FadeVolume(typing, 0.0f, 0.2f);
                    }
                }

                delayCounter++;

                yield return new WaitForEndOfFrame();
            }
            if(typing != null)
                Destroy(typing.gameObject);
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
        List<Dialog> currDialog = GetDialog();
      
        if (_curDialogNumber < currDialog.Count && _text.maxVisibleCharacters < currDialog[_curDialogNumber].text.Length)
        {
            if (typing != null)
                Destroy(typing.gameObject);
            _text.maxVisibleCharacters = currDialog[_curDialogNumber].text.Length;
            return false;
        }
        else
        {
            return true;
        }
    }

    List<Dialog> GetDialog() {
        switch (currDialogId) {
            case 0:
                return _dialogs;
            case 1:
                return _dialogs2;
            case 2:
                return _dialogs3;
        }
        return _dialogs;
    }
}

[Serializable]
public struct Dialog
{
    public string name;
    public string text;
    public Sprite character;
}