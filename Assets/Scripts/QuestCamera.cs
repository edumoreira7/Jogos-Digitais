using UnityEngine;

public class QuestCamera : MonoBehaviour
{
    public Transform alvo;
    public float minimoX = 0f;
    public float maximoX = 56f;
    private float suavizacao;

    void LateUpdate()
    {
        if (alvo == null)
            return;
        Vector3 posicao = transform.position;
        posicao.x = Mathf.SmoothDamp(posicao.x, Mathf.Clamp(alvo.position.x + 2f, minimoX, maximoX), ref suavizacao, .18f);
        transform.position = posicao;
    }
}
