using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float speed = 8f;

    void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.Die();
            }

            Destroy(gameObject);
        }

        if (collision.CompareTag("Boss"))
        {
            BossControl boss = collision.GetComponent<BossControl>();

            if (boss != null)
            {
                boss.Die();
            }

            Destroy(gameObject);
        }

        if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}