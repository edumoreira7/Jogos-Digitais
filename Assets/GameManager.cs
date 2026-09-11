using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static int Score = 0;
    public static int Lives = 3;
    public static int Level = 1;

    public static bool GameEnded = false;
    public static bool PlayerWon = false;

    public GUISkin scoreSkin;

    private bool changingScene = false;

    void Update()
    {
        if (GameEnded)
            return;

        // DERROTA
        if (Lives <= 0)
        {
            GameOver();
            return;
        }

        // Verifica se todos os aliens comuns morreram
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies.Length == 0 && !changingScene)
        {
            NextLevel();
        }
    }

    public static void AddScore(int points)
    {
        Score += points;
    }

    public static void LoseLife()
    {
        if (GameEnded)
            return;

        Lives--;

        if (Lives < 0)
            Lives = 0;
    }

    void NextLevel()
    {
        changingScene = true;

        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.name == "Fase1")
        {
            Level = 2;
            SceneManager.LoadScene("Fase2");
        }
        else if (currentScene.name == "Fase2")
        {
            WinGame();
        }
    }

    void GameOver()
    {
        GameEnded = true;
        PlayerWon = false;

        SceneManager.LoadScene("Fim");
    }

    void WinGame()
    {
        GameEnded = true;
        PlayerWon = true;

        SceneManager.LoadScene("Fim");
    }

    public static void RestartGame()
    {
        Score = 0;
        Lives = 3;
        Level = 1;

        GameEnded = false;
        PlayerWon = false;

        SceneManager.LoadScene("Fase1");
    }

    void OnGUI()
    {
        if (scoreSkin != null)
            GUI.skin = scoreSkin;

        GUIStyle style = new GUIStyle(GUI.skin.label);

        style.fontSize = 28;
        style.alignment = TextAnchor.MiddleCenter;

        GUI.Label(
            new Rect(20, 20, 250, 60),
            "SCORE: " + Score,
            style
        );

        GUI.Label(
            new Rect(Screen.width - 270, 20, 250, 60),
            "LIVES: " + Lives,
            style
        );

        GUI.Label(
            new Rect(Screen.width / 2 - 100, 20, 200, 60),
            "LEVEL: " + Level,
            style
        );
    }
}