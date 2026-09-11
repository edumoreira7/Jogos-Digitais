using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    public GUISkin scoreSkin;

    void OnGUI()
    {
        if (scoreSkin != null)
            GUI.skin = scoreSkin;

        GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
        titleStyle.fontSize = 40;
        titleStyle.alignment = TextAnchor.MiddleCenter;

        GUIStyle infoStyle = new GUIStyle(GUI.skin.label);
        infoStyle.fontSize = 30;
        infoStyle.alignment = TextAnchor.MiddleCenter;

        if (GameManager.PlayerWon)
        {
            GUI.Label(
                new Rect(
                    Screen.width / 2 - 250,
                    Screen.height / 2 - 180,
                    500,
                    100
                ),
                "VOCÊ VENCEU!",
                titleStyle
            );
        }
        else
        {
            GUI.Label(
                new Rect(
                    Screen.width / 2 - 250,
                    Screen.height / 2 - 180,
                    500,
                    100
                ),
                "GAME OVER",
                titleStyle
            );
        }

        GUI.Label(
            new Rect(
                Screen.width / 2 - 200,
                Screen.height / 2 - 80,
                400,
                80
            ),
            "SCORE: " + GameManager.Score,
            infoStyle
        );

        if (GUI.Button(
            new Rect(
                Screen.width / 2 - 120,
                Screen.height / 2 + 40,
                240,
                70
            ),
            "JOGAR NOVAMENTE"))
        {
            GameManager.RestartGame();
        }

        if (GUI.Button(
            new Rect(
                Screen.width / 2 - 120,
                Screen.height / 2 + 130,
                240,
                70
            ),
            "MENU"))
        {
            GameManager.Score = 0;
            GameManager.Lives = 3;
            GameManager.Level = 1;

            GameManager.GameEnded = false;
            GameManager.PlayerWon = false;

            SceneManager.LoadScene("Menu");
        }
    }
}