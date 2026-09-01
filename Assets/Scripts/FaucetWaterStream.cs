using UnityEngine;

/// <summary>
/// Water running from a tap: a falling stream plus a splash where it lands.
/// Both particle systems and their material are built at runtime, so this needs no
/// imported VFX asset — attach it, point <see cref="spoutPoint"/> at the spout opening and
/// <see cref="basinPoint"/> at the drain, and call <see cref="SetRunning"/>.
/// </summary>
public class FaucetWaterStream : MonoBehaviour
{
    [Header("Placement")]
    [Tooltip("Where the water leaves the tap. The stream is emitted downward from here.")]
    [SerializeField] private Transform spoutPoint;
    [Tooltip("Where the water lands. Only the height matters; it sets the splash position.")]
    [SerializeField] private Transform basinPoint;

    [Header("Stream")]
    [SerializeField] private float streamRadius = 0.009f;
    [SerializeField] private float streamSpeed = 2.2f;
    [Tooltip("High enough that the falling droplets read as one continuous column of water.")]
    [SerializeField] private int streamParticlesPerSecond = 520;
    [Tooltip("Sideways spread in degrees. Keep at 0 for a tap; anything more looks like a spray.")]
    [SerializeField] private float streamSpreadAngle = 0f;
    [SerializeField] private float streamGravity = 0.55f;

    [Header("Splash")]
    [Tooltip("Off by default - an upward splash cone reads as a fountain rather than a tap.")]
    [SerializeField] private bool enableSplash = false;
    [SerializeField] private int splashParticlesPerSecond = 45;
    [SerializeField] private float splashSpeed = 0.22f;
    [SerializeField] private float splashRadius = 0.035f;
    [SerializeField] private float splashAngle = 20f;

    [Header("Look")]
    [SerializeField] private Color waterColor = new Color(0.78f, 0.90f, 1f, 0.60f);

    private ParticleSystem stream;
    private ParticleSystem splash;
    private Material waterMaterial;
    private Texture2D waterTexture;
    private bool isRunning;

    public bool IsRunning => isRunning;

    private void Awake()
    {
        BuildMaterial();
        BuildStream();
        BuildSplash();
        SetRunning(false);
    }

    private void OnDestroy()
    {
        if (waterMaterial != null) Destroy(waterMaterial);
        if (waterTexture != null) Destroy(waterTexture);
    }

