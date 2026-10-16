using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class QuestBuilder
{
    private const string Raiz = "Assets/ElementalQuest";
    private static Sprite grama;
    private static Sprite terra;
    private static Dictionary<string, Sprite> recortes;
    private static GameObject disparoInimigo;
    private static Sprite[] cristais;
    private static Sprite[] portais;
    private static GameObject patoPrefab;
    private static Material material;

    [MenuItem("Elemental Quest/Criar ou reconstruir aventura")]
    public static void Criar()
    {
        Directory.CreateDirectory(Raiz + "/Arte");
        Directory.CreateDirectory(Raiz + "/Prefabs");
        AssetDatabase.Refresh();
        material = AssetDatabase.LoadAssetAtPath<Material>(Raiz + "/Arte/SpritesSemLuz.mat");
        if (material == null)
        {
            material = new Material(Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default"));
            AssetDatabase.CreateAsset(material, Raiz + "/Arte/SpritesSemLuz.mat");
        }
        material.shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
        EditorUtility.SetDirty(material);
        recortes = QuestSlicer.Criar();
        grama = Peca(0);
        terra = Peca(9);
        CriarDisparoInimigo();
        cristais = Arte("Cristal", 16, 20, 6, 24f);
        portais = Arte("Portal", 32, 48, 6, 24f);

        EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
        PatoController original = UnityEngine.Object.FindFirstObjectByType<PatoController>();
        if (original == null)
            throw new InvalidOperationException("O Pato nao foi encontrado na SampleScene.");
        GameObject copia = UnityEngine.Object.Instantiate(original.gameObject);
        copia.name = "PatoMago";
        GameObject poder = UnityEngine.Object.Instantiate(original.prefabPoder);
        poder.name = "MagiaDoPato";
        poder.GetComponent<SpriteRenderer>().sharedMaterial = MaterialSprite(poder.GetComponent<SpriteRenderer>().sprite);
        copia.GetComponent<PatoController>().prefabPoder = PrefabUtility.SaveAsPrefabAsset(poder, Raiz + "/Prefabs/MagiaDoPato.prefab");
        UnityEngine.Object.DestroyImmediate(poder);
        copia.GetComponent<SpriteRenderer>().sharedMaterial = MaterialSprite(copia.GetComponent<SpriteRenderer>().sprite);
        copia.GetComponent<SpriteRenderer>().sortingOrder = 10;
        copia.GetComponent<Rigidbody2D>().interpolation = RigidbodyInterpolation2D.Interpolate;
        patoPrefab = PrefabUtility.SaveAsPrefabAsset(copia, Raiz + "/Prefabs/PatoMago.prefab");
        CriarTela("Inicio", 0);
        CriarFase(1);
        CriarFase(2);
        CriarTela("Final", 3);
        EditorBuildSettings.scenes = new[] { "Inicio", "Fase1", "Fase2", "Final" }
            .Select(nome => new EditorBuildSettingsScene("Assets/Scenes/" + nome + ".unity", true)).ToArray();
        PlayerSettings.productName = "Elemental Quest";
        PlayerSettings.defaultScreenWidth = 1280;
        PlayerSettings.defaultScreenHeight = 720;
        AssetDatabase.SaveAssets();
        EditorSceneManager.OpenScene("Assets/Scenes/Inicio.unity");
        Debug.Log("QUEST_BUILD_OK: Inicio, Fase1, Fase2 e Final criadas.");
    }

    private static Sprite CarregarSprite(string caminho)
    {
        return AssetDatabase.LoadAllAssetsAtPath(caminho).OfType<Sprite>().First();
    }

    private static Camera CameraNova()
    {
        GameObject objeto = new GameObject("Main Camera");
        objeto.tag = "MainCamera";
        Camera camera = objeto.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 5.5f;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(.18f, .5f, .8f);
        camera.transform.position = new Vector3(0f, .3f, -10f);
        objeto.AddComponent<AudioListener>();
        return camera;
    }

    private static void Fundo(Camera camera, Transform jogador, bool noite)
    {
        Sprite primeiro = CarregarSprite("Assets/Background/BackGround 1.1.png");
        Sprite segundo = CarregarSprite("Assets/Background/Background 2.1.png");
        GameObject raiz = new GameObject("Ceu - parallax acompanha o pato");
        QuestBackground fundo = raiz.AddComponent<QuestBackground>();
        fundo.alvo = jogador;
        fundo.cameraAlvo = camera.transform;
        fundo.fator = noite ? .28f : .18f;
        fundo.paineis = new Transform[4];
        for (int indice = 0; indice < 4; indice++)
        {
            SpriteRenderer visual = Visual("Ceu " + indice, indice % 2 == 0 ? primeiro : segundo, Vector2.zero, -20);
            visual.transform.SetParent(raiz.transform);
            visual.transform.localScale = Vector3.one * 2.1f;
            visual.transform.position = new Vector3(indice * primeiro.bounds.size.x * 2.1f, .3f, 5f);
            visual.color = noite ? new Color(.57f, .68f, .92f) : Color.white;
            fundo.paineis[indice] = visual.transform;
        }
    }

    private static SpriteRenderer Visual(string nome, Sprite sprite, Vector2 posicao, int ordem)
    {
        GameObject objeto = new GameObject(nome);
        objeto.transform.position = posicao;
        SpriteRenderer visual = objeto.AddComponent<SpriteRenderer>();
        visual.sprite = sprite;
        visual.sortingOrder = ordem;
        visual.sharedMaterial = MaterialSprite(sprite);
        return visual;
    }

    private static Material MaterialSprite(Sprite sprite)
    {
        string identificador = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(sprite.texture));
        string caminho = Raiz + "/Arte/Material_" + identificador + ".mat";
        Material resultado = AssetDatabase.LoadAssetAtPath<Material>(caminho);
        if (resultado == null)
        {
            resultado = new Material(material.shader);
            AssetDatabase.CreateAsset(resultado, caminho);
        }
        resultado.mainTexture = sprite.texture;
        EditorUtility.SetDirty(resultado);
        return resultado;
    }

    private static void Plataforma(float inicio, int tamanho, float topo, bool suspensa, bool noite)
    {
        GameObject raiz = new GameObject(suspensa ? "Plataforma suspensa" : "Ilha de terra");
        raiz.layer = 3;
        raiz.transform.position = new Vector3(inicio, topo, 0);
        int profundidade = suspensa ? 1 : 4;
        if (suspensa)
        {
            int variante = Mathf.RoundToInt(inicio) % 2;
            TileFisico(variante == 0 ? 16 : 15, new Vector2(inicio + .5f, topo - .5f), 1f, 1f, raiz.transform, noite);
            TileFisico(17, new Vector2(inicio + 2f, topo - .5f), 2f, 1f, raiz.transform, noite);
            TileFisico(variante == 0 ? 21 : 18, new Vector2(inicio + 3.5f, topo - .5f), 1f, 1f, raiz.transform, noite);
            return;
        }
        BoxCollider2D colisor = raiz.AddComponent<BoxCollider2D>();
        colisor.size = new Vector2(tamanho, profundidade);
        colisor.offset = new Vector2(tamanho / 2f, -profundidade / 2f);
        for (int coluna = 0; coluna < tamanho; coluna++)
        {
            for (int linha = 0; linha < profundidade; linha++)
            {
                int indice = linha == 0 ? new[] { 0, 1, 3, 4 }[coluna % 4] : coluna == 0 ? 8 : coluna == tamanho - 1 ? 10 : 9;
                SpriteRenderer tile = Visual("Tile " + indice.ToString("00") + " - " + coluna + "," + linha, Peca(indice),
                    new Vector2(inicio + coluna + .5f, topo - linha - .5f), 0);
                tile.transform.SetParent(raiz.transform);
                tile.transform.localScale = new Vector3(1f / tile.sprite.bounds.size.x, 1f / tile.sprite.bounds.size.y, 1f);
                if (noite)
                    tile.color = new Color(.75f, .85f, .83f);
            }
        }
    }

    private static Sprite Peca(int indice)
    {
        return recortes["Cenario_" + indice.ToString("00")];
    }

    private static Sprite[] Quadros(string nome, string acao)
    {
        return recortes.Where(par => par.Key.StartsWith(nome + "_" + acao + "_")).OrderBy(par => par.Key).Select(par => par.Value).ToArray();
    }

    private static void TileFisico(int indice, Vector2 centro, float largura, float altura, Transform pai, bool noite)
    {
        SpriteRenderer visual = Visual("Terreno recortado " + indice.ToString("00"), Peca(indice), centro, 0);
        visual.transform.SetParent(pai);
        visual.transform.localScale = new Vector3(largura / visual.sprite.bounds.size.x, altura / visual.sprite.bounds.size.y, 1f);
        visual.gameObject.layer = 3;
        if (noite)
            visual.color = new Color(.75f, .85f, .83f);
        PolygonCollider2D colisor = visual.gameObject.AddComponent<PolygonCollider2D>();
        colisor.SetPath(0, QuestSlicer.Contorno(visual.sprite));
    }

    private static void EnriquecerCenario(int numero)
    {
        Transform raiz = new GameObject("Rampas, ilhas curvas e vegetacao - 30 pecas").transform;
        int[] indices = { 2, 5, 6, 7, 11, 12, 13, 14, 19, 20 };
        float[] posicoes = numero == 1 ? new[] { 9f, 27f, 46f, 35f, 53f } : new[] { 7f, 23f, 38f, 44f, 57f };
        for (int indice = 0; indice < indices.Length; indice++)
            TileFisico(indices[indice], new Vector2(posicoes[indice / 2] + (indice % 2) * 1.5f, -.7f), 1.5f, 1f, raiz, numero == 2);
        float[] vegetacao = { -7f, -3f, 2f, 7f, 18f, 27f, 38f, 49f };
        float[] alturas = { .4f, .5f, .5f, .6f, .6f, .5f, .9f, .4f };
        for (int indice = 0; indice < vegetacao.Length; indice++)
        {
            Sprite sprite = Peca(indice + 22);
            SpriteRenderer visual = Visual("Decoracao " + sprite.name, sprite, new Vector2(vegetacao[indice], -3f + alturas[indice] / 2f), 1);
            visual.transform.SetParent(raiz);
            visual.transform.localScale = Vector3.one * (alturas[indice] / sprite.bounds.size.y);
            if (numero == 2)
                visual.color = new Color(.75f, .85f, .83f);
        }
    }

    private static void CriarDisparoInimigo()
    {
        Sprite[] quadros = Quadros("Atirador", "Projetil");
        SpriteRenderer visual = Visual("Semente magica inimiga", quadros[0], Vector2.zero, 7);
        visual.transform.localScale = Vector3.one * .6f;
        QuestAnimation animacao = visual.gameObject.AddComponent<QuestAnimation>();
        animacao.quadros = quadros;
        Rigidbody2D corpo = visual.gameObject.AddComponent<Rigidbody2D>();
        corpo.gravityScale = 0f;
        corpo.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        CircleCollider2D colisor = visual.gameObject.AddComponent<CircleCollider2D>();
        colisor.radius = .2f;
        colisor.isTrigger = true;
        visual.gameObject.AddComponent<QuestEnemyShot>();
        disparoInimigo = PrefabUtility.SaveAsPrefabAsset(visual.gameObject, Raiz + "/Prefabs/SementeInimiga.prefab");
        UnityEngine.Object.DestroyImmediate(visual.gameObject);
    }

    private static void Cristal(float posicaoX, float posicaoY)
    {
        SpriteRenderer visual = Visual("Cristal coletavel +100", cristais[0], new Vector2(posicaoX, posicaoY), 5);
        QuestAnimation animacao = visual.gameObject.AddComponent<QuestAnimation>();
        animacao.quadros = cristais;
        visual.gameObject.AddComponent<QuestCrystal>();
        CircleCollider2D colisor = visual.gameObject.AddComponent<CircleCollider2D>();
        colisor.radius = .35f;
        colisor.isTrigger = true;
    }

    private static void Inimigo(float esquerda, float direita, float topo, QuestEnemy.Tipo tipo)
    {
        string nome = tipo.ToString();
        Sprite[] andar = Quadros(nome, "Andar");
        SpriteRenderer visual = Visual(nome, andar[0], new Vector2((esquerda + direita) / 2f, topo + .02f), 6);
        float escala = tipo == QuestEnemy.Tipo.Guardiao ? 1.5f : 1f;
        visual.transform.localScale = Vector3.one * escala;
        Rigidbody2D corpo = visual.gameObject.AddComponent<Rigidbody2D>();
        corpo.bodyType = RigidbodyType2D.Kinematic;
        corpo.useFullKinematicContacts = true;
        corpo.interpolation = RigidbodyInterpolation2D.Interpolate;
        BoxCollider2D colisor = visual.gameObject.AddComponent<BoxCollider2D>();
        colisor.size = new Vector2(.7f, .8f);
        colisor.offset = new Vector2(.05f, .45f);
        colisor.isTrigger = true;
        QuestEnemy inimigo = visual.gameObject.AddComponent<QuestEnemy>();
        inimigo.limiteEsquerdo = esquerda;
        inimigo.limiteDireito = direita;
        inimigo.tipo = tipo;
        inimigo.vidas = tipo == QuestEnemy.Tipo.Guardiao ? 2 : 1;
        inimigo.velocidade = tipo == QuestEnemy.Tipo.Guardiao ? .85f : tipo == QuestEnemy.Tipo.Atirador ? .5f : 1.4f;
        inimigo.andar = andar;
        inimigo.parado = Quadros(nome, "Parado");
        inimigo.atacar = Quadros(nome, "Ataque");
        inimigo.dano = Quadros(nome, "Dano");
        inimigo.morte = Quadros(nome, "Morte");
        inimigo.prefabDisparo = disparoInimigo;
    }

    private static void Portal(float posicaoX)
    {
        SpriteRenderer visual = Visual("Portal - exige todos os cristais", portais[0], new Vector2(posicaoX, -2f), 4);
        QuestAnimation animacao = visual.gameObject.AddComponent<QuestAnimation>();
        animacao.quadros = portais;
        BoxCollider2D colisor = visual.gameObject.AddComponent<BoxCollider2D>();
        colisor.size = new Vector2(.9f, 1.8f);
        colisor.isTrigger = true;
        visual.gameObject.AddComponent<QuestPortal>();
    }

    private static void Placa(string mensagem, Vector2 posicao)
    {
        GameObject objeto = new GameObject("Placa " + mensagem);
        objeto.transform.position = posicao;
        TextMesh texto = objeto.AddComponent<TextMesh>();
        texto.text = mensagem;
        texto.fontSize = 48;
        texto.characterSize = .065f;
        texto.anchor = TextAnchor.MiddleCenter;
        texto.alignment = TextAlignment.Center;
        texto.color = new Color(.06f, .12f, .23f);
        objeto.GetComponent<MeshRenderer>().sortingOrder = 12;
    }

    private static void CriarFase(int numero)
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        Camera camera = CameraNova();
        GameObject jogador = (GameObject)PrefabUtility.InstantiatePrefab(patoPrefab);
        jogador.transform.position = new Vector3(-5f, -2.3f, 0f);
        jogador.transform.localScale = Vector3.one * .72f;
        QuestPlayer vida = jogador.AddComponent<QuestPlayer>();
        vida.pontoSeguro = jogador.transform.position;
        QuestCamera seguir = camera.gameObject.AddComponent<QuestCamera>();
        seguir.alvo = jogador.transform;
        seguir.maximoX = numero == 1 ? 49f : 57f;
        Fundo(camera, jogador.transform, numero == 2);
        QuestSession sessao = new GameObject("Regras, vidas e pontuacao").AddComponent<QuestSession>();
        sessao.fase = numero;
        sessao.nomeFase = numero == 1 ? "Jardim das Nuvens" : "Ruinas Esmeralda";
        if (numero == 1)
        {
            Plataforma(-12, 27, -3f, false, false);
            Plataforma(17, 16, -3f, false, false);
            Plataforma(35, 26, -3f, false, false);
            Plataforma(3, 4, -1.8f, true, false);
            Plataforma(22, 4, -1.8f, true, false);
            Plataforma(40, 4, -1.8f, true, false);
            foreach (float posicaoX in new[] { -2f, 1f, 10f, 13f, 19f, 28f, 31f, 37f, 47f, 52f })
                Cristal(posicaoX, -2.15f);
            foreach (float posicaoX in new[] { 4f, 6f, 23f, 25f, 41f, 43f })
                Cristal(posicaoX, -.95f);
            Inimigo(8f, 12f, -3f, QuestEnemy.Tipo.Guaxinim);
            Inimigo(27f, 31f, -3f, QuestEnemy.Tipo.Atirador);
            Inimigo(45f, 49f, -3f, QuestEnemy.Tipo.Guardiao);
            Portal(55f);
            Placa("CRISTAIS NAS PLATAFORMAS\nEspaco para pular", new Vector2(4.5f, .2f));
            Placa("J: MAGIA\nCuidado com os inimigos", new Vector2(11f, 1.3f));
        }
        else
        {
            Plataforma(-12, 24, -3f, false, true);
            Plataforma(14, 13, -3f, false, true);
            Plataforma(29, 15, -3f, false, true);
            Plataforma(46, 23, -3f, false, true);
            Plataforma(1, 4, -1.8f, true, true);
            Plataforma(17, 4, -1.8f, true, true);
            Plataforma(32, 4, -1.8f, true, true);
            Plataforma(50, 4, -1.8f, true, true);
            foreach (float posicaoX in new[] { -2f, 7f, 10f, 15f, 23f, 25f, 30f, 39f, 42f, 47f, 56f, 60f })
                Cristal(posicaoX, -2.15f);
            foreach (float posicaoX in new[] { 2f, 4f, 18f, 20f, 33f, 35f, 51f, 53f })
                Cristal(posicaoX, -.95f);
            Inimigo(6f, 10f, -3f, QuestEnemy.Tipo.Guaxinim);
            Inimigo(22f, 25f, -3f, QuestEnemy.Tipo.Atirador);
            Inimigo(37f, 42f, -3f, QuestEnemy.Tipo.Guardiao);
            Inimigo(55f, 60f, -3f, QuestEnemy.Tipo.Atirador);
            Portal(63f);
            Placa("RUINAS ESMERALDA\nRecupere os ultimos cristais", new Vector2(-1f, .4f));
        }
        EnriquecerCenario(numero);
        sessao.totalItens = UnityEngine.Object.FindObjectsByType<QuestCrystal>(FindObjectsSortMode.None).Length;
        PhysicsMaterial2D semAtrito = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(Raiz + "/Prefabs/SemAtrito.physicsMaterial2D");
        if (semAtrito == null)
        {
            semAtrito = new PhysicsMaterial2D("SemAtrito") { friction = 0f, bounciness = 0f };
            AssetDatabase.CreateAsset(semAtrito, Raiz + "/Prefabs/SemAtrito.physicsMaterial2D");
        }
        jogador.GetComponent<Collider2D>().sharedMaterial = semAtrito;
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), "Assets/Scenes/Fase" + numero + ".unity");
    }

    private static void CriarTela(string nome, int fase)
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        Camera camera = CameraNova();
        Fundo(camera, null, fase == 3);
        QuestSession sessao = new GameObject("Tela " + nome).AddComponent<QuestSession>();
        sessao.fase = fase;
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), "Assets/Scenes/" + nome + ".unity");
    }

    private static Sprite[] Arte(string tipo, int largura, int altura, int quantidade, float pixelsPorUnidade)
    {
        Sprite[] resultado = new Sprite[quantidade];
        for (int quadro = 0; quadro < quantidade; quadro++)
        {
            Texture2D textura = new Texture2D(largura, altura, TextureFormat.RGBA32, false);
            Color[] pixels = new Color[largura * altura];
            for (int vertical = 0; vertical < altura; vertical++)
            {
                for (int horizontal = 0; horizontal < largura; horizontal++)
                {
                    Color cor = Color.clear;
                    float centroX = horizontal - (largura - 1) / 2f;
                    if (tipo == "Slime")
                    {
                        float raioX = 10f + (quadro % 2);
                        float raioY = 11f - (quadro % 2) * 2f;
                        float distancia = centroX * centroX / (raioX * raioX) + (vertical - 3f) * (vertical - 3f) / (raioY * raioY);
                        if (vertical >= 2 && distancia < 1f)
                            cor = distancia > .76f ? new Color(.07f, .2f, .22f) : new Color(.2f, .8f - vertical * .012f, .55f);
                        if (vertical >= 7 && vertical <= 9 && (horizontal == 8 || horizontal == 15))
                            cor = new Color(.03f, .08f, .15f);
                        if (vertical == 11 && horizontal > 6 && horizontal < 12)
                            cor = new Color(.64f, 1f, .77f);
                    }
                    else if (tipo == "Cristal")
                    {
                        float raio = 4f + Mathf.Abs(Mathf.Cos(quadro * Mathf.PI / 3f)) * 2f;
                        float distancia = Mathf.Abs(centroX) / raio + Mathf.Abs(vertical - 10f) / 9f;
                        if (distancia < 1f)
                            cor = distancia > .78f ? new Color(.16f, .25f, .5f) : horizontal < 8 ? new Color(.5f, 1f, 1f) : new Color(.13f, .63f, .95f);
                        if (horizontal == 7 && vertical > 9 && vertical < 15)
                            cor = Color.white;
                    }
                    else
                    {
                        float distancia = centroX * centroX / 170f + (vertical - 24f) * (vertical - 24f) / 470f;
                        if (distancia < 1f && distancia > .65f)
                            cor = new Color(.27f + quadro * .055f, .75f, 1f);
                        else if (distancia <= .65f)
                            cor = new Color(.12f, .16f + .08f * Mathf.Sin(vertical + quadro), .4f, .9f);
                        if (vertical < 4 && Mathf.Abs(centroX) < 15)
                            cor = new Color(.27f, .35f, .42f);
                    }
                    pixels[vertical * largura + horizontal] = cor;
                }
            }
            textura.SetPixels(pixels);
            textura.Apply();
            string caminho = Raiz + "/Arte/" + tipo + quadro + ".png";
            File.WriteAllBytes(caminho, textura.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(textura);
            AssetDatabase.ImportAsset(caminho, ImportAssetOptions.ForceSynchronousImport);
            TextureImporter importador = (TextureImporter)AssetImporter.GetAtPath(caminho);
            importador.textureType = TextureImporterType.Sprite;
            importador.spritePixelsPerUnit = pixelsPorUnidade;
            importador.filterMode = FilterMode.Point;
            importador.textureCompression = TextureImporterCompression.Uncompressed;
            importador.alphaIsTransparency = true;
            importador.mipmapEnabled = false;
            importador.SaveAndReimport();
            resultado[quadro] = AssetDatabase.LoadAssetAtPath<Sprite>(caminho);
        }
        return resultado;
    }

    public static void Validar()
    {
        foreach (string nome in new[] { "Inicio", "Fase1", "Fase2", "Final" })
        {
            Scene cena = EditorSceneManager.OpenScene("Assets/Scenes/" + nome + ".unity");
            foreach (GameObject raiz in cena.GetRootGameObjects())
            {
                foreach (Transform objeto in raiz.GetComponentsInChildren<Transform>(true))
                    if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(objeto.gameObject) > 0)
                        throw new InvalidOperationException("Script ausente: " + objeto.name);
            }
            QuestSession sessao = UnityEngine.Object.FindFirstObjectByType<QuestSession>();
            if (sessao == null)
                throw new InvalidOperationException("Regras ausentes: " + nome);
            if (nome.StartsWith("Fase"))
            {
                if (sessao.totalItens != UnityEngine.Object.FindObjectsByType<QuestCrystal>(FindObjectsSortMode.None).Length)
                    throw new InvalidOperationException("Contagem incorreta de cristais.");
                PatoController jogador = UnityEngine.Object.FindFirstObjectByType<PatoController>();
                if (jogador == null || jogador.prefabPoder == null || jogador.groundCheck == null)
                    throw new InvalidOperationException("Jogador incompleto.");
                if (UnityEngine.Object.FindFirstObjectByType<QuestPortal>() == null)
                    throw new InvalidOperationException("Portal ausente.");
                int pecasPresentes = UnityEngine.Object.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None)
                    .Where(visual => visual.sprite != null && visual.sprite.name.StartsWith("Cenario_"))
                    .Select(visual => visual.sprite.name).Distinct().Count();
                if (pecasPresentes != 30)
                    throw new InvalidOperationException("A fase nao utiliza as 30 pecas: " + pecasPresentes);
                QuestEnemy[] inimigos = UnityEngine.Object.FindObjectsByType<QuestEnemy>(FindObjectsSortMode.None);
                if (inimigos.Select(inimigo => inimigo.tipo).Distinct().Count() != 3)
                    throw new InvalidOperationException("Falta um dos tres tipos de inimigos.");
                if (inimigos.Any(inimigo => inimigo.andar.Length != 6 || inimigo.atacar.Length < 4 || inimigo.morte.Length != 2))
                    throw new InvalidOperationException("Animacao de inimigo incompleta.");
            }
        }
        Debug.Log("QUEST_VALIDATION_OK");
    }

    public static void ConstruirWindows()
    {
        Validar();
        string pasta = Path.GetFullPath(Application.dataPath + "/../../../outputs/ElementalQuest");
        Directory.CreateDirectory(pasta);
        var resultado = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes.Select(cena => cena.path).ToArray(),
            pasta + "/Elemental Quest.exe", BuildTarget.StandaloneWindows64, BuildOptions.Development);
        if (resultado.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
            throw new InvalidOperationException("A compilacao do jogo falhou: " + resultado.summary.result);
        Debug.Log("QUEST_WINDOWS_BUILD_OK");
    }

    public static void Testar()
    {
        Validar();
        EditorSceneManager.OpenScene("Assets/Scenes/Inicio.unity");
        EditorApplication.EnterPlaymode();
    }

    public static void PrepararETestar()
    {
        Criar();
        Testar();
    }
}
