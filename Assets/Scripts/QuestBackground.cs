using UnityEngine;

[DefaultExecutionOrder(100)]
public class QuestBackground : MonoBehaviour
{
    public Transform alvo;
    public Transform cameraAlvo;
    public Transform[] paineis;
    public float fator = .2f;
    private float largura;
    private float inicioCamera;
    private float altura;

    void Start()
    {
        largura = paineis[0].GetComponent<SpriteRenderer>().bounds.size.x;
        inicioCamera = cameraAlvo.position.x;
        altura = paineis[0].position.y - cameraAlvo.position.y;
    }

    void LateUpdate()
    {
        float deslocamento = (cameraAlvo.position.x - inicioCamera) * fator;
        float origem = cameraAlvo.position.x;
        float ciclo = largura * paineis.Length;
        for (int indice = 0; indice < paineis.Length; indice++)
        {
            float distancia = Mathf.Repeat(indice * largura - deslocamento + largura, ciclo) - largura;
            paineis[indice].position = new Vector3(origem + distancia, cameraAlvo.position.y + altura, 5f);
        }
    }
}
