using UnityEngine;

public class QuestAnimation : MonoBehaviour
{
    public Sprite[] quadros;
    public float velocidade = 8f;
    private SpriteRenderer visual;
    private Material materialQuadros;

    void Awake()
    {
        visual = GetComponent<SpriteRenderer>();
        materialQuadros = visual.material;
    }

    void Update()
    {
        if (quadros != null && quadros.Length > 0)
        {
            visual.sprite = quadros[Mathf.FloorToInt(Time.time * velocidade) % quadros.Length];
            materialQuadros.mainTexture = visual.sprite.texture;
        }
    }

    void OnDestroy()
    {
        if (materialQuadros != null)
            Destroy(materialQuadros);
    }
}
