using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

public static class QuestSlicer
{
    private const string Pasta = "Assets/ElementalQuest/Recortes";
    private static readonly Dictionary<string, (Color32[] pixels, int largura)> Texturas = new Dictionary<string, (Color32[] pixels, int largura)>();

    public static Dictionary<string, Sprite> Criar()
    {
        Directory.CreateDirectory(Pasta);
        var resultado = new Dictionary<string, Sprite>();
        var terreno = new List<(string nome, Rect area)>();
        int[][] limites = {
            new[] { 22, 212, 395, 551, 723, 891, 1070, 1236, 1427 },
            new[] { 22, 212, 395, 550, 738, 933, 1180, 1427 },
            new[] { 22, 212, 395, 731, 891, 1070, 1237, 1427 },
            new[] { 22, 200, 381, 550, 727, 893, 1070, 1271, 1427 }
        };
        int[] topos = { 137, 350, 559, 810 };
        int[] fundos = { 333, 545, 754, 968 };
        int contador = 0;
        for (int linha = 0; linha < limites.Length; linha++)
            for (int coluna = 0; coluna < limites[linha].Length - 1; coluna++)
                terreno.Add(("Cenario_" + (contador++).ToString("00"), Area(limites[linha][coluna], topos[linha], limites[linha][coluna + 1], fundos[linha], 1086)));
        Importar("Assets/Tiles/Tileset Pixel Art de Terra e Vegetação.png", "Cenario", terreno, 180f, resultado);
        ImportarInimigo("inimigoCaC", "Guaxinim", new[] { 0, 400, 700, 995, 1300 },
            new[] { 0, 295, 565, 805, 1075, 1290, 1536 }, new[] { 0, 390, 780, 1150, 1536 }, resultado);
        ImportarInimigo("inimigoAtirador", "Atirador", new[] { 35, 320, 580, 850, 1140 },
            new[] { 0, 265, 530, 785, 1040, 1285, 1536 }, new[] { 0, 270, 540, 780, 1110 }, resultado);
        ImportarInimigo("inimigoGrande", "Guardiao", new[] { 80, 430, 750, 1070, 1400 },
            new[] { 0, 305, 550, 805, 1050, 1305, 1536 }, new[] { 0, 300, 590, 875, 1200, 1536 }, resultado);
        File.WriteAllText(Pasta + "/CATALOGO.txt", "Cenario_00 a Cenario_21: terreno, rampas, bordas e ilhas.\nCenario_22 a Cenario_29: vegetacao, flores e pedras.\n\n" + string.Join("\n", resultado.Keys.OrderBy(nome => nome)));
        return resultado;
    }

    private static void ImportarInimigo(string arquivo, string nome, int[] parado, int[] andar, int[] atacar, Dictionary<string, Sprite> resultado)
    {
        var areas = new List<(string nome, Rect area)>();
        Linha(areas, nome + "_Parado", parado, 15, 283);
        Linha(areas, nome + "_Andar", andar, 288, 520);
        Linha(areas, nome + "_Ataque", atacar, 535, 783);
        Linha(areas, nome + "_Dano", new[] { 0, 380, 745 }, 785, 1024);
        Linha(areas, nome + "_Morte", new[] { 745, 1110, 1536 }, 785, 1024);
        if (nome == "Atirador")
            Linha(areas, nome + "_Projetil", new[] { 1100, 1210, 1370, 1536 }, 590, 735);
        Importar("Assets/Sprites/" + arquivo + ".png", nome, areas, 230f, resultado);
    }

    private static void Linha(List<(string nome, Rect area)> areas, string nome, int[] limites, int topo, int fundo)
    {
        for (int indice = 0; indice < limites.Length - 1; indice++)
            areas.Add((nome + "_" + indice.ToString("00"), Area(limites[indice], topo, limites[indice + 1], fundo, 1024)));
    }

    private static Rect Area(int esquerda, int topo, int direita, int fundo, int altura)
    {
        return new Rect(esquerda, altura - fundo, direita - esquerda, fundo - topo);
    }

    private static void Importar(string origem, string nome, List<(string nome, Rect area)> areas, float escala, Dictionary<string, Sprite> resultado)
    {
        string destino = Pasta + "/" + nome + ".png";
        if (!File.Exists(destino))
            File.Copy(origem, destino);
        AssetDatabase.ImportAsset(destino, ImportAssetOptions.ForceSynchronousImport);
        TextureImporter importador = (TextureImporter)AssetImporter.GetAtPath(destino);
        importador.textureType = TextureImporterType.Sprite;
        importador.spriteImportMode = SpriteImportMode.Multiple;
        importador.spritePixelsPerUnit = escala;
        importador.alphaIsTransparency = true;
        importador.filterMode = FilterMode.Point;
        importador.textureCompression = TextureImporterCompression.Uncompressed;
        importador.mipmapEnabled = false;
        importador.maxTextureSize = 2048;
        importador.SaveAndReimport();
        Texture2D textura = new Texture2D(2, 2);
        textura.LoadImage(File.ReadAllBytes(destino));
        Texturas[destino] = (textura.GetPixels32(), textura.width);
        var fabricas = new SpriteDataProviderFactories();
        fabricas.Init();
        var provedor = fabricas.GetSpriteEditorDataProviderFromObject(importador);
        provedor.InitSpriteEditorDataProvider();
        var anteriores = provedor.GetSpriteRects().ToDictionary(rect => rect.name, rect => rect.spriteID);
        var recortes = areas.Select(area => new SpriteRect {
            name = area.nome,
            rect = nome == "Cenario" ? Ajustar(textura, area.area) : Personagem(textura, area.area),
            alignment = nome == "Cenario" ? SpriteAlignment.Center : SpriteAlignment.BottomCenter,
            pivot = nome == "Cenario" ? new Vector2(.5f, .5f) : new Vector2(.5f, 0f),
            spriteID = anteriores.TryGetValue(area.nome, out GUID identificador) ? identificador : GUID.Generate()
        }).ToArray();
        provedor.SetSpriteRects(recortes);
        provedor.GetDataProvider<ISpriteNameFileIdDataProvider>().SetNameFileIdPairs(recortes.Select(rect => new SpriteNameFileIdPair(rect.name, rect.spriteID)));
        provedor.Apply();
        importador.SaveAndReimport();
        foreach (Sprite sprite in AssetDatabase.LoadAllAssetsAtPath(destino).OfType<Sprite>())
            resultado[sprite.name] = sprite;
        Debug.Log("QUEST_SLICE_OK: " + nome + " / " + recortes.Length + " recortes");
        UnityEngine.Object.DestroyImmediate(textura);
    }

