using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnDie : MonoBehaviour {
	[SerializeField] private float _timeToDestroy = 2f;

	void Die() {
		Destroy(gameObject, _timeToDestroy);
	}
}
