using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float speed = 10.0f;             // Define a velocidade da raquete
    public float boundY = -4.5f;            // Define os limites em Y
    public float boundX = 2.65f;
    public float boundCenter = 0.2f;
    private Rigidbody2D rb2d;               // Define o corpo rigido 2D que representa a raquete


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();     // Inicializa a raquete
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPos = transform.position;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3 dir = mousePos - playerPos;
        dir.Normalize();

        Vector3 speedVec = dir * speed;

        var vel = rb2d.linearVelocity;
        vel.x = speedVec.x;
        vel.y = speedVec.y;
        rb2d.linearVelocity = vel; 

        var pos = transform.position;           // Acessa a Posição da raquete
        if (pos.y < boundY) {                  
            pos.y = boundY;                     // Corrige a posicao da raquete caso ele ultrapasse o limite superior
        }
        else if (pos.y > boundCenter) {
            pos.y = boundCenter;                    // Corrige a posicao da raquete caso ele ultrapasse o limite inferior
        }

        
        if (pos.x > boundX) {                  
            pos.x = boundX;                     // Corrige a posicao da raquete caso ele ultrapasse o limite superior
        }
        else if (pos.x < -boundX) {
            pos.x = -boundX;                    // Corrige a posicao da raquete caso ele ultrapasse o limite inferior
        }
        transform.position = pos;               // Atualiza a posição da raquete

    }
}
