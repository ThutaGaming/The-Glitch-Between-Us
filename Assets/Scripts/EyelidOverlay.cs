using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// First-person eyelids. A black mask with a soft, lens-shaped opening is drawn over the whole
/// screen with IMGUI: <see cref="Openness"/> 0 is shut, 1 is fully open (nothing drawn). While the
/// eyes are still heavy the view is also blurred through a temporary depth-of-field volume that
/// clears as they open. PlayerWakeUpSequence adds this at runtime for the morning wake-up.
/// </summary>
public class EyelidOverlay : MonoBehaviour
{
    private const int MaskWidth = 192;
    private const int MaskHeight = 108;

    [Tooltip("How far past the screen's left/right edges the corners of the eye sit. Larger opens the sides sooner.")]
    [SerializeField] private float cornerReach = 1.25f;
    [Tooltip("The lower lid travels less than the upper one.")]
    [SerializeField] private float lowerLidTravel = 0.8f;
    [Tooltip("Width of the lids' soft edge, in half-screen heights.")]
    [SerializeField] private float edgeSoftness = 0.12f;
    [Tooltip("Extra darkness over the open part while the eyes are still heavy.")]
    [SerializeField] private float drowsyDim = 0.2f;

    [Header("Blur")]
    [SerializeField] private bool blurWhileWaking = true;
    [Tooltip("Depth-of-field focus while the eyes are shut; anything further away is out of focus.")]
    [SerializeField] private float blurFocusDistance = 0.3f;
    [SerializeField] private float blurAperture = 1.4f;

    public float Openness { get; set; } = 1f;

    private Texture2D mask;
    private Color32[] pixels;
    private float builtFor = -1f;

    private GameObject blurGo;
    private Volume blurVolume;
    private VolumeProfile blurProfile;

    private void Awake()
    {
        mask = new Texture2D(MaskWidth, MaskHeight, TextureFormat.RGBA32, false)
        {
            hideFlags = HideFlags.HideAndDontSave,
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear
        };
        pixels = new Color32[MaskWidth * MaskHeight];

        if (blurWhileWaking) CreateBlurVolume();
    }

    private void OnDestroy()
    {
        if (mask != null) Destroy(mask);
        if (blurGo != null) Destroy(blurGo);
        if (blurProfile != null) Destroy(blurProfile);
    }

    private void Update()
    {
        // The blur lingers until the eyes are nearly open, then clears with them.
        if (blurVolume != null) blurVolume.weight = 1f - Mathf.Pow(Mathf.Clamp01(Openness), 1.5f);
    }

    /// <summary>A global volume that outranks the scene's own, overriding only the focus, so as its
    /// weight falls the scene's normal depth of field comes back without a pop.</summary>
    private void CreateBlurVolume()
    {
        blurProfile = ScriptableObject.CreateInstance<VolumeProfile>();
        blurProfile.hideFlags = HideFlags.HideAndDontSave;
        var dof = blurProfile.Add<DepthOfField>();
        dof.mode.Override(DepthOfFieldMode.Bokeh);
        dof.focusDistance.Override(blurFocusDistance);
        dof.aperture.Override(blurAperture);

        blurGo = new GameObject("~WakeUpBlur") { hideFlags = HideFlags.DontSave };
        blurVolume = blurGo.AddComponent<Volume>();
        blurVolume.isGlobal = true;
        blurVolume.priority = 100f;
        blurVolume.sharedProfile = blurProfile;
        blurVolume.weight = 1f;
    }

    private void OnGUI()
    {
        float open = Mathf.Clamp01(Openness);
        if (open >= 0.999f) return;

        if (Mathf.Abs(open - builtFor) > 0.0005f) RebuildMask(open);

        GUI.depth = -100;
        GUI.color = Color.white;
        GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), mask, ScaleMode.StretchToFill, true);
    }

    /// <summary>Screen space runs -1..1 both ways. Each lid's edge is an arc pinned at the eye's
    /// corners (x = ±cornerReach) that lifts further in the middle as the eye opens. The soft edges
    /// overlap when shut, so openness 0 is solid black.</summary>
    private void RebuildMask(float open)
    {
        builtFor = open;
        // Opens a readable slit early; 5.5 clears even the corners at full open.
        float lift = Mathf.Pow(open, 1.5f) * 5.5f;
        float soft = edgeSoftness;
        float dim = drowsyDim * (1f - open);
        float reach2 = cornerReach * cornerReach;

        for (int y = 0; y < MaskHeight; y++)
        {
            // Texture row 0 is the bottom of the screen.
            float ny = (y + 0.5f) / MaskHeight * 2f - 1f;
            for (int x = 0; x < MaskWidth; x++)
            {
                float nx = (x + 0.5f) / MaskWidth * 2f - 1f;
                float arc = 1f - nx * nx / reach2;
                float top = lift * arc - soft;
                float bottom = -(lift * lowerLidTravel * arc - soft);

                float upperLid = Ease(top - soft, top + soft, ny);
                float lowerLid = 1f - Ease(bottom - soft, bottom + soft, ny);
                float lid = Mathf.Max(upperLid, lowerLid);
                float alpha = 1f - (1f - lid) * (1f - dim);

                pixels[y * MaskWidth + x] = new Color32(0, 0, 0, (byte)(Mathf.Clamp01(alpha) * 255f + 0.5f));
            }
        }

        mask.SetPixels32(pixels);
        mask.Apply(false);
    }

    /// <summary>GLSL-style smoothstep (Mathf.SmoothStep is an eased lerp, not this).</summary>
    private static float Ease(float edge0, float edge1, float x)
    {
        float t = Mathf.Clamp01((x - edge0) / (edge1 - edge0));
        return t * t * (3f - 2f * t);
    }
}
