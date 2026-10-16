using UnityEngine;

public class QuestEnemyShot : MonoBehaviour
{
    public void Inicializar(float direcao)
    {
        GetComponent<Rigidbody2D>().linearVelocity = new Vector2(direcao * 5f, 0f);
        GetComponent<SpriteRenderer>().flipX = direcao < 0;
        Destroy(gameObject, 3f);
    }

    void OnTriggerEnter2D(Collider2D outro)
    {
        QuestPlayer jogador = outro.GetComponent<QuestPlayer>();
        if (jogador != null)
        {
            jogador.Ferir();
            Destroy(gameObject);
        }
        else if (outro.gameObject.layer == 3 && !outro.isTrigger)
            Destroy(gameObject);
    }
}
