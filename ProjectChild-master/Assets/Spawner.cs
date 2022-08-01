using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public bool showGizmos = true;
    public LayerMask mask = -1;
    public float maxHeightForSpawner = 100f;

    public GameObject spawnPoint;
    public GameObject player;
    public float spawnRadius = 200f;
    public float minimumDistanceBtwPoints = 50f;
    public List<Vector2> spawnPoints = new List<Vector2>(); //We want to take the x and z pos

    [Header("Multi Spawning Logic")]
    public int startPoints = 8;
    public int currentPoints;
    public int minPointForSpawn = 5;
    public float intervalBtwChecks = 15f;

    private void Start()
    {
        currentPoints = startPoints;
        MultiSpawn();
        StartCoroutine(IncreasePoints());
    }

    IEnumerator IncreasePoints()
    {
        yield return new WaitForSeconds(4f);
        currentPoints++;
        StartCoroutine(IncreasePoints());
    }

    IEnumerator CheckForMultiSpawn()
    {
        yield return new WaitForSeconds(intervalBtwChecks);
        MultiSpawn();
    }

    void MultiSpawn()
    {
        if (currentPoints >= minPointForSpawn)
        {
            int index = Random.Range(1, currentPoints);
            for (int i = 0; i < index; i++)
            {
                Spawn();
            }
            currentPoints -= index;
        }else{
            return;
        }
        StartCoroutine(CheckForMultiSpawn());
    }

    void Spawn()
    {
        float randomX = Random.Range(player.transform.position.x, player.transform.position.x + spawnRadius);
        float randomZ = Random.Range(player.transform.position.z, player.transform.position.z + spawnRadius);

        if (Vector2.Distance(new Vector2(player.transform.position.x, player.transform.position.z), new Vector2(randomX, randomZ)) >= minimumDistanceBtwPoints)
        {
            foreach (Vector2 v in spawnPoints)
            {
                if (Vector2.Distance(v, new Vector2(randomX, randomZ)) < minimumDistanceBtwPoints)
                {
                    Spawn(); return;
                }
            }
            GameObject spawn = Instantiate(spawnPoint, new Vector3(randomX, 0, randomZ), Quaternion.identity);
            PutObjectAboveGround(spawn);
            spawnPoints.Add(new Vector2(randomX, randomZ));
        }
    }

    void PutObjectAboveGround(GameObject target)
    {
        float radius;
        if (target.GetComponent<Collider>() != null)
        {
            radius = target.GetComponent<Collider>().bounds.extents.y;
        }
        else
        {
            radius = 1f;
        }

        RaycastHit hit;
        Ray ray = new Ray(target.transform.position + Vector3.up * maxHeightForSpawner, Vector3.down);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
        {
            Debug.Log(hit.point);
            if (hit.collider != null)
            {
                target.transform.position = new Vector3(target.transform.position.x, hit.point.y + radius, target.transform.position.z);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (showGizmos != true) return;
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawSphere(player.transform.position, spawnRadius);
    }
}
