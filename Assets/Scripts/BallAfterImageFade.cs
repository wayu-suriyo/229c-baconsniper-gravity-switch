using UnityEngine;

public class BallAfterImageFade : MonoBehaviour
{
    public float lifeTime = 0.25f;
    public float startAlpha = 0.20f;

    private float timer;
    private Renderer[] renderers;
    private Color[] baseColors;

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>(true);
        baseColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] == null) continue;

            Material mat = renderers[i].material;

            if (mat.HasProperty("_BaseColor"))
                baseColors[i] = mat.GetColor("_BaseColor");
            else if (mat.HasProperty("_Color"))
                baseColors[i] = mat.GetColor("_Color");
            else
                baseColors[i] = Color.white;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / lifeTime);

        float alpha = startAlpha * Mathf.Pow(1f - t, 2f);
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] == null) continue;

            Material mat = renderers[i].material;
            Color c = baseColors[i];
            c.a = alpha;

            if (mat.HasProperty("_BaseColor"))
                mat.SetColor("_BaseColor", c);

            if (mat.HasProperty("_Color"))
                mat.SetColor("_Color", c);
        }

        if (timer >= lifeTime)
            Destroy(gameObject);
    }
}