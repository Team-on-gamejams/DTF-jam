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

    [Space]
    [SerializeField] private Image _characterImage = null;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private TextMeshProUGUI _textName;
    private int _curDialogNumber;
    Transform _imageTransform;

    [Header("Parameters")]
    [SerializeField] private int _textDelay = 5;
    [SerializeField] private float _slideDuration = .5f;

    private CanvasGroup canvasGroup;
    bool _switchDialog = false;
    bool _isClosing = false;
    bool isShowed = false;

    Action onEndDialogue;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        _imageTransform = _characterImage.transform;
    }

    private void Update()
    {
        if (_isClosing)
        {
            canvasGroup.alpha -= Time.deltaTime * 2f;

            if (canvasGroup.alpha <= 0)
                _isClosing = false;
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
        isShowed = true;
        this.onEndDialogue = onEndDialogue;
        StartCoroutine(MainCoroutine());
    }

    private IEnumerator MainCoroutine()
    {
        for (_curDialogNumber = 0; _curDialogNumber < _dialogs.Count; _curDialogNumber++)
        {
            _switchDialog = false;
            _text.maxVisibleCharacters = 1;
            _characterImage.sprite = _dialogs[_curDialogNumber].character;
            _text.text = _dialogs[_curDialogNumber].text;

            int delayCounter = 0;

            while (_switchDialog == false)
            {
                if (delayCounter >= _textDelay && _text.maxVisibleCharacters < _dialogs[_curDialogNumber].text.Length)
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

        onEndDialogue?.Invoke();
        onEndDialogue = null;
    }

    private bool Next()
    {
        if (_curDialogNumber < _dialogs.Count && _text.maxVisibleCharacters < _dialogs[_curDialogNumber].text.Length)
        {
            _text.maxVisibleCharacters = _dialogs[_curDialogNumber].text.Length;
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