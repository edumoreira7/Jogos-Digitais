using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class QuestBackgroundSetup
{
    private const string Pasta = "Assets/ElementalQuest/Fundos";

    public static void Criar(Camera camera, Transform jogador, int fase)
    {
        Directory.CreateDirectory(Pasta);
        AssetDatabase.Refresh();
        if (fase == 1)
        {
            Camada(camera, jogador, "Ceu dourado", "Background1novo.png", .04f, -30, Color.white);
            Camada(camera, jogador, "Montanhas distantes", "Background1.4novo.png", .11f, -29, new Color(.9f, .94f, 1f, .85f));
            Camada(camera, jogador, "Floresta com nevoa", "Background1.2novo.png", .22f, -28, new Color(.9f, .95f, 1f, .72f));
        }
        else
        {
            Camada(camera, jogador, "Ceu de entardecer", "Backkground2novo.png", .05f, -30, Color.white);
            Camada(camera, jogador, "Montanhas do vale", "Background2.4novo.png", .14f, -29, new Color(.9f, .92f, 1f, .85f));
            Camada(camera, jogador, "Ruinas entre as nuvens", "Background2.3novo.png", .25f, -28, new Color(.94f, .96f, 1f, .9f));
        }
    }

    private static void Camada(Camera camera, Transform jogador, string nome, string arquivo, float fator, int ordem, Color cor)
    {
        string caminho = Pasta + "/" + arquivo;
        File.Copy("Assets/Background/" + arquivo, caminho, true);
        AssetDatabase.ImportAsset(caminho, ImportAssetOptions.ForceSynchronousImport);
        TextureImporter importador = (TextureImporter)AssetImporter.GetAtPath(caminho);
        importador.textureType = TextureImporterType.Sprite;
        importador.spriteImportMode = SpriteImportMode.Single;
        importador.spritePixelsPerUnit = 100f;
        importador.alphaIsTransparency = true;
        importador.filterMode = FilterMode.Point;
        importador.textureCompression = TextureImporterCompression.Uncompressed;
        importador.maxTextureSize = 2048;
        importador.npotScale = TextureImporterNPOTScale.None;
        importador.mipmapEnabled = false;
        var configuracao = new TextureImporterSettings();
        importador.ReadTextureSettings(configuracao);
        configuracao.spriteMeshType = SpriteMeshType.FullRect;
        configuracao.spriteAlignment = (int)SpriteAlignment.Center;
        configuracao.spritePivot = new Vector2(.5f, .5f);
        importador.SetTextureSettings(configuracao);
        importador.SaveAndReimport();
        Sprite sprite = AssetDatabase.LoadAllAssetsAtPath(caminho).OfType<Sprite>().First();
        string caminhoMaterial = Pasta + "/" + Path.GetFileNameWithoutExtension(arquivo) + ".mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(caminhoMaterial);
        if (material == null)
        {
            material = new Material(Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default"));
            AssetDatabase.CreateAsset(material, caminhoMaterial);
        }
        material.mainTexture = sprite.texture;
        EditorUtility.SetDirty(material);
        GameObject raiz = new GameObject("Fundo - " + nome);
        QuestBackground fundo = raiz.AddComponent<QuestBackground>();
        fundo.alvo = jogador;
        fundo.cameraAlvo = camera.transform;
        fundo.fator = fator;
        fundo.paineis = new Transform[4];
        float escala = (camera.orthographicSize * 2f + 1.2f) / sprite.bounds.size.y;
        float largura = sprite.bounds.size.x * escala;
        for (int indice = 0; indice < fundo.paineis.Length; indice++)
        {
            GameObject painel = new GameObject(nome + " " + indice);
            painel.transform.SetParent(raiz.transform);
            painel.transform.position = new Vector3(camera.transform.position.x + indice * largura, camera.transform.position.y, 5f);
            painel.transform.localScale = new Vector3(escala, escala, 1f);
            SpriteRenderer visual = painel.AddComponent<SpriteRenderer>();
            visual.sprite = sprite;
            visual.sharedMaterial = material;
            visual.sortingOrder = ordem;
            visual.color = cor;
            visual.flipX = indice % 2 == 1;
            fundo.paineis[indice] = painel.transform;
        }
    }

    [MenuItem("Elemental Quest/Atualizar apenas os fundos")]
    public static void Atualizar()
    {
        foreach (int numero in new[] { 1, 2 })
        {
            Scene cena = EditorSceneManager.OpenScene("Assets/Scenes/Fase" + numero + ".unity");
            foreach (QuestBackground fundo in Object.FindObjectsByType<QuestBackground>())
                Object.DestroyImmediate(fundo.gameObject);
            Criar(Camera.main, Object.FindAnyObjectByType<QuestPlayer>().transform, numero);
            EditorSceneManager.SaveScene(cena);
        }
        AssetDatabase.SaveAssets();
        Debug.Log("QUEST_NEW_BACKGROUNDS_OK");
    }

    public static void AtualizarETestar()
    {
        Atualizar();
        QuestBuilder.Testar();
    }
}
