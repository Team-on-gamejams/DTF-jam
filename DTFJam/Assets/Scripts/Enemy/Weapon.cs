using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum AttackType { Melee, Range }

public class Weapon : MonoBehaviour
{
    public AttackType attackType;
    public float attackRange = 2f;
    public float attackCD = 1f;

    [Header("Range Properties")]
    [SerializeField] private int _bulletNumber = 2;
    [SerializeField] private float _delayBtwShots = .2f;
    [SerializeField] private float _bulletSpeed = 10f;
    [SerializeField] private GameObject _bulletPrefab = null;
    [SerializeField] private Transform _barrelTransform = null;

    public IEnumerator Shoot()
    {
        for (int i = 0; i < _bulletNumber; i++)
        {
            Vector3 rot = transform.rotation.eulerAngles;
            rot.z += _bulletPrefab.transform.rotation.eulerAngles.z;
            
            Bullet bullet = Instantiate(_bulletPrefab, _barrelTransform.position, Quaternion.Euler(rot)).GetComponent<Bullet>();
            bullet.speed = _bulletSpeed;

            yield return new WaitForSeconds(_delayBtwShots);
        }
    }
}