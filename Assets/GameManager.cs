using UnityEngine;

public class GameManager : MonoBehaviour
{        
    public static int PlayerScore1 = 0; // Pontuação do player 1
    public static int PlayerScore2 = 0; // Pontuação do player 2

    public GUISkin layout;              // Fonte do placar
    GameObject theBall;                 // Referência ao objeto bola

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        theBall = GameObject.FindGameObjectWithTag("Ball"); // Busca a referência da bola
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // incrementa a potuação
    public static void Score (string wallID) {
        if (wallID == "GolTop")
        {
            PlayerScore1++;
        } else if (wallID == "GolBottom")
        {
            PlayerScore2++;
        }
    }

    // Gerência da pontuação e fluxo do jogo
    void OnGUI()
    {
        GUI.skin = layout;

        // =========================
        // ESTILO DO PLACAR
        // =========================
        GUIStyle scoreStyle = new GUIStyle(GUI.skin.label);
        scoreStyle.fontSize = 50;
        scoreStyle.alignment = TextAnchor.MiddleCenter;
        scoreStyle.normal.textColor = Color.black;


        // PLAYER 2 - EM CIMA, DESLOCADO PARA ESQUERDA
        GUI.Label(
            new Rect(
                30,
                30,
                120,
                80
            ),
            PlayerScore2.ToString(),
            scoreStyle
        );


        // PLAYER 1 - EMBAIXO, DESLOCADO PARA DIREITA
        GUI.Label(
            new Rect(
                Screen.width - 150,
                Screen.height - 110,
                120,
                80
            ),
            PlayerScore1.ToString(),
            scoreStyle
        );


        // =========================
        // ESTILO DO RESTART
        // =========================
        GUIStyle restartStyle = new GUIStyle(GUI.skin.button);
        restartStyle.fontSize = 28;
        restartStyle.alignment = TextAnchor.MiddleCenter;

        restartStyle.normal.textColor = Color.black;
        restartStyle.hover.textColor = Color.black;
        restartStyle.active.textColor = Color.black;


        Matrix4x4 originalMatrix = GUI.matrix;


        // =========================
        // RESTART NA LATERAL DIREITA
        // =========================
        Vector2 pivot = new Vector2(
            Screen.width - 40,
            Screen.height / 2
        );

        GUIUtility.RotateAroundPivot(90, pivot);

        if (GUI.Button(
            new Rect(
                Screen.width - 120,
                Screen.height / 2 - 5,
                160,
                80
            ),
            "RESTART",
            restartStyle
        ))
        {
            PlayerScore1 = 0;
            PlayerScore2 = 0;

            theBall.SendMessage(
                "RestartGame",
                null,
                SendMessageOptions.RequireReceiver
            );
        }

        GUI.matrix = originalMatrix;


        // =========================
        // PLAYER 1 VENCEU
        // =========================
        if (PlayerScore1 >= 10)
        {
            GUIStyle winStyle = new GUIStyle(GUI.skin.label);
            winStyle.fontSize = 35;
            winStyle.alignment = TextAnchor.MiddleCenter;
            winStyle.normal.textColor = Color.black;

            GUI.Label(
                new Rect(
                    Screen.width / 2 - 200,
                    Screen.height / 2 - 200,
                    400,
                    100
                ),
                "PLAYER ONE WINS",
                winStyle
            );

            theBall.SendMessage(
                "ResetBall",
                null,
                SendMessageOptions.RequireReceiver
            );
        }
        else if (PlayerScore2 >= 10)
        {
            GUIStyle winStyle = new GUIStyle(GUI.skin.label);
            winStyle.fontSize = 35;
            winStyle.alignment = TextAnchor.MiddleCenter;
            winStyle.normal.textColor = Color.black;

            GUI.Label(
                new Rect(
                    Screen.width / 2 - 200,
                    Screen.height / 2 - 200,
                    400,
                    100
                ),
                "PLAYER TWO WINS",
                winStyle
            );

            theBall.SendMessage(
                "ResetBall",
                null,
                SendMessageOptions.RequireReceiver
            );
        }
    }

}
