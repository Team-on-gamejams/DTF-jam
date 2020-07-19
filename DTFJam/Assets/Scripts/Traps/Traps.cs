using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traps : MonoBehaviour
{
    [SerializeField] private float _activationDelay = .5f;
    [SerializeField] private float _duration = 2f;
    private float _curDelay;
    private float _curDuration;
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private AudioClip _prepareClip;
    [SerializeField] private AudioClip _activateClip;
    [SerializeField] private AudioClip _hitClip;

    [SerializeField] private Animator _anim;
    private Health _health;
    private bool isActive;
    private int isActiveParamID;

    private void Start()
    {
        isActiveParamID = Animator.StringToHash("isActive");
    }

    // Update is called once per frame
    void Update()
    {
        if(_curDelay != 0 && _curDelay <= Time.time)
        {
            isActive = true;
            _anim.SetBool(isActiveParamID, isActive);
            AudioManager.Instance.Play(_activateClip, channel: AudioManager.AudioChannel.Sound);
            _curDelay = 0f;
            _curDuration = _duration + Time.time;
        }

        if(isActive && _health != null)
        {
            AudioManager.Instance.Play(_hitClip, channel: AudioManager.AudioChannel.Sound);
            _health.GetHit();
            _health = null;
        }

        if(isActive && _curDuration <= Time.time)
        {
            isActive = false;
            _anim.SetBool(isActiveParamID, isActive);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == Mathf.Log(_targetLayer, 2))
        {
            if (isActive == false)
            {
                _curDelay = _activationDelay + Time.time;
                AudioManager.Instance.Play(_prepareClip, channel: AudioManager.AudioChannel.Sound);
                _health = other.GetComponent<Health>();

                if (_health == null)
                    _health = other.GetComponent<HealthPass>()?.mainHealth;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == Mathf.Log(_targetLayer, 2))
        {
            _health = null;
        }
    }
}
