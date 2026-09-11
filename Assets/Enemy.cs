using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int points = 10;

    public void Die()
    {
        GameManager.AddScore(points);
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BottomWall"))
        {
            GameManager.LoseLife();

            Destroy(gameObject);
        }
    }
}