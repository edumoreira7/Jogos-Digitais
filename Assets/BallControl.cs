using UnityEngine;

public class BallControl : MonoBehaviour
{
    private Rigidbody2D rb2d;               // Define o corpo rigido 2D que representa a bola
    public float maxSpeed = 5f;
    public AudioSource source;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>(); // Inicializa o objeto bola
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rb2d.linearVelocity.magnitude > maxSpeed)
        {
            rb2d.linearVelocity = rb2d.linearVelocity.normalized * maxSpeed;
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
{
    if (coll.collider.CompareTag("Player"))
        {
            Rigidbody2D playerRb = coll.collider.attachedRigidbody;

            if (playerRb != null)
            {
                rb2d.linearVelocity += playerRb.linearVelocity * 0.5f;
            }
        }

        source.Play();
    }
    // Reinicializa a posição e velocidade da bola
    void ResetBall(){
        rb2d.linearVelocity = Vector2.zero;
        transform.position = Vector2.zero;
    }

    // Reinicializa o jogo
    void RestartGame(){
        ResetBall();
    }

}