    public void SetRunning(bool running)
    {
        isRunning = running;

        if (stream != null)
        {
            var emission = stream.emission;
            emission.enabled = running;
            if (running && !stream.isPlaying) stream.Play();
            if (!running) stream.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        if (splash != null)
        {
            bool splashOn = running && enableSplash;
            var emission = splash.emission;
            emission.enabled = splashOn;
            if (splashOn && !splash.isPlaying) splash.Play();
            if (!splashOn) splash.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    private float FallDistance
    {
        get
        {
            if (spoutPoint == null || basinPoint == null) return 0.4f;
            return Mathf.Max(0.05f, spoutPoint.position.y - basinPoint.position.y);
        }
    }

    /// <summary>
    /// Seconds for a particle to fall <paramref name="distance"/> given its launch speed and
    /// gravity: solves d = v·t + ½g·t². Ignoring the g·t² term makes the lifetime far too long
    /// and the stream shoots straight through the bottom of the basin.
    /// </summary>
    private float TimeToFall(float distance)
    {
        float v = Mathf.Max(0.01f, streamSpeed);
        float g = Mathf.Abs(Physics.gravity.y) * streamGravity;
        if (g <= 0.001f) return distance / v;
        return (-v + Mathf.Sqrt(v * v + 2f * g * distance)) / g;
    }

    private void BuildStream()
    {
        var go = new GameObject("WaterStream");
        go.transform.SetParent(spoutPoint != null ? spoutPoint : transform, false);
        go.transform.localPosition = Vector3.zero;
        // Emit straight down regardless of how the tap prefab is rotated.
        go.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);

        stream = go.AddComponent<ParticleSystem>();
        var main = stream.main;
        // Die just as the water reaches the basin, so it doesn't punch through the sink.
        main.startLifetime = TimeToFall(FallDistance) * 1.04f;
        main.startSpeed = streamSpeed;
        main.startSize = 0.022f;
        main.startColor = waterColor;
        main.gravityModifier = streamGravity;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.playOnAwake = false;
        main.maxParticles = 900;

        var emission = stream.emission;
        emission.rateOverTime = streamParticlesPerSecond;

        var shape = stream.shape;
        // Cone with a ~0 angle emits along the object's +Z (pointed straight down above),
        // giving a vertical column. A Circle shape would fire particles radially OUTWARD in
        // the plane of the circle instead, which is what made this look like a spray.
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = streamSpreadAngle;
        shape.radius = streamRadius;
        shape.radiusThickness = 1f;

        // Water accelerates and necks down as it falls, like a real tap stream.
        var sizeOverLifetime = stream.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        var sizeCurve = new AnimationCurve();
        sizeCurve.AddKey(0f, 1f);
        sizeCurve.AddKey(1f, 0.75f);
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

        ConfigureRenderer(stream, ParticleSystemRenderMode.Stretch, 3.4f);
    }

    private void BuildSplash()
    {
        var go = new GameObject("WaterSplash");
        go.transform.SetParent(basinPoint != null ? basinPoint : transform, false);
        go.transform.localPosition = Vector3.zero;
        go.transform.rotation = Quaternion.LookRotation(Vector3.up, Vector3.forward);

        splash = go.AddComponent<ParticleSystem>();
        var main = splash.main;
        main.startLifetime = 0.18f;
        main.startSpeed = splashSpeed;
        main.startSize = 0.008f;
        main.startColor = waterColor;
        main.gravityModifier = 1.4f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.playOnAwake = false;
        main.maxParticles = 200;

        var emission = splash.emission;
        emission.rateOverTime = splashParticlesPerSecond;

        var shape = splash.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = splashAngle;
        shape.radius = splashRadius;

        var sizeOverLifetime = splash.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        var sizeCurve = new AnimationCurve();
        sizeCurve.AddKey(0f, 1f);
        sizeCurve.AddKey(1f, 0.1f);
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

        ConfigureRenderer(splash, ParticleSystemRenderMode.Billboard, 1f);
    }

    private void ConfigureRenderer(ParticleSystem ps, ParticleSystemRenderMode mode, float lengthScale)
    {
        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = mode;
        if (mode == ParticleSystemRenderMode.Stretch)
        {
            renderer.lengthScale = lengthScale;
            renderer.velocityScale = 0.05f;
        }
        renderer.material = waterMaterial;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;
    }

    private void BuildMaterial()
    {
        waterTexture = BuildDropTexture(32);

        // URP first; fall back through the shaders that exist in a plain project.
        Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
        if (shader == null) shader = Shader.Find("Sprites/Default");
        if (shader == null) shader = Shader.Find("Legacy Shaders/Particles/Alpha Blended");

        waterMaterial = new Material(shader) { hideFlags = HideFlags.HideAndDontSave };
        waterMaterial.mainTexture = waterTexture;
        if (waterMaterial.HasProperty("_BaseMap")) waterMaterial.SetTexture("_BaseMap", waterTexture);
        if (waterMaterial.HasProperty("_BaseColor")) waterMaterial.SetColor("_BaseColor", Color.white);
        if (waterMaterial.HasProperty("_Surface")) waterMaterial.SetFloat("_Surface", 1f); // transparent
        if (waterMaterial.HasProperty("_Blend")) waterMaterial.SetFloat("_Blend", 0f);     // alpha blend
        waterMaterial.renderQueue = 3000;
    }

    private static Texture2D BuildDropTexture(int size)
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
                float t = Mathf.Clamp01((dist - 0.15f) / 0.85f);
                float alpha = 1f - (t * t * (3f - 2f * t)); // GLSL-style smoothstep falloff
                pixels[y * size + x] = new Color(1f, 1f, 1f, alpha);
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();
        tex.wrapMode = TextureWrapMode.Clamp;
        return tex;
    }

    private void OnDrawGizmosSelected()
    {
        if (spoutPoint == null || basinPoint == null) return;
        Gizmos.color = new Color(0.4f, 0.8f, 1f);
        Gizmos.DrawLine(spoutPoint.position, basinPoint.position);
        Gizmos.DrawWireSphere(spoutPoint.position, 0.03f);
        Gizmos.DrawWireSphere(basinPoint.position, 0.05f);
    }
}
