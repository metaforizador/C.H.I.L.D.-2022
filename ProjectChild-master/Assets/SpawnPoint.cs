using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public Enemy enemy;
    private void Start() {
        enemy = GetComponentInChildren<Enemy>(true);
    }
    //Will be used in animator
    public void DestroySpawnerGO()
    {
        enemy.transform.parent = null;
        Destroy(this.gameObject, 0.3f);
    }
}
