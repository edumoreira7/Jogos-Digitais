#if UNITY_EDITOR
using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestSmokeTest : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Iniciar()
    {
        if (!Environment.GetCommandLineArgs().Contains("-questTest"))
            return;
        GameObject objeto = new GameObject("Verificacao automatica da aventura");
        DontDestroyOnLoad(objeto);
        objeto.AddComponent<QuestSmokeTest>();
    }

    private IEnumerator Start()
    {
        Application.logMessageReceived += Erro;
        QuestSession.NovoJogo();
        yield return new WaitForSeconds(1f);
        QuestPlayer jogador = FindFirstObjectByType<QuestPlayer>();
        Exigir(jogador != null, "Jogador presente");
        Exigir(Mathf.Abs(jogador.GetComponent<Collider2D>().bounds.min.y + 3f) < .15f, "Colisao com o chao");
        Capturar("Fase1-previa.png");
        QuestSession.Atual.Portal();
        Exigir(SceneManager.GetActiveScene().name == "Fase1", "Portal bloqueia coleta incompleta");
        jogador.Ferir();
        jogador.Ferir();
        Exigir(QuestSession.Vidas == 2, "Dano e invulnerabilidade");
        yield return new WaitForSeconds(1.9f);
        jogador.transform.position = new Vector3(-5f, -9f, 0f);
        yield return null;
        yield return null;
        Exigir(QuestSession.Vidas == 1 && jogador.transform.position.y > -8f, "Queda remove vida e reposiciona");
        QuestSession.Vidas = 3;
        PatoController controle = jogador.GetComponent<PatoController>();
        Rigidbody2D corpo = jogador.GetComponent<Rigidbody2D>();
        controle.enabled = false;
        float posicaoInicial = corpo.position.x;
        corpo.linearVelocity = new Vector2(4f, 0);
        yield return new WaitForSeconds(.3f);
        Exigir(corpo.position.x > posicaoInicial + .5f, "Movimento fisico horizontal");
        corpo.linearVelocity = Vector2.zero;
        float alturaInicial = corpo.position.y;
        corpo.AddForce(Vector2.up * controle.forcaPulo, ForceMode2D.Impulse);
        yield return new WaitForSeconds(.25f);
        Exigir(corpo.position.y > alturaInicial + .8f, "Pulo fisico");
        corpo.position = new Vector2(-5f, -2.3f);
        corpo.linearVelocity = Vector2.zero;
        controle.enabled = true;
        controle.IniciarAtaque();
        yield return new WaitForSeconds(.45f);
        Exigir(FindObjectsByType<ProjetilPoder>(FindObjectsSortMode.None).Length > 0, "Evento de ataque cria projetil");
        yield return new WaitForSeconds(.2f);
        QuestEnemy inimigo = FindFirstObjectByType<QuestEnemy>();
        int pontosAntes = QuestSession.Pontos;
        GameObject magia = Instantiate(controle.prefabPoder, inimigo.transform.position, Quaternion.identity);
        magia.GetComponent<ProjetilPoder>().Inicializar(Vector2.right, jogador.GetComponent<Collider2D>());
        yield return new WaitForSeconds(.1f);
        Exigir(QuestSession.Pontos == pontosAntes + 50, "Magia derrota inimigo e soma pontos");
        for (int numero = 1; numero <= 2; numero++)
        {
            if (numero == 2)
                Capturar("Fase2-previa.png");
            jogador = FindFirstObjectByType<QuestPlayer>();
            corpo = jogador.GetComponent<Rigidbody2D>();
            corpo.gravityScale = 0;
            jogador.GetComponent<PatoController>().enabled = false;
            foreach (QuestEnemy slime in FindObjectsByType<QuestEnemy>(FindObjectsSortMode.None))
                slime.gameObject.SetActive(false);
            QuestCrystal[] itens = FindObjectsByType<QuestCrystal>(FindObjectsSortMode.None);
            foreach (QuestCrystal item in itens)
            {
                corpo.position = item.transform.position;
                corpo.linearVelocity = Vector2.zero;
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
                yield return null;
            }
            Exigir(QuestSession.Atual.Coletados == QuestSession.Atual.totalItens, "Coleta fisica completa da fase " + numero);
            QuestSession.Atual.Portal();
            yield return new WaitForSeconds(.2f);
        }
        Exigir(SceneManager.GetActiveScene().name == "Final" && QuestSession.Venceu, "Vitoria apos duas fases");
        Exigir(QuestSession.Pontos >= 3600, "Pontuacao dos 36 cristais");
        QuestSession.NovoJogo();
        yield return new WaitForSeconds(.2f);
        Exigir(QuestSession.Vidas == 3 && QuestSession.Pontos == 0, "Reinicio limpa vidas e pontos");
        QuestSession.Atual.PerderVida();
        QuestSession.Atual.PerderVida();
        QuestSession.Atual.PerderVida();
        yield return new WaitForSeconds(.2f);
        Exigir(SceneManager.GetActiveScene().name == "Final" && !QuestSession.Venceu, "Derrota ao zerar vidas");
        Debug.Log("QUEST_PLAYTEST_OK");
        UnityEditor.EditorApplication.Exit(0);
    }

    private void Exigir(bool condicao, string mensagem)
    {
        if (!condicao)
        {
            Debug.LogError("QUEST_PLAYTEST_FAILED: " + mensagem);
            UnityEditor.EditorApplication.Exit(1);
            throw new InvalidOperationException(mensagem);
        }
        Debug.Log("QUEST_CHECK_OK: " + mensagem);
    }

    private void Erro(string mensagem, string pilha, LogType tipo)
    {
        if (tipo == LogType.Exception && !pilha.Contains("UnityEditor.Search."))
        {
            File.AppendAllText(Path.GetFullPath(Application.dataPath + "/../../../work/playtest-exceptions.txt"), mensagem + "\n" + pilha + "\n");
            UnityEditor.EditorApplication.delayCall += () => UnityEditor.EditorApplication.Exit(1);
        }
    }

    private void Capturar(string nome)
    {
        Camera camera = Camera.main;
        RenderTexture destino = new RenderTexture(1280, 720, 24);
        RenderTexture anterior = RenderTexture.active;
        camera.targetTexture = destino;
        camera.Render();
        RenderTexture.active = destino;
        Texture2D imagem = new Texture2D(1280, 720, TextureFormat.RGB24, false);
        imagem.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
        imagem.Apply();
        string pasta = Path.GetFullPath(Application.dataPath + "/../../../outputs");
        Directory.CreateDirectory(pasta);
        File.WriteAllBytes(Path.Combine(pasta, nome), imagem.EncodeToPNG());
        camera.targetTexture = null;
        RenderTexture.active = anterior;
        Destroy(destino);
        Destroy(imagem);
    }
}
#endif
