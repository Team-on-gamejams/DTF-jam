using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Bullet : MonoBehaviour
{
    public float speed;
    public Vector3 playerPosition;
    private Rigidbody _rigidBody;
    private float _lifetime = 4f;
    private float _curLifetime;

    private void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();

        playerPosition.y = 0f;
        Vector3 myPosition = transform.position;
        myPosition.y = 0f;

        Vector3 targetDirection = (playerPosition - myPosition).normalized;
        transform.rotation = Quaternion.LookRotation(targetDirection);
        transform.up = transform.forward;
        _rigidBody.velocity = targetDirection * speed;
        //_rigidBody.AddForce(transform.up * speed, ForceMode.Impulse);

        _curLifetime = _lifetime + Time.time;
    }

    private void Update()
    {
        if (_curLifetime <= Time.time)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9 || other.gameObject.layer == 8)
        {
            Health otherHealth = other.GetComponent<Health>();

            if (otherHealth == null)
                otherHealth = other.GetComponent<HealthPass>()?.mainHealth;

            if (otherHealth != null)
                otherHealth.GetHit();

            Destroy(gameObject);
        }
    }
}