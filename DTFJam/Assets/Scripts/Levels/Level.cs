using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Level : MonoBehaviour {
	[SerializeField] Transform playerSpawnPos = null;
	[SerializeField] GameObject[] props = null;

	[Header("Filled by script")]
	[Space]
	[SerializeField] List<GameObject> enemyPrefabs = null;
	[SerializeField] List<Transform> enemyPositions = null;

	Enemy[] enemies;

	public void InitLevel() {
		foreach (var p in props) {
			p.SetActive(true);
		}

		enemies = new Enemy[enemyPrefabs.Count];
		for (int i = 0; i < enemyPositions.Count; ++i) {
			GameObject go  = Instantiate(enemyPrefabs[i], enemyPositions[i].position, enemyPositions[i].rotation, transform);
			go.name = enemyPositions[i].name.Substring(0, enemyPositions[i].name.Length - 4);
			enemies[i] = go.GetComponent<Enemy>();
		}

		GameManager.Instance.player.OnEnterNewLevel(playerSpawnPos);

		GameManager.Instance.player.onRespawn += OnPlayerRespawn;
	}

	public void UnInitLevel() {
		foreach (var p in props) {
			p.SetActive(false);
		}

		for (int i = 0; i < enemyPositions.Count; ++i)
			if(enemies[i] != null)
				Destroy(enemies[i].gameObject);
		enemies = null;

		GameManager.Instance.player.onRespawn -= OnPlayerRespawn;
	}

	public void SetAsUnusedAtInit() {
		foreach (var p in props) {
			p.SetActive(false);
		}
	}

	public void OnPlayerRespawn() {
		for (int i = 0; i < enemyPositions.Count; ++i) {
			if(enemies[i] == null) {
				GameObject go = Instantiate(enemyPrefabs[i], enemyPositions[i].position, enemyPositions[i].rotation, transform);
				go.name = enemyPositions[i].name.Substring(0, enemyPositions[i].name.Length - 4);
				enemies[i] = go.GetComponent<Enemy>();
			}
			else {
				enemies[i].transform.position = enemyPositions[i].position;
				enemies[i].transform.rotation = enemyPositions[i].rotation;
			}
		}
	}

#if UNITY_EDITOR
	[Button("Enemies setup")]
	void SetupEnemies() {
		Enemy[] enemies = transform.GetComponentsInChildren<Enemy>();
		if(enemies.Length == 0) {
			for(int i = 0; i < enemyPositions.Count; ++i) {
				GameObject go = PrefabUtility.InstantiatePrefab(enemyPrefabs[i], transform) as GameObject;
				go.name = enemyPositions[i].name.Substring(0, enemyPositions[i].name.Length - 4);
				go.transform.position = enemyPositions[i].position;
				go.transform.rotation = enemyPositions[i].rotation;

				DestroyImmediate(enemyPositions[i].gameObject);
			}

			enemyPrefabs.Clear();
			enemyPositions.Clear();
		}
		else {
			foreach (var enemy in enemies) {
				GameObject parentObject = PrefabUtility.GetCorrespondingObjectFromSource(enemy.gameObject);
				string path = AssetDatabase.GetAssetPath(parentObject);

				enemyPrefabs.Add(parentObject);

				GameObject pos = new GameObject($"{enemy.name} pos");
				pos.transform.position = enemy.transform.position;
				pos.transform.rotation = enemy.transform.rotation;
				pos.transform.parent = transform;
				enemyPositions.Add(pos.transform);

				DestroyImmediate(enemy.gameObject);
			}
		}

		UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
	}
#endif
}
