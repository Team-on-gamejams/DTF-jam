using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traps : MonoBehaviour
{
    [SerializeField] private float _activationDelay = .5f;
    [SerializeField] private float _duration = 2f;
    private float _curDelay;
    private float _curDuration;
    //[SerializeField] private AudioClip _prepareClip;
    [SerializeField] private AudioClip _activateClip;
    [SerializeField] private AudioClip _hitClip;

    [SerializeField] private Animator _anim;
    private List<Health> _health = new List<Health>();
    private bool isActive;
    private int isActiveParamID;
    [SerializeField] private GameObject _sparksParticles;
    private ParticleSystem _sparks;
    private Material _myMaterial;
    private Color _matColor;
    [ColorUsage(true, true)] [SerializeField] Color _activeColor;
    private Coroutine _colorCoroutine;

    private void Start()
    {
        isActiveParamID = Animator.StringToHash("isActive");
        _myMaterial = GetComponent<MeshRenderer>().material;
        _matColor = _myMaterial.color;
    }

    // Update is called once per frame
    void Update()
    {
        if(_curDelay != 0 && _curDelay <= Time.time)
        {
            isActive = true;
            _anim.SetBool(isActiveParamID, isActive);
            _sparks = Instantiate(_sparksParticles, transform).GetComponent<ParticleSystem>();
            Invoke("DestroyParticle", _sparks.main.duration);
            //AudioManager.Instance.Play(_activateClip, channel: AudioManager.AudioChannel.Sound);
            _curDelay = 0f;
            _curDuration = _duration + Time.time;

            if(_colorCoroutine != null)
                StopCoroutine(_colorCoroutine);

            _colorCoroutine = StartCoroutine(ChangeColor(_matColor, _duration, 1f));
        }

        if(isActive && _health.Count > 0)
        {
            AudioManager.Instance.Play(_hitClip, channel: AudioManager.AudioChannel.Sound);
            
            foreach(Health h in _health)
            {
                if(h != null)
                    h.GetHit();
            }

            _health.Clear();
        }

        if(isActive && _curDuration <= Time.time)
        {
            isActive = false;
            _anim.SetBool(isActiveParamID, isActive);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 9 || other.gameObject.layer == 10)
        {
            if (_curDelay <= 0f && other.gameObject.layer == 9)
            {
                _curDelay = _activationDelay + Time.time;
                //AudioManager.Instance.Play(_prepareClip, channel: AudioManager.AudioChannel.Sound);
                AudioManager.Instance.Play(_activateClip, channel: AudioManager.AudioChannel.Sound);

                if (_colorCoroutine != null)
                    StopCoroutine(_colorCoroutine);

                _colorCoroutine = StartCoroutine(ChangeColor(_activeColor, _activationDelay));
            }

            Health otherHealth = other.GetComponent<Health>();

            if (otherHealth == null)
            {
                otherHealth = other.GetComponent<HealthPass>()?.mainHealth;
                _health.Add(otherHealth);
            }
            else
                _health.Add(otherHealth);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 9 || other.gameObject.layer == 10)
        {
            Health otherHealth = other.GetComponent<Health>();

            if (otherHealth == null)
                otherHealth = other.GetComponent<HealthPass>()?.mainHealth;

            _health.Remove(otherHealth);
            _health.Clear();
        }
    }

    private void DestroyParticle()
    {
        _sparks.Stop();
    }

    private IEnumerator ChangeColor(Color color, float time, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);
        time -= delay;

        while (_myMaterial.color != color)
        {
            _myMaterial.color = Color.Lerp(_myMaterial.color, color, Time.deltaTime / time);
            time -= Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
    }
}
