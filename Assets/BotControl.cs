using UnityEngine;

public class BotControl : MonoBehaviour
{
    public float speed = 3.0f;
    public float attackSpeed = 4.0f;

    public float boundX = 2.65f;
    public float boundCenter = 0.2f;

    private GameObject theBall;
    private Rigidbody2D rb2d;

    private float directionX = 1f;

    private float defenseY = 4.4f;


    void Start()
    {
        theBall = GameObject.FindGameObjectWithTag("Ball");
        rb2d = GetComponent<Rigidbody2D>();
    }


    void FixedUpdate()
    {
        Vector2 pos = rb2d.position;
        Vector2 ballPos = theBall.transform.position;


        // ==========================================
        // BOLA NO CAMPO ADVERSÁRIO
        // BOT DEFENDE
        // ==========================================
        if (ballPos.y < 0)
        {
            pos.y = Mathf.MoveTowards(
                pos.y,
                defenseY,
                speed * Time.fixedDeltaTime
            );

            if (Mathf.Abs(pos.y - defenseY) < 0.05f)
            {
                pos.x += directionX * speed * Time.fixedDeltaTime;

                if (pos.x >= boundX)
                {
                    pos.x = boundX;
                    directionX = -1f;
                }
                else if (pos.x <= -boundX)
                {
                    pos.x = -boundX;
                    directionX = 1f;
                }
            }
        }


        // ==========================================
        // BOLA NO CAMPO DO BOT
        // BOT ATACA
        // ==========================================
        else
        {
            pos.x = Mathf.MoveTowards(
                pos.x,
                ballPos.x,
                attackSpeed * Time.fixedDeltaTime
            );

            pos.y = Mathf.MoveTowards(
                pos.y,
                ballPos.y,
                attackSpeed * Time.fixedDeltaTime
            );
        }


        // LIMITES X
        if (pos.x > boundX)
        {
            pos.x = boundX;
        }
        else if (pos.x < -boundX)
        {
            pos.x = -boundX;
        }


        // NÃO PASSA DO MEIO
        if (pos.y < boundCenter)
        {
            pos.y = boundCenter;
        }


        // Movimento físico
        rb2d.MovePosition(pos);
    }
}