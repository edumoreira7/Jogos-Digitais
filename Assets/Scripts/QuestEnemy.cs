using UnityEngine;

public class QuestEnemy : MonoBehaviour
{
    public enum Tipo { Guaxinim, Atirador, Guardiao }
    public Tipo tipo;
    public float limiteEsquerdo;
    public float limiteDireito;
    public float velocidade = 1.4f;
    public int vidas = 1;
    public Sprite[] parado;
    public Sprite[] andar;
    public Sprite[] atacar;
    public Sprite[] dano;
    public Sprite[] morte;
    public GameObject prefabDisparo;
    private float direcao = 1f;
    private Rigidbody2D corpo;
    private bool derrotado;
    private SpriteRenderer visual;
    private Material materialVisual;
    private QuestPlayer jogador;
    private float ataqueInicio = -10f;
    private float proximoAtaque;
    private float danoAte;
    private float morteInicio;
    private bool disparou;

    void Awake()
    {
        corpo = GetComponent<Rigidbody2D>();
        visual = GetComponent<SpriteRenderer>();
        materialVisual = visual.material;
    }

    void Start()
    {
        jogador = FindAnyObjectByType<QuestPlayer>();
    }

    void Update()
    {
        Sprite[] quadros = andar;
        float instante = Time.time * 8f;
        if (derrotado)
        {
            quadros = morte;
            instante = Mathf.Min((Time.time - morteInicio) * 4f, morte.Length - 1);
        }
        else if (Time.time < danoAte)
        {
            quadros = dano;
            instante = (Time.time - danoAte + .3f) * 6f;
        }
        else if (Time.time < ataqueInicio + .8f)
        {
            quadros = atacar;
            instante = Mathf.Clamp01((Time.time - ataqueInicio) / .8f) * atacar.Length;
            if (!disparou && Time.time >= ataqueInicio + .45f)
            {
                disparou = true;
                if (tipo == Tipo.Atirador && prefabDisparo != null)
                {
                    Vector3 posicao = transform.position + new Vector3(direcao * .8f, .65f, 0f);
                    Instantiate(prefabDisparo, posicao, Quaternion.identity).GetComponent<QuestEnemyShot>().Inicializar(direcao);
                }
                else if (jogador != null && Mathf.Abs(jogador.transform.position.x - transform.position.x) < 1.6f
                    && Mathf.Abs(jogador.transform.position.y - transform.position.y) < 1.6f)
                    jogador.Ferir();
            }
        }
        else if (velocidade == 0f)
            quadros = parado;
        if (quadros != null && quadros.Length > 0)
        {
            visual.sprite = quadros[Mathf.Clamp(Mathf.FloorToInt(instante), 0, int.MaxValue) % quadros.Length];
            materialVisual.mainTexture = visual.sprite.texture;
        }
        visual.flipX = direcao < 0;
    }

    void FixedUpdate()
    {
        if (derrotado || Time.time < danoAte || Time.time < ataqueInicio + .8f)
            return;
        float alcance = tipo == Tipo.Atirador ? 7f : 1.25f;
        if (jogador != null && Time.time >= proximoAtaque
            && Mathf.Abs(jogador.transform.position.x - corpo.position.x) < alcance
            && Mathf.Abs(jogador.transform.position.y - corpo.position.y) < 1.6f)
        {
            direcao = jogador.transform.position.x < corpo.position.x ? -1f : 1f;
            ataqueInicio = Time.time;
            proximoAtaque = Time.time + (tipo == Tipo.Atirador ? 2.7f : 1.8f);
            disparou = false;
            return;
        }
        if (corpo.position.x >= limiteDireito)
            direcao = -1f;
        else if (corpo.position.x <= limiteEsquerdo)
            direcao = 1f;
        Vector2 posicao = corpo.position;
        posicao.x = Mathf.Clamp(posicao.x + direcao * velocidade * Time.fixedDeltaTime, limiteEsquerdo, limiteDireito);
        corpo.MovePosition(posicao);
    }

    void OnTriggerEnter2D(Collider2D outro)
    {
        ProjetilPoder magia = outro.GetComponent<ProjetilPoder>();
        if (magia != null && !derrotado)
        {
            Destroy(magia.gameObject);
            vidas--;
            danoAte = Time.time + .3f;
            if (vidas <= 0)
            {
                derrotado = true;
                morteInicio = Time.time;
                GetComponent<Collider2D>().enabled = false;
                QuestSession.Pontos += 50;
                Destroy(gameObject, .7f);
            }
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

    void OnDestroy()
    {
        if (materialVisual != null)
            Destroy(materialVisual);
    }
}
