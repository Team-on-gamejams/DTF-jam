using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public GameObject body;
    [SerializeField] GameObject shield;
    [SerializeField] Health health;
    [SerializeField] int generatorsCount = 4;

    private void Awake() {
        health.ForceSetHp(420);
    }

    public void OnGeneratorDestroy() {
        --generatorsCount;
        if(generatorsCount == 0) {
            shield.SetActive(false);
            health.ForceSetHp(1);
        }
    }
}
