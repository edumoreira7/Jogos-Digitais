using UnityEngine;

public class BotControl : MonoBehaviour
{
    public float speed = 3.0f;          // Velocidade defendendo
    public float attackSpeed = 4.0f;    // Velocidade atacando

    public float boundX = 2.65f;
    public float boundCenter = 0.2f;

    private GameObject theBall;

    private float directionX = 1f;

    // Posição onde o bot defende
    private float defenseY = 4.4f;

    void Start()
    {
        theBall = GameObject.FindGameObjectWithTag("Ball");
    }

    void Update()
    {
        Vector3 pos = transform.position;
        Vector3 ballPos = theBall.transform.position;


        // ==========================================
        // BOLA NO CAMPO ADVERSÁRIO
        // BOT VOLTA PARA O GOL E DEFENDE
        // ==========================================
        if (ballPos.y < 0)
        {
            // Primeiro volta para Y = 4.4
            pos.y = Mathf.MoveTowards(
                pos.y,
                defenseY,
                speed * Time.deltaTime
            );

            // Só começa a ir de um lado para o outro
            // quando já estiver perto do gol
            if (Mathf.Abs(pos.y - defenseY) < 0.05f)
            {
                pos.x += directionX * speed * Time.deltaTime;

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
        // BOT ATACA A BOLA
        // ==========================================
        else
        {
            pos.x = Mathf.MoveTowards(
                pos.x,
                ballPos.x,
                attackSpeed * Time.deltaTime
            );

            pos.y = Mathf.MoveTowards(
                pos.y,
                ballPos.y,
                attackSpeed * Time.deltaTime
            );
        }


        // ==========================================
        // LIMITES EM X
        // ==========================================
        if (pos.x > boundX)
        {
            pos.x = boundX;
        }
        else if (pos.x < -boundX)
        {
            pos.x = -boundX;
        }


        // ==========================================
        // BOT NÃO PASSA DO MEIO DO CAMPO
        // ==========================================
        if (pos.y < boundCenter)
        {
            pos.y = boundCenter;
        }


        transform.position = pos;
    }
}