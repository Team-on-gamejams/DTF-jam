using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem.Controls;

public class DialogController : MonoBehaviour
{
    [HideInInspector] public UnityEvent dialogStart = new UnityEvent();
    [HideInInspector] public UnityEvent dialogEnd = new UnityEvent();
    [SerializeField] private List<Dialog> _dialogs = new List<Dialog>();
    private int _curDialogNumber;
    [SerializeField] private Image _characterImage = null;
    [SerializeField] private TextMeshProUGUI _text;
    Transform _imageTransform;

    [Header("Parameters")]
    [SerializeField] private int _textDelay = 5;
    private Vector2 _characterStartPosition;
    [SerializeField] private Vector2 _characterPosition;
    [SerializeField] private float _slideDuration = .5f;

    private CanvasGroup canvasGroup;
    bool _switchDialog = false;
    bool _isClosing = false;

    private void Awake()
    {
        //if (GameManager.isNewGame == false)
        //    Destroy(gameObject);

        canvasGroup = GetComponent<CanvasGroup>();
        _imageTransform = _characterImage.transform;
        _characterStartPosition = _imageTransform.localPosition;
        StartCoroutine(MainCoroutine());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            _switchDialog = Next();

        if (_isClosing)
        {
            canvasGroup.alpha -= Time.deltaTime * 2f;

            if (canvasGroup.alpha <= 0f)
            {
                dialogEnd.Invoke();
                Destroy(gameObject);
            }
        }
        else if (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime;
        }
    }

    private IEnumerator MainCoroutine()
    {
        dialogStart.Invoke();

        for (_curDialogNumber = 0; _curDialogNumber < _dialogs.Count; _curDialogNumber++)
        {
            _switchDialog = false;
            _text.maxVisibleCharacters = 1;
            _imageTransform.localPosition = _characterStartPosition;
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

                if(_imageTransform.localPosition.x > _characterPosition.x)
                {
                    float slideSpeed = Time.deltaTime / _slideDuration;
                    _imageTransform.localPosition = Vector3.Lerp(_imageTransform.localPosition, _characterPosition, slideSpeed);
                    
                }


                delayCounter++;

                yield return new WaitForEndOfFrame();
            }
        }

        _isClosing = true;
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

[System.Serializable]
public struct Dialog
{
    public string text;
    public Sprite character;
}