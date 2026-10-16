using UnityEngine;

public class QuestPlayer : MonoBehaviour
{
    public Vector3 pontoSeguro;
    private float invulneravelAte;
    private SpriteRenderer visual;
    private Rigidbody2D corpo;

    void Awake()
    {
        visual = GetComponent<SpriteRenderer>();
        corpo = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        visual.color = Time.time < invulneravelAte && Mathf.FloorToInt(Time.time * 12) % 2 == 0
            ? new Color(1f, .45f, .45f, .4f) : Color.white;
        if (transform.position.y < -8f)
        {
            if (Time.time >= invulneravelAte)
                Ferir();
            transform.position = pontoSeguro;
            corpo.linearVelocity = Vector2.zero;
        }
    }

    public void Ferir()
    {
        if (Time.time < invulneravelAte || QuestSession.Atual == null)
            return;
        invulneravelAte = Time.time + 1.8f;
        QuestSession.Atual.PerderVida();
    }
}
