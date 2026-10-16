
using UnityEngine;

public class PatoController : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidadeAndar = 4f;
    public float velocidadeCorrer = 7f;
    public float forcaPulo = 10f;

    [Header("Chao")]
    public Transform groundCheck;
    public float raioChao = 0.15f;
    public LayerMask camadaChao;

    [Header("Poder")]
    public GameObject prefabPoder;
    public Transform pontoDisparo;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Collider2D colisorPato;

    private float movimento;
    private bool correndo;
    private bool atacando;
    private bool noChao;
    private bool olhandoDireita = true;

    private string animacaoAtual = "";

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        colisorPato = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (QuestSession.Atual != null && QuestSession.Atual.Pausado)
            return;

        noChao = Physics2D.OverlapCircle(
            groundCheck.position,
            raioChao,
            camadaChao
        ) != null;

        // Inicia o ataque.
        if (Input.GetKeyDown(KeyCode.J) && !atacando)
        {
            IniciarAtaque();
        }

        // Aguarda os Animation Events.
        if (atacando)
            return;

        // Movimento horizontal.
        movimento = Input.GetAxisRaw("Horizontal");
        correndo = Input.GetKey(KeyCode.LeftShift);

        if (movimento > 0)
            olhandoDireita = true;
        else if (movimento < 0)
            olhandoDireita = false;

        spriteRenderer.flipX = !olhandoDireita;

        // Pulo.
        if (Input.GetKeyDown(KeyCode.Space) && noChao)
        {
            rb.AddForce(
                Vector2.up * forcaPulo,
                ForceMode2D.Impulse
            );
        }

        // Animacoes.
        if (!noChao)
            TocarAnimacao("Jump");
        else if (Mathf.Abs(movimento) > 0.01f)
            TocarAnimacao(correndo ? "Run" : "Walk");
        else
            TocarAnimacao("Idle");
    }

    void FixedUpdate()
    {
        float velocidade = correndo
            ? velocidadeCorrer
            : velocidadeAndar;

        // O pato para horizontalmente ao atacar.
        float velocidadeX = atacando
            ? 0f
            : movimento * velocidade;

        rb.linearVelocity = new Vector2(
            velocidadeX,
            rb.linearVelocity.y
        );
    }

    public void IniciarAtaque()
    {
        if (atacando)
            return;
        atacando = true;
        movimento = 0;
        TocarAnimacao("Attack");
    }

    void TocarAnimacao(string nome)
    {
        if (animacaoAtual == nome)
            return;

        animator.Play(nome, 0, 0f);
        animacaoAtual = nome;
    }

    // Chamado por Animation Event.
    public void LancarPoder()
    {
        if (prefabPoder == null || pontoDisparo == null)
        {
            Debug.LogWarning(
                "Configure o Prefab e PontoDisparo!"
            );
            return;
        }

        float direcaoX = olhandoDireita ? 1f : -1f;

        Vector3 posicao = pontoDisparo.position;

        // Espelha a posicao em relacao ao pato.
        if (!olhandoDireita)
        {
            float distanciaX =
                pontoDisparo.position.x - transform.position.x;

            posicao.x = transform.position.x - distanciaX;
        }

        GameObject novoPoder = Instantiate(
            prefabPoder,
            posicao,
            Quaternion.identity
        );

        ProjetilPoder projetil =
            novoPoder.GetComponent<ProjetilPoder>();

        if (projetil != null)
        {
            projetil.Inicializar(
                new Vector2(direcaoX, 0f),
                colisorPato
            );
        }
    }

    // Chamado no fim da animacao Attack.
    public void FinalizarAtaque()
    {
        atacando = false;

        // Permite que o proximo estado seja tocado.
        animacaoAtual = "";

        TocarAnimacao(noChao ? "Idle" : "Jump");
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(
                groundCheck.position,
                raioChao
            );
        }
    }
}
