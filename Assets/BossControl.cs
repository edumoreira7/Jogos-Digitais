using UnityEngine;

public class BossControl : MonoBehaviour
{
    public float speed = 5f;
    public float maxX = 9f;

    void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;

        if (transform.position.x >= maxX)
        {
            Destroy(gameObject);
        }
    }

    public void Die()
    {
        GameManager.AddScore(50);

        Destroy(gameObject);
    }
}