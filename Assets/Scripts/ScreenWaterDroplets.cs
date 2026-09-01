using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Screen-space "water on the lens" overlay: droplets that cling, slide down and fade.
/// Drawn with IMGUI so it needs no Canvas, no imported sprite and no post-process volume —
/// the droplet and streak textures are generated procedurally in Awake.
/// Drive it with <see cref="SetWetness"/> (0 = dry, 1 = fully splashed).
/// </summary>
public class ScreenWaterDroplets : MonoBehaviour
{
    [Header("Spawning")]
    [Tooltip("Droplets spawned per second while wetness is at 1.")]
    [SerializeField] private float spawnRatePerSecond = 26f;
    [SerializeField] private int maxDroplets = 90;

    [Header("Droplet Size (fraction of screen height)")]
    [SerializeField] private float minSize = 0.012f;
    [SerializeField] private float maxSize = 0.055f;

    [Header("Motion")]
    [Tooltip("How fast a droplet slides down, in screen heights per second.")]
    [SerializeField] private float minSlideSpeed = 0.01f;
    [SerializeField] private float maxSlideSpeed = 0.10f;
    [Tooltip("Big droplets slide, small ones cling. Below this size a droplet barely moves.")]
    [SerializeField] private float slideSizeThreshold = 0.03f;

    [Header("Life")]
    [SerializeField] private float minLifetime = 1.2f;
    [SerializeField] private float maxLifetime = 3.5f;

    [Header("Look")]
    [SerializeField] private Color dropletTint = new Color(0.80f, 0.90f, 1.00f, 0.55f);
    [Tooltip("Cool wet haze laid over the whole screen, scaled by wetness.")]
    [SerializeField] private Color wetHazeTint = new Color(0.62f, 0.78f, 0.95f, 0.13f);

    private struct Droplet
    {
        public Vector2 pos;      // normalised screen position, y grows downward
        public float size;       // fraction of screen height
        public float speed;      // screen heights per second
        public float age;
        public float lifetime;
        public float wobblePhase;
        public float alphaScale;
    }

    private readonly List<Droplet> droplets = new List<Droplet>();
    private Texture2D dropletTexture;
    private Texture2D hazeTexture;
    private float wetness;
    private float spawnAccumulator;

    /// <summary>0 = dry, 1 = fully wet. Ramp it yourself for fade-in / fade-out.</summary>
    public void SetWetness(float value) => wetness = Mathf.Clamp01(value);

    public float Wetness => wetness;

    /// <summary>Throws a burst of droplets at the lens, e.g. the instant water hits the face.</summary>
    public void Splash(int count)
    {
        for (int i = 0; i < count; i++) SpawnDroplet();
    }

    public void ClearDroplets() => droplets.Clear();

    private void Awake()
    {
        dropletTexture = BuildDropletTexture(64);
        hazeTexture = BuildHazeTexture(64);
    }

    private void OnDestroy()
    {
        if (dropletTexture != null) Destroy(dropletTexture);
        if (hazeTexture != null) Destroy(hazeTexture);
    }

    private void Update()
    {
        if (wetness > 0f && droplets.Count < maxDroplets)
        {
            spawnAccumulator += spawnRatePerSecond * wetness * Time.deltaTime;
            while (spawnAccumulator >= 1f && droplets.Count < maxDroplets)
            {
                spawnAccumulator -= 1f;
                SpawnDroplet();
            }
        }

        for (int i = droplets.Count - 1; i >= 0; i--)
        {
            Droplet d = droplets[i];
            d.age += Time.deltaTime;
            if (d.age >= d.lifetime)
            {
                droplets.RemoveAt(i);
                continue;
            }

            // Small droplets cling; larger ones gather weight and run down the lens.
            float weight = Mathf.InverseLerp(slideSizeThreshold * 0.5f, maxSize, d.size);
            d.pos.y += d.speed * weight * Time.deltaTime;
            d.pos.x += Mathf.Sin((d.age + d.wobblePhase) * 2.2f) * 0.006f * weight * Time.deltaTime;

            droplets[i] = d;
        }
    }