    private static Rect Ajustar(Texture2D textura, Rect area)
    {
        int esquerda = (int)area.xMax;
        int direita = (int)area.xMin;
        int fundo = (int)area.yMax;
        int topo = (int)area.yMin;
        for (int vertical = (int)area.yMin; vertical < area.yMax; vertical++)
            for (int horizontal = (int)area.xMin; horizontal < area.xMax; horizontal++)
                if (textura.GetPixel(horizontal, vertical).a > .5f)
                {
                    esquerda = Mathf.Min(esquerda, horizontal);
                    direita = Mathf.Max(direita, horizontal);
                    fundo = Mathf.Min(fundo, vertical);
                    topo = Mathf.Max(topo, vertical);
                }
        if (direita <= esquerda || topo <= fundo)
            throw new InvalidOperationException("Recorte vazio: " + area);
        return Rect.MinMaxRect(esquerda, fundo, direita + 1, topo + 1);
    }

    private static Rect Personagem(Texture2D textura, Rect area)
    {
        int largura = (int)area.width;
        int altura = (int)area.height;
        Color32[] pixels = textura.GetPixels32();
        bool[] visitados = new bool[largura * altura];
        int[] fila = new int[visitados.Length];
        int maior = 0;
        Rect resultado = area;
        for (int inicio = 0; inicio < visitados.Length; inicio++)
        {
            if (visitados[inicio])
                continue;
            visitados[inicio] = true;
            int posicaoX = inicio % largura;
            int posicaoY = inicio / largura;
            if (pixels[((int)area.y + posicaoY) * textura.width + (int)area.x + posicaoX].a <= 127)
                continue;
            int cabeca = 0;
            int fim = 1;
            fila[0] = inicio;
            int minimoX = posicaoX;
            int maximoX = posicaoX;
            int minimoY = posicaoY;
            int maximoY = posicaoY;
            while (cabeca < fim)
            {
                int atual = fila[cabeca++];
                int horizontal = atual % largura;
                int vertical = atual / largura;
                minimoX = Mathf.Min(minimoX, horizontal);
                maximoX = Mathf.Max(maximoX, horizontal);
                minimoY = Mathf.Min(minimoY, vertical);
                maximoY = Mathf.Max(maximoY, vertical);
                foreach (Vector2Int direcao in new[] { Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down })
                {
                    int vizinhoX = horizontal + direcao.x;
                    int vizinhoY = vertical + direcao.y;
                    if (vizinhoX < 0 || vizinhoX >= largura || vizinhoY < 0 || vizinhoY >= altura)
                        continue;
                    int vizinho = vizinhoY * largura + vizinhoX;
                    if (visitados[vizinho])
                        continue;
                    visitados[vizinho] = true;
                    if (pixels[((int)area.y + vizinhoY) * textura.width + (int)area.x + vizinhoX].a > 127)
                        fila[fim++] = vizinho;
                }
            }
            if (fim > maior)
            {
                maior = fim;
                resultado = new Rect(area.x + minimoX, area.y + minimoY, maximoX - minimoX + 1, maximoY - minimoY + 1);
            }
        }
        if (maior < 20)
            throw new InvalidOperationException("Quadro sem personagem: " + area);
        return resultado;
    }

    public static Vector2[] Contorno(Sprite sprite)
    {
        string caminho = AssetDatabase.GetAssetPath(sprite.texture);
        var textura = Texturas[caminho];
        Rect area = sprite.rect;
        var superiores = new List<Vector2>();
        var inferiores = new List<Vector2>();
        for (int indice = 0; indice <= 20; indice++)
        {
            int horizontal = Mathf.RoundToInt(Mathf.Lerp(area.xMin + 2, area.xMax - 3, indice / 20f));
            int baixo = -1;
            int alto = -1;
            for (int vertical = (int)area.yMin; vertical < area.yMax; vertical++)
                if (textura.pixels[vertical * textura.largura + horizontal].a > 153)
                {
                    if (baixo < 0)
                        baixo = vertical;
                    alto = vertical;
                }
            if (baixo < 0)
                continue;
            superiores.Add((new Vector2(horizontal, alto) - area.position - sprite.pivot) / sprite.pixelsPerUnit);
            inferiores.Add((new Vector2(horizontal, baixo) - area.position - sprite.pivot) / sprite.pixelsPerUnit);
        }
        inferiores.Reverse();
        superiores.AddRange(inferiores);
        return superiores.ToArray();
    }
}
