using UnityEngine;

public class TVFlicker : MonoBehaviour
{
    public Color[] colors = {
        new Color(0.2f, 0.4f, 1f),
        new Color(0.8f, 0.8f, 1f),
        new Color(0.3f, 0.1f, 0.5f)
    };
    public float intensity = 1.5f;
    public Light glow;                 // optional: the TV_Glow light
    Material m;
    float timer, nextChange;

    void Start()
    {
        m = GetComponent<Renderer>().material;
        m.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer < nextChange) return;
        timer = 0f;
        nextChange = Random.Range(0.05f, 0.3f);

        Color c = colors[Random.Range(0, colors.Length)];
        m.SetColor("_EmissionColor", c * intensity * Random.Range(0.6f, 1.2f));
        if (glow != null) glow.color = c;
    }
}