    private void SpawnDroplet()
    {
        float size = Mathf.Lerp(minSize, maxSize, Random.value * Random.value);
        droplets.Add(new Droplet
        {
            // Bias toward the lower half: water runs down the lens and pools there.
            pos = new Vector2(Random.value, Mathf.Lerp(0.05f, 1.0f, Mathf.Sqrt(Random.value))),
            size = size,
            speed = Mathf.Lerp(minSlideSpeed, maxSlideSpeed, Random.value),
            age = 0f,
            lifetime = Random.Range(minLifetime, maxLifetime),
            wobblePhase = Random.Range(0f, 10f),
            alphaScale = Random.Range(0.6f, 1f)
        });
    }

    private void OnGUI()
    {
        if (Event.current.type != EventType.Repaint) return;
        if (wetness <= 0.001f && droplets.Count == 0) return;

        Color previousColor = GUI.color;
        float h = Screen.height;

        if (wetness > 0.001f)
        {
            Color haze = wetHazeTint;
            haze.a *= wetness;
            GUI.color = haze;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), hazeTexture);
        }

        foreach (Droplet d in droplets)
        {
            // Pop in quickly, linger, then fade as the droplet evaporates or runs off.
            float t = d.age / d.lifetime;
            float fade = SmoothStep01(t / 0.12f) * (1f - SmoothStep01((t - 0.6f) / 0.4f));

            Color c = dropletTint;
            c.a *= fade * d.alphaScale;
            if (c.a <= 0.002f) continue;

            float px = d.size * h;
            GUI.color = c;
            GUI.DrawTexture(
                new Rect(d.pos.x * Screen.width - px * 0.5f, d.pos.y * h - px * 0.5f, px, px * 1.15f),
                dropletTexture);
        }

        GUI.color = previousColor;
    }

    /// <summary>
    /// GLSL-style smoothstep on an already-normalised t. Deliberately NOT Mathf.SmoothStep,
    /// which interpolates between two values rather than shaping a 0..1 ramp.
    /// </summary>
    private static float SmoothStep01(float t)
    {
        t = Mathf.Clamp01(t);
        return t * t * (3f - 2f * t);
    }

    private static Texture2D BuildDropletTexture(int size)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false) { hideFlags = HideFlags.HideAndDontSave };
        var pixels = new Color[size * size];
        float r = size * 0.5f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = (x + 0.5f - r) / r;
                float dy = (y + 0.5f - r) / r;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);

                // Body: soft-edged disc.
                float body = 1f - SmoothStep01((dist - 0.55f) / 0.45f);

                // Rim: a droplet reads as water mostly from its bright meniscus.
                float rim = SmoothStep01((dist - 0.45f) / 0.25f) * (1f - SmoothStep01((dist - 0.80f) / 0.20f));

                // Specular highlight, offset up-left like a lit bead of water.
                float hx = dx + 0.30f;
                float hy = dy + 0.32f;
                float highlight = 1f - SmoothStep01((Mathf.Sqrt(hx * hx + hy * hy) - 0.05f) / 0.28f);

                float alpha = Mathf.Clamp01(body * 0.45f + rim * 0.75f + highlight * 0.85f);
                float lum = Mathf.Clamp01(0.62f + rim * 0.25f + highlight * 0.75f);
                pixels[y * size + x] = new Color(lum, lum, lum, alpha);
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();
        tex.wrapMode = TextureWrapMode.Clamp;
        return tex;
    }

    private static Texture2D BuildHazeTexture(int size)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false) { hideFlags = HideFlags.HideAndDontSave };
        var pixels = new Color[size * size];
        float r = size * 0.5f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = (x + 0.5f - r) / r;
                float dy = (y + 0.5f - r) / r;
                // Vignette: wetness gathers at the edges of the lens, centre stays clearer.
                float edge = SmoothStep01((Mathf.Sqrt(dx * dx + dy * dy) - 0.25f) / 0.85f);
                pixels[y * size + x] = new Color(1f, 1f, 1f, 0.25f + edge * 0.75f);
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();
        tex.wrapMode = TextureWrapMode.Clamp;
        return tex;
    }
}
