using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamageOnTriggerEnter : MonoBehaviour {
	[SerializeField] bool isPlayerWeapon;

	private void OnTriggerEnter(Collider other) {
		Health otherHealth = other.GetComponent<Health>();
		if(otherHealth == null) {
			otherHealth = other.GetComponent<HealthPass>()?.mainHealth;
		}

		if (otherHealth != null && !other.isTrigger) {
			if (isPlayerWeapon) {
				otherHealth.isUnderPlayerAttack = true;
			}

			otherHealth.GetHit();
		}

		Debug.Log($"Attack {other.transform.name}");
	}
}
