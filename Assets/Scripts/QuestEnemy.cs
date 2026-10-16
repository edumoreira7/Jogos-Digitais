using UnityEngine;

public class QuestEnemy : MonoBehaviour
{
    public float limiteEsquerdo;
    public float limiteDireito;
    public float velocidade = 1.4f;
    private float direcao = 1f;
    private Rigidbody2D corpo;
    private bool derrotado;

    void Awake()
    {
        corpo = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (corpo.position.x >= limiteDireito)
            direcao = -1f;
        else if (corpo.position.x <= limiteEsquerdo)
            direcao = 1f;
        Vector2 posicao = corpo.position;
        posicao.x = Mathf.Clamp(posicao.x + direcao * velocidade * Time.fixedDeltaTime, limiteEsquerdo, limiteDireito);
        corpo.MovePosition(posicao);
        GetComponent<SpriteRenderer>().flipX = direcao < 0;
    }

    void OnTriggerEnter2D(Collider2D outro)
    {
        ProjetilPoder magia = outro.GetComponent<ProjetilPoder>();
        if (magia != null && !derrotado)
        {
            derrotado = true;
            QuestSession.Pontos += 50;
            Destroy(magia.gameObject);
            Destroy(gameObject);
            return;
        }
        TocarJogador(outro);
    }

    void OnTriggerStay2D(Collider2D outro)
    {
        TocarJogador(outro);
    }

    void TocarJogador(Collider2D outro)
    {
        QuestPlayer jogador = outro.GetComponent<QuestPlayer>();
        if (!derrotado && jogador != null)
            jogador.Ferir();
    }
}
