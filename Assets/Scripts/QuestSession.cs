using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestSession : MonoBehaviour
{
    public int fase;
    public int totalItens;
    public string nomeFase;
    public static int Vidas = 3;
    public static int Pontos;
    public static bool Venceu;
    public static QuestSession Atual;
    public int Coletados { get; private set; }
    public bool Pausado { get; private set; }
    private string aviso;
    private float avisoAte;
    private GUIStyle titulo;
    private GUIStyle texto;
    private GUIStyle pequeno;
    private GUIStyle botao;

    void Awake()
    {
        Atual = this;
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (fase > 0 && fase < 3 && Input.GetKeyDown(KeyCode.Escape))
        {
            Pausado = !Pausado;
            Time.timeScale = Pausado ? 0f : 1f;
        }
        if (fase == 0 && Input.GetKeyDown(KeyCode.Return))
            NovoJogo();
        if (fase == 3 && Input.GetKeyDown(KeyCode.Return))
            NovoJogo();
    }

    public static void NovoJogo()
    {
        Vidas = 3;
        Pontos = 0;
        Venceu = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("Fase1");
    }

    public void Coletar()
    {
        Coletados++;
        Pontos += 100;
        if (Coletados == totalItens)
            Avisar("Todos os cristais! Siga ate o portal.");
    }

    public void Avisar(string mensagem)
    {
        aviso = mensagem;
        avisoAte = Time.unscaledTime + 3f;
    }

    public void Portal()
    {
        if (Coletados < totalItens)
        {
            Avisar("Faltam " + (totalItens - Coletados) + " cristais. Explore as plataformas!");
            return;
        }
        if (fase == 1)
            SceneManager.LoadScene("Fase2");
        else
        {
            Venceu = true;
            SceneManager.LoadScene("Final");
        }
    }

    public void PerderVida()
    {
        Vidas = Mathf.Max(0, Vidas - 1);
        if (Vidas == 0)
        {
            Venceu = false;
            SceneManager.LoadScene("Final");
        }
    }

    void Estilos()
    {
        if (titulo != null)
            return;
        titulo = new GUIStyle(GUI.skin.label) { fontSize = 54, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        titulo.normal.textColor = new Color(1f, .91f, .62f);
        texto = new GUIStyle(GUI.skin.label) { fontSize = 24, alignment = TextAnchor.MiddleCenter, wordWrap = true };
        texto.normal.textColor = Color.white;
        pequeno = new GUIStyle(texto) { fontSize = 18 };
        botao = new GUIStyle(GUI.skin.button) { fontSize = 24, fontStyle = FontStyle.Bold };
        botao.normal.textColor = new Color(.08f, .16f, .23f);
    }

    void Painel(Rect area)
    {
        Color anterior = GUI.color;
        GUI.color = new Color(.025f, .07f, .14f, .93f);
        GUI.DrawTexture(area, Texture2D.whiteTexture);
        GUI.color = anterior;
    }

    void OnGUI()
    {
        Estilos();
        Matrix4x4 matrizAnterior = GUI.matrix;
        float escala = Mathf.Min(Screen.width / 1280f, Screen.height / 720f);
        GUI.matrix = Matrix4x4.TRS(new Vector3((Screen.width - 1280 * escala) / 2, (Screen.height - 720 * escala) / 2), Quaternion.identity, Vector3.one * escala);
        if (fase == 0)
        {
            Painel(new Rect(220, 64, 840, 592));
            GUI.Label(new Rect(280, 90, 720, 75), "ELEMENTAL QUEST", titulo);
            GUI.Label(new Rect(280, 169, 720, 36), "O caminho dos cristais", texto);
            GUI.Label(new Rect(300, 232, 680, 140), "A / D ou setas: andar     |     Shift: correr\nEspaco: pular     |     J: lancar magia\nEsc: pausar", texto);
            GUI.Label(new Rect(310, 388, 660, 90), "Colete TODOS os cristais de cada fase e entre no portal.\nEvite os slimes e os abismos. Voce tem 3 vidas para as duas fases.", pequeno);
            if (GUI.Button(new Rect(465, 513, 350, 65), "JOGAR  /  ENTER", botao))
                NovoJogo();
            GUI.Label(new Rect(340, 601, 600, 30), "01  Jardim das Nuvens     /     02  Ruinas Esmeralda", pequeno);
        }
        else if (fase == 3)
        {
            Painel(new Rect(250, 140, 780, 450));
            GUI.Label(new Rect(280, 175, 720, 80), Venceu ? "VITORIA!" : "FIM DE JOGO", titulo);
            GUI.Label(new Rect(320, 275, 640, 80), Venceu ? "Todos os cristais foram recuperados!\nO vale esta seguro outra vez." : "As vidas acabaram.\nO vale espera pela sua proxima aventura.", texto);
            GUI.Label(new Rect(400, 375, 480, 40), "PONTUACAO   " + Pontos.ToString("00000"), texto);
            if (GUI.Button(new Rect(390, 452, 270, 65), "REINICIAR", botao))
                NovoJogo();
            if (GUI.Button(new Rect(680, 452, 210, 65), "INICIO", botao))
                SceneManager.LoadScene("Inicio");
        }
        else
        {
            Painel(new Rect(20, 18, 1240, 70));
            GUI.Label(new Rect(35, 27, 395, 50), "0" + fase + "  /  " + nomeFase, pequeno);
            GUI.Label(new Rect(455, 27, 190, 50), "VIDAS  " + Vidas + " / 3", texto);
            GUI.Label(new Rect(675, 27, 260, 50), "CRISTAIS  " + Coletados + " / " + totalItens, texto);
            GUI.Label(new Rect(960, 27, 275, 50), "PONTOS  " + Pontos.ToString("00000"), texto);
            if (Time.unscaledTime < avisoAte)
            {
                Painel(new Rect(260, 110, 760, 56));
                GUI.Label(new Rect(270, 117, 740, 40), aviso, pequeno);
            }
            if (Pausado)
            {
                Painel(new Rect(400, 240, 480, 255));
                GUI.Label(new Rect(430, 265, 420, 70), "PAUSA", titulo);
                if (GUI.Button(new Rect(460, 365, 360, 65), "CONTINUAR", botao))
                {
                    Pausado = false;
                    Time.timeScale = 1f;
                }
            }
        }
        GUI.matrix = matrizAnterior;
    }
}
