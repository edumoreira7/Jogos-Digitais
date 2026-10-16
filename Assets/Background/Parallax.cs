using UnityEngine;

public class Parallax : MonoBehaviour
{
    public float parallaxEffect;
    public Transform alvo;
    public Transform primeiraImagem;
    public int indiceImagem;

    private float largura;
    private float origemX;
    private float alvoInicialX;
    private float cameraInicialX;
    private Transform cameraTransform;

    void Start()
    {
        if (alvo == null)
        {
            PatoController pato = FindFirstObjectByType<PatoController>();
            if (pato != null)
                alvo = pato.transform;
        }

        if (alvo == null || primeiraImagem == null)
        {
            enabled = false;
            return;
        }

        largura = primeiraImagem.GetComponent<SpriteRenderer>().bounds.size.x;
        if (largura <= 0f)
        {
            enabled = false;
            return;
        }

        origemX = primeiraImagem.position.x;
        alvoInicialX = alvo.position.x;
        Camera cameraPrincipal = Camera.main;
        cameraTransform = cameraPrincipal != null ? cameraPrincipal.transform : null;
        cameraInicialX = cameraTransform != null ? cameraTransform.position.x : 0f;
    }

    void LateUpdate()
    {
        if (alvo == null)
            return;

        float deslocamentoPato = alvo.position.x - alvoInicialX;
        float deslocamentoCamera = cameraTransform != null
            ? cameraTransform.position.x - cameraInicialX
            : 0f;
        float deslocamentoImagem = Mathf.Repeat(
            indiceImagem * largura - deslocamentoPato * parallaxEffect + largura,
            largura * 2f
        ) - largura;

        Vector3 posicao = transform.position;
        posicao.x = origemX + deslocamentoCamera + deslocamentoImagem;
        transform.position = posicao;
    }
}
