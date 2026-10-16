using UnityEngine;

public class QuestCrystal : MonoBehaviour
{
    private Vector3 origem;
    private bool coletado;

    void Start()
    {
        origem = transform.position;
    }

    void Update()
    {
        transform.position = origem + Vector3.up * (Mathf.Sin(Time.time * 3f + origem.x) * .12f);
    }

    void OnTriggerEnter2D(Collider2D outro)
    {
        if (coletado || outro.GetComponent<QuestPlayer>() == null)
            return;
        coletado = true;
        QuestSession.Atual.Coletar();
        Destroy(gameObject);
    }
}
