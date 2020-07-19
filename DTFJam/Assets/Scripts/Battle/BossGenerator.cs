using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGenerator : MonoBehaviour {
	[SerializeField] Boss boss;
	[SerializeField] LineRenderer lr;
	[SerializeField] Transform sphere;

	private void Start() {
		if (boss != null)
			lr.SetPositions(new Vector3[] { sphere.transform.position, boss.transform.position });
		else
			lr.SetPositions(new Vector3[] { sphere.transform.position, Vector3.zero });
	}

	private void Update() {
		if (boss != null) {
			lr.SetPosition(1, boss.body.transform.position + Vector3.up + (sphere.position - boss.body.transform.position).normalized);
		}
	}

	void Die() {
		boss.OnGeneratorDestroy();
	}
}
