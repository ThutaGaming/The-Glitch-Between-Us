using System.Collections;
using UnityEngine;

/// <summary>
/// A red energy wall that seals an arena exit while the boss fight is on. Solid BoxCollider for
/// blocking, plus a self-built double-sided scrolling "force field" visual, a hum and a glow.
/// Starts lowered; <see cref="Raise"/> / <see cref="Lower"/> animate it.
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class EnergyBarrier : MonoBehaviour
{
    [SerializeField] private Color color = new Color(1f, 0.18f, 0.1f, 0.55f);
    [SerializeField] private AudioClip humLoop;
    [SerializeField] private AudioClip raiseClip;

    private BoxCollider box;
    private Transform visualRoot;
    private Material material;
    private AudioSource hum;
    private Light glow;
    private float shown;

    private void Awake()
    {
        box = GetComponent<BoxCollider>();
        BuildVisual();
        SetShown(0f);
        box.enabled = false;
    }

    public void Configure(AudioClip hum, AudioClip raise)
    {
        humLoop = hum;
        raiseClip = raise;
        if (this.hum != null) this.hum.clip = humLoop;
    }

    private void BuildVisual()
    {
        visualRoot = new GameObject("BarrierVisual").transform;
        visualRoot.SetParent(transform, false);
        visualRoot.localPosition = box.center - new Vector3(0f, box.size.y * 0.5f, 0f);

        material = new Material(Shader.Find("Sprites/Default")) { mainTexture = BuildTexture(), color = color };
        material.mainTexture.wrapMode = TextureWrapMode.Repeat;

        for (int side = 0; side < 2; side++)
        {
            var q = GameObject.CreatePrimitive(PrimitiveType.Quad);
            q.name = side == 0 ? "Front" : "Back";
            Destroy(q.GetComponent<Collider>());
            q.transform.SetParent(visualRoot, false);
            q.transform.localPosition = new Vector3(0f, box.size.y * 0.5f, 0f);
            q.transform.localRotation = Quaternion.Euler(0f, side == 0 ? 0f : 180f, 0f);
            q.transform.localScale = new Vector3(box.size.x, box.size.y, 1f);
            var r = q.GetComponent<MeshRenderer>();
            r.sharedMaterial = material;
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            r.receiveShadows = false;
        }

        glow = new GameObject("BarrierGlow").AddComponent<Light>();
        glow.transform.SetParent(visualRoot, false);
        glow.transform.localPosition = new Vector3(0f, box.size.y * 0.5f, 0f);
        glow.type = LightType.Point;
        glow.color = new Color(color.r, color.g, color.b);
        glow.range = Mathf.Max(box.size.x, box.size.y) * 1.2f;
        glow.intensity = 0f;
        glow.shadows = LightShadows.None;

        hum = gameObject.AddComponent<AudioSource>();
        hum.clip = humLoop;
        hum.loop = true;
        hum.playOnAwake = false;
        hum.spatialBlend = 1f;
        hum.rolloffMode = AudioRolloffMode.Linear;
        hum.minDistance = 2f;
        hum.maxDistance = 20f;
        hum.volume = 0.5f;
        hum.pitch = 0.8f;
    }

    private static Texture2D BuildTexture()
    {
        const int size = 64;
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, true);
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            float band = Mathf.Pow(Mathf.Abs(Mathf.Sin(y * Mathf.PI / 16f)), 12f);
            float hex = Mathf.Pow(Mathf.Abs(Mathf.Sin((x + (y / 8 % 2) * 4) * Mathf.PI / 8f)), 20f) * 0.6f;
            tex.SetPixel(x, y, new Color(1f, 1f, 1f, Mathf.Clamp01(0.25f + band * 0.6f + hex)));
        }
        tex.Apply();
        return tex;
    }

    public void Raise()
    {
        StopAllCoroutines();
        box.enabled = true;
        if (raiseClip != null) BossFx.Sfx(raiseClip, transform.position + box.center, 1f, 0.9f, 3f, 40f);
        if (humLoop != null && !hum.isPlaying) { hum.clip = humLoop; hum.Play(); }
        StartCoroutine(Animate(1f));
    }

    public void Lower()
    {
        StopAllCoroutines();
        if (raiseClip != null) BossFx.Sfx(raiseClip, transform.position + box.center, 0.8f, 0.7f, 3f, 40f);
        StartCoroutine(Animate(0f));
    }

    private IEnumerator Animate(float target)
    {
        float start = shown;
        float t = 0f;
        while (t < 0.6f)
        {
            t += Time.deltaTime;
            SetShown(Mathf.Lerp(start, target, t / 0.6f));
            yield return null;
        }
        SetShown(target);
        if (target <= 0f)
        {
            box.enabled = false;
            hum.Stop();
        }
    }

    private void SetShown(float k)
    {
        shown = k;
        if (visualRoot != null)
        {
            visualRoot.localScale = new Vector3(1f, Mathf.Max(0.001f, k), 1f);
            visualRoot.gameObject.SetActive(k > 0.001f);
        }
        if (glow != null) glow.intensity = 3f * k;
    }

    private void Update()
    {
        if (shown <= 0f || material == null) return;
        material.mainTextureOffset = new Vector2(Time.time * 0.15f, Time.time * 0.8f);
        float flicker = 0.85f + 0.15f * Mathf.PerlinNoise(Time.time * 12f, 0.2f);
        material.color = new Color(color.r, color.g, color.b, color.a * flicker);
    }

    private void OnDestroy()
    {
        if (material != null)
        {
            if (material.mainTexture != null) Destroy(material.mainTexture);
            Destroy(material);
        }
    }
}
