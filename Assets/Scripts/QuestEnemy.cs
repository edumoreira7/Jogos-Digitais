using UnityEngine;

public class QuestEnemy : MonoBehaviour
{
    public enum Tipo { Guaxinim, Atirador, Guardiao }
    public Tipo tipo;
    public float limiteEsquerdo;
    public float limiteDireito;
    public float velocidade = 1.4f;
    public float multiplicadorGuaxinim = 1.7f;
    public float alcancePerseguicao = 8f;
    public float velocidadePerseguicao = 2.2f;
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
    private bool movendo = true;
    private Collider2D colisor;
    private Collider2D colisorJogador;

    void Awake()
    {
        corpo = GetComponent<Rigidbody2D>();
        visual = GetComponent<SpriteRenderer>();
        materialVisual = visual.material;
        colisor = GetComponent<Collider2D>();
    }

    void Start()
    {
        jogador = FindAnyObjectByType<QuestPlayer>();
        if (jogador != null)
            colisorJogador = jogador.GetComponent<Collider2D>();
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
        else if (!movendo || velocidade == 0f)
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
        movendo = false;
        if (derrotado || Time.time < danoAte || Time.time < ataqueInicio + .8f)
            return;
        float alcance = tipo == Tipo.Atirador ? 7f : 1.25f;
        if (jogador != null && Time.time >= proximoAtaque
            && Mathf.Abs(jogador.transform.position.x - corpo.position.x) < alcance
            && Mathf.Abs(jogador.transform.position.y - corpo.position.y) < 1.6f
            && (tipo != Tipo.Guardiao || MesmoTerreno(out _)))
        {
            direcao = jogador.transform.position.x < corpo.position.x ? -1f : 1f;
            ataqueInicio = Time.time;
            proximoAtaque = Time.time + (tipo == Tipo.Atirador ? 2.7f : 1.8f);
            disparou = false;
            return;
        }
        if (PerseguirGuardiao())
            return;
        float velocidadeAtual = velocidade * (tipo == Tipo.Guaxinim ? multiplicadorGuaxinim : 1f);
        if (corpo.position.x < limiteEsquerdo || corpo.position.x > limiteDireito)
        {
            float destino = Mathf.Clamp(corpo.position.x, limiteEsquerdo, limiteDireito);
            direcao = destino > corpo.position.x ? 1f : -1f;
            Mover(Mathf.MoveTowards(corpo.position.x, destino, velocidadeAtual * Time.fixedDeltaTime));
            return;
        }
        if (corpo.position.x >= limiteDireito)
            direcao = -1f;
        else if (corpo.position.x <= limiteEsquerdo)
            direcao = 1f;
        float posicaoX = Mathf.Clamp(corpo.position.x + direcao * velocidadeAtual * Time.fixedDeltaTime, limiteEsquerdo, limiteDireito);
        Mover(posicaoX);
    }

    private bool MesmoTerreno(out Collider2D piso)
    {
        piso = Physics2D.Raycast(new Vector2(corpo.position.x, colisor.bounds.min.y + .3f), Vector2.down, .9f, 1 << 3).collider;
        if (jogador == null || colisorJogador == null || piso == null)
            return false;
        Collider2D pisoJogador = Physics2D.Raycast(new Vector2(jogador.transform.position.x, colisorJogador.bounds.min.y + .3f), Vector2.down, .9f, 1 << 3).collider;
        return piso == pisoJogador;
    }

    private bool PerseguirGuardiao()
    {
        if (tipo != Tipo.Guardiao || jogador == null
            || Mathf.Abs(jogador.transform.position.x - corpo.position.x) > alcancePerseguicao
            || !MesmoTerreno(out Collider2D piso))
            return false;
        float distancia = jogador.transform.position.x - corpo.position.x;
        direcao = distancia < 0 ? -1f : 1f;
        if (Mathf.Abs(distancia) <= 1.1f)
            return true;
        float margem = colisor.bounds.extents.x + .15f;
        float esquerdaSegura = piso.bounds.min.x + margem;
        float direitaSegura = piso.bounds.max.x - margem;
        if (esquerdaSegura >= direitaSegura)
            return true;
        float destino = Mathf.Clamp(jogador.transform.position.x, esquerdaSegura, direitaSegura);
        float proximoX = Mathf.MoveTowards(corpo.position.x, destino, velocidadePerseguicao * Time.fixedDeltaTime);
        Vector2 frente = new Vector2(proximoX + direcao * margem, colisor.bounds.min.y + .3f);
        if (Physics2D.Raycast(frente, Vector2.down, .9f, 1 << 3).collider == piso)
            Mover(proximoX);
        return true;
    }

    private void Mover(float posicaoX)
    {
        movendo = Mathf.Abs(posicaoX - corpo.position.x) > .0001f;
        corpo.MovePosition(new Vector2(posicaoX, corpo.position.y));
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
