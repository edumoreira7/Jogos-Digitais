using System.Collections;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public GameObject bossPrefab;
    public Transform spawnPoint;

    void Start()
    {
        StartCoroutine(SpawnBoss());
    }

    IEnumerator SpawnBoss()
    {
        while (true)
        {
            float waitTime = Random.Range(30f, 50f);

            yield return new WaitForSeconds(waitTime);

            Instantiate(
                bossPrefab,
                spawnPoint.position,
                Quaternion.identity
            );
        }
    }
}