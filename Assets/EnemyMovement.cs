using System.Collections;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float stepX = 1f;
    public float stepDown = 5f;
    public float stepDelay = 0.5f;

    public GameObject bulletPrefab;

    public float minShootTime = 2f;
    public float maxShootTime = 5f;

    void Start()
    {
        ApplyDifficulty();

        StartCoroutine(MoveEnemies());
        StartCoroutine(ShootRandomly());
    }

    void ApplyDifficulty()
    {
        if (GameManager.Level == 1)
        {
            stepDelay = 0.5f;

            minShootTime = 2f;
            maxShootTime = 5f;
        }

        if (GameManager.Level == 2)
        {
            stepDelay = 0.3f;

            minShootTime = 1f;
            maxShootTime = 3f;
        }
    }
    IEnumerator MoveEnemies()
    {
        while (true)
        {
            // 10 passos para direita
            for (int i = 0; i < 10; i++)
            {
                transform.position += Vector3.right * stepX;
                yield return new WaitForSeconds(stepDelay);
            }

            // 10 passos para esquerda
            for (int i = 0; i < 10; i++)
            {
                transform.position += Vector3.left * stepX;
                yield return new WaitForSeconds(stepDelay);
            }

            // desce
            transform.position += Vector3.down * stepDown;

            yield return new WaitForSeconds(stepDelay);
        }
    }

    IEnumerator ShootRandomly()
    {
        while (true)
        {
            float waitTime = Random.Range(minShootTime, maxShootTime);

            yield return new WaitForSeconds(waitTime);

            if (transform.childCount > 0)
            {
                int randomAlien = Random.Range(0, transform.childCount);

                Transform alien = transform.GetChild(randomAlien);

                Instantiate(
                    bulletPrefab,
                    alien.position,
                    Quaternion.identity
                );
            }
        }
    }
}