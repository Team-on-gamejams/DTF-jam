using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class DealDamageOnTriggerEnter : MonoBehaviour {
	static StringBuilder sb = new StringBuilder(64);

	[SerializeField] bool isPlayerWeapon;
	[SerializeField] LayerMask _targetLayer;

	[Header("Audio")]
	[Space]
	[SerializeField] AudioClip[] missClips;
	[SerializeField] AudioClip[] hitClips;

	[Header("Refs")]
	[Space]
	[SerializeField] Collider hitCollider;

	List<Health> hitted = new List<Health>(4);
	bool isAttacking = false;

	public void CheckHit() {
		if(isPlayerWeapon)
			hitCollider.enabled = true;
	}

	public void EnableCollider()
    {
		if (!isPlayerWeapon)
			hitCollider.enabled = true;
	}

	public void AttackStart() {
		isAttacking = true;

		sb.Clear();
		sb.Append($"Attack {hitted.Count} entities");

		foreach (var health in hitted) {
			DealHit(health);
			sb.Append($"\nAttack {health.transform.name}");
		}

		if(hitted.Count != 0) {
			AudioManager.Instance.Play(hitClips.Random(), channel: AudioManager.AudioChannel.Sound);
		}
		else {
			AudioManager.Instance.Play(missClips.Random(), channel: AudioManager.AudioChannel.Sound);
		}

		Debug.Log(sb.ToString());
	}

	public void AttackEnd() {
		hitCollider.enabled = false;
		isAttacking = false;

		if(isPlayerWeapon)
			hitted.Clear();
	}

	private void OnTriggerEnter(Collider other) {
		if (other.gameObject.layer == Mathf.Log(_targetLayer, 2))
		{
			Health otherHealth = other.GetComponent<Health>();
			if (otherHealth == null)
			{
				otherHealth = other.GetComponent<HealthPass>()?.mainHealth;
			}

			if (otherHealth != null && !other.isTrigger)
			{
				if((!isPlayerWeapon && hitted.Count <= 0) || isPlayerWeapon)
					hitted.Add(otherHealth);

				if (isAttacking)
				{
					DealHit(otherHealth);
				}
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == Mathf.Log(_targetLayer, 2))
		{
			if(!isPlayerWeapon)
				hitted.Clear();
		}
	}

    void DealHit(Health health) {
		if (isPlayerWeapon) {
			health.isUnderPlayerAttack = true;
		}

		health.GetHit();
	}
}
