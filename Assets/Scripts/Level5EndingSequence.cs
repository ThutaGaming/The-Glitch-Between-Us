using System.Collections;
using System.Collections.Generic;
using InfimaGames.LowPolyShooterPack;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Level 5's ending, after the Spider-Mech falls:
///   1. The two exit doors slide open and stay open.
///   2. Mission "Turn on the switch to rescue the girl" - CR_Lab2_Computer_01 glows blue; E on it
///      fades out the green glass of CR_Lab2_Capsule_01 (and swaps its collider so she can be reached).
///   3. Mission "Talk to the girl" - E on her plays the Thanks animation once while she thanks Thuta.
///   4. The screen glitches the same way the bedroom computer did, then fades to black.
/// Mission/dialogue text is English on purpose: IMGUI can't shape Burmese.
/// </summary>
public class Level5EndingSequence : MonoBehaviour
{
    private enum Stage { WaitingForBoss, Switch, Talk, Busy, Done }

    [Header("Trigger")]
    [SerializeField] private BossAIController boss;
    [SerializeField] private BossEncounter encounter;
    [SerializeField] private SpiderMechFxLibrary fx;

    [Header("1. Exit doors (slide on X, stay open)")]
    [SerializeField] private Transform doorRight;
    [SerializeField] private float doorRightOpenX = 354.599f;
    [SerializeField] private Transform doorLeft;
    [SerializeField] private float doorLeftOpenX = 360.87f;
    [SerializeField] private float doorOpenDelay = 0.5f;
    [SerializeField] private float doorOpenDuration = 1.4f;

    [Header("2. Capsule switch")]
    [SerializeField] private string switchObjective = "Turn on the switch to rescue the girl";
    [SerializeField] private Transform computer;
    [SerializeField] private ObjectiveGlow computerGlow;
    [SerializeField] private string computerPrompt = "[E]  TURN ON THE SWITCH";
    [SerializeField] private MeshRenderer capsuleRenderer;
    [Tooltip("Material slot of the green glass on the capsule mesh.")]
    [SerializeField] private int glassMaterialIndex = 1;
    [SerializeField] private MeshCollider capsuleCollider;
    [Tooltip("The capsule's collision without the glass, so the player can walk up to her.")]
    [SerializeField] private Mesh capsuleOpenCollider;
    [SerializeField] private float glassFadeDuration = 1.4f;

    [Header("3. Talk to the girl")]
    [SerializeField] private string talkObjective = "Talk to the girl";
    [SerializeField] private Animator girl;
    [SerializeField] private ObjectiveGlow girlGlow;
    [SerializeField] private string girlPrompt = "[E]  TALK";
    [SerializeField] private string girlName = "Girl";
    [SerializeField] private string thanksLine = "Thank you so much for saving me.";
    [SerializeField] private string thanksState = "Thanks";
    [Tooltip("FinalGirl's Mixamo rig has its left/right labels swapped, so once a humanoid clip drives " +
             "her the body ends up turned around. This yaw puts her back the way the model faces in the editor.")]
    [SerializeField] private float girlFacingOffset = 180f;

    [Header("Interaction")]
    [SerializeField] private float interactRange = 2.4f;
    [Tooltip("How far off-centre (degrees) the target may be and still count as looked at.")]
    [SerializeField] private float interactAngle = 55f;

    [Header("4. Glitch (same look as the bedroom computer)")]
    [SerializeField] private float glitchDelay = 0.6f;
    [SerializeField] private float glitchBuildDuration = 2.6f;
    [SerializeField] private float glitchLineDelay = 0.9f;
    [SerializeField] private string playerName = "Thuta";
    [SerializeField] private string glitchLine = "Wait... not again?!";
    [SerializeField] private float glitchLineHold = 1.2f;
    [SerializeField] private float blackoutFadeDuration = 0.9f;
    [Range(2, 40)]
    [SerializeField] private int glitchStreakCount = 24;
    [Tooltip("Once the screen is black, go back to the bedroom: Thuta wakes up slumped over his desk " +
             "(DeskWakeUpSequence) instead of the normal morning intro.")]
    [SerializeField] private bool wakeAtBedroomDesk = true;
    [SerializeField] private string bedroomSceneName = "Bedroom Scene";
    [Tooltip("Used only when wakeAtBedroomDesk is off: scene to load once the screen is black. Leave empty to stay on black.")]
    [SerializeField] private string nextSceneName = "";
    public UnityEvent onEndingFinished;

    private static readonly Color[] GlitchStreakColors =
    {
        Color.white, new Color(0.45f, 0.90f, 1f, 1f), new Color(1f, 0.30f, 0.80f, 1f), new Color(0.55f, 0.70f, 1f, 1f),
    };

    private Stage stage = Stage.WaitingForBoss;
    private Transform player;
    private Rigidbody playerBody;
    private Camera playerCamera;
    private readonly List<Behaviour> frozenControls = new List<Behaviour>();

    private bool glitching;
    private float glitchIntensity;
    private float blackoutAlpha;
    private AudioSource glitchAudio;
    private Texture2D solid;
    private GUIStyle promptStyle;

    public bool IsFinished => stage == Stage.Done;

    private void Awake()
    {
        solid = new Texture2D(1, 1, TextureFormat.RGBA32, false) { hideFlags = HideFlags.HideAndDontSave };
        solid.SetPixel(0, 0, Color.white);
        solid.Apply();

        if (girl != null)
        {
            // Her clip is authored in place; root motion would only let her drift out of the capsule.
            girl.applyRootMotion = false;
            girl.transform.rotation *= Quaternion.Euler(0f, girlFacingOffset, 0f);
        }
    }

    private void Start()
    {
        var go = GameObject.FindGameObjectWithTag("Player");
        if (go != null)
        {
            player = go.transform;
            playerBody = go.GetComponent<Rigidbody>();
        }

        if (boss != null) boss.Defeated += OnBossDefeated;
        if (encounter != null) encounter.VictoryFlowFinished += OnVictoryFlowFinished;
    }

    private void OnDestroy()
    {
        if (boss != null) boss.Defeated -= OnBossDefeated;
        if (encounter != null) encounter.VictoryFlowFinished -= OnVictoryFlowFinished;
        if (solid != null) Destroy(solid);
    }

    // ---------------------------------------------------------------- 1. doors

    private void OnBossDefeated()
    {
        StartCoroutine(OpenDoors());
        // Without an encounter nothing else announces the victory, so move straight on.
        if (encounter == null) StartCoroutine(BeginSwitchAfter(6f));
    }

    private IEnumerator OpenDoors()
    {
        yield return new WaitForSeconds(doorOpenDelay);

        var doors = new List<(Transform door, float fromX, float toX)>();
        if (doorRight != null) doors.Add((doorRight, doorRight.position.x, doorRightOpenX));
        if (doorLeft != null) doors.Add((doorLeft, doorLeft.position.x, doorLeftOpenX));
        foreach (var d in doors)
        {
            var sound = d.door.GetComponent<AudioSource>();
            if (sound != null && sound.clip != null) sound.Play();
        }

        float t = 0f;
        while (t < doorOpenDuration)
        {
            t += Time.deltaTime;
            float k = Mathf.SmoothStep(0f, 1f, t / doorOpenDuration);
            foreach (var d in doors) SetX(d.door, Mathf.Lerp(d.fromX, d.toX, k));
            yield return null;
        }
        foreach (var d in doors) SetX(d.door, d.toX);
    }

    private static void SetX(Transform t, float x)
    {
        var p = t.position;
        t.position = new Vector3(x, p.y, p.z);
    }

    // ---------------------------------------------------------------- 2. switch

    private void OnVictoryFlowFinished() => StartCoroutine(BeginSwitchAfter(0.8f));

    private IEnumerator BeginSwitchAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (stage != Stage.WaitingForBoss) yield break;

        if (MissionHUD.Instance != null) MissionHUD.Instance.SetObjective(switchObjective);
        if (computerGlow != null) computerGlow.SetGlowing(true);
        stage = Stage.Switch;
    }

    private void Update()
    {
        bool pressed = Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame;
        if (!pressed) return;

        if (stage == Stage.Switch && IsLookingAt(ComputerPoint)) StartCoroutine(ActivateSwitch());
        else if (stage == Stage.Talk && IsLookingAt(GirlPoint)) StartCoroutine(TalkToGirl());
    }

    private IEnumerator ActivateSwitch()
    {
        stage = Stage.Busy;
        if (computerGlow != null) computerGlow.SetGlowing(false);
        if (MissionHUD.Instance != null) MissionHUD.Instance.CompleteObjective();
        if (fx != null) BossFx.Sfx(fx.powerUp, ComputerPoint, 0.9f, 1.3f, 2f, 25f);

        yield return OpenCapsule();

        yield return new WaitForSeconds(0.8f);
        if (MissionHUD.Instance != null) MissionHUD.Instance.SetObjective(talkObjective);
        if (girlGlow != null) girlGlow.SetGlowing(true);
        stage = Stage.Talk;
    }

    /// <summary>Hisses, vents a little steam and fades the green glass out, then stops drawing
    /// that submesh altogether and swaps in a collider without it.</summary>
    private IEnumerator OpenCapsule()
    {
        if (capsuleRenderer == null) yield break;

        Vector3 center = capsuleRenderer.bounds.center;
        if (fx != null)
        {
            BossFx.Sfx(fx.barrierClip, center, 1f, 1.1f, 2f, 30f);
            BossFx.Play(fx.stunSmoke, center - Vector3.up * 1.2f, Quaternion.identity, 0.7f, 0.8f);
        }
        BossFx.Flash(center, new Color(0.35f, 1f, 0.55f), 4f, 6f, 0.8f);

        var shared = capsuleRenderer.sharedMaterials;
        if (glassMaterialIndex < 0 || glassMaterialIndex >= shared.Length) yield break;

        // Fade a private copy so the shared glass material (also used by the computer) is untouched.
        var glass = new Material(shared[glassMaterialIndex]);
        var working = (Material[])shared.Clone();
        working[glassMaterialIndex] = glass;
        capsuleRenderer.sharedMaterials = working;

        int baseColorId = Shader.PropertyToID("_BaseColor");
        int emissionId = Shader.PropertyToID("_EmissionColor");
        Color baseColor = glass.HasProperty(baseColorId) ? glass.GetColor(baseColorId) : Color.white;
        Color emission = glass.HasProperty(emissionId) ? glass.GetColor(emissionId) : Color.black;

        float t = 0f;
        while (t < glassFadeDuration)
        {
            t += Time.deltaTime;
            float k = 1f - Mathf.Clamp01(t / glassFadeDuration);
            if (glass.HasProperty(baseColorId)) glass.SetColor(baseColorId, new Color(baseColor.r, baseColor.g, baseColor.b, baseColor.a * k));
            if (glass.HasProperty(emissionId)) glass.SetColor(emissionId, emission * k);
            yield return null;
        }

        // Fewer materials than submeshes = the glass submesh simply isn't drawn any more.
        var remaining = new List<Material>();
        for (int i = 0; i < shared.Length; i++) if (i != glassMaterialIndex) remaining.Add(shared[i]);
        capsuleRenderer.sharedMaterials = remaining.ToArray();
        Destroy(glass);

        if (capsuleCollider != null && capsuleOpenCollider != null) capsuleCollider.sharedMesh = capsuleOpenCollider;
    }

    // ---------------------------------------------------------------- 3. talk

    private IEnumerator TalkToGirl()
    {
        stage = Stage.Busy;
        if (girlGlow != null) girlGlow.SetGlowing(false);
        if (MissionHUD.Instance != null) MissionHUD.Instance.CompleteObjective();
        FreezePlayer();

        // Turn to face Thuta before thanking him.
        if (girl != null && player != null)
        {
            Vector3 to = player.position - girl.transform.position;
            to.y = 0f;
            if (to.sqrMagnitude > 0.01f)
            {
                Quaternion from = girl.transform.rotation;
                Quaternion target = Quaternion.LookRotation(to.normalized, Vector3.up) * Quaternion.Euler(0f, girlFacingOffset, 0f);
                for (float t = 0f; t < 0.5f; t += Time.deltaTime)
                {
                    girl.transform.rotation = Quaternion.Slerp(from, target, Mathf.SmoothStep(0f, 1f, t / 0.5f));
                    yield return null;
                }
                girl.transform.rotation = target;
            }
        }

        float clipLength = 3f;
        if (girl != null && girl.runtimeAnimatorController != null)
        {
            girl.CrossFadeInFixedTime(thanksState, 0.2f);
            foreach (var clip in girl.runtimeAnimatorController.animationClips)
                if (clip.name == thanksState) clipLength = clip.length;
        }

        var dialogue = DialogueHUD.Instance;
        float started = Time.time;
        if (dialogue != null)
        {
            dialogue.Say(girlName, thanksLine, 2.2f);
            yield return null;
        }
        yield return new WaitUntil(() => Time.time - started >= clipLength && (dialogue == null || !dialogue.IsPlaying));

        yield return new WaitForSeconds(glitchDelay);
        yield return GlitchRoutine();
    }

    // ---------------------------------------------------------------- 4. glitch

    /// <summary>The bedroom computer's glitch, over the 3D view this time: streaks and corruption
    /// blocks that build up, a strobing flash, camera shake and garbled computer noise - Thuta's
    /// line partway through - then a hard fade to black.</summary>
    private IEnumerator GlitchRoutine()
    {
        glitching = true;
        StartGlitchAudio();
        if (PlayerCamera != null) CameraShake.Ensure(PlayerCamera.transform);

        var dialogue = DialogueHUD.Instance;
        bool linePlayed = false;
        float t = 0f;
        while (t < glitchBuildDuration)
        {
            t += Time.deltaTime;
            glitchIntensity = Mathf.Clamp01(t / glitchBuildDuration);
            TickGlitchFeedback();

            if (!linePlayed && t >= glitchLineDelay)
            {
                linePlayed = true;
                if (dialogue != null) dialogue.Say(playerName, glitchLine, glitchLineHold);
            }
            yield return null;
        }

        yield return null;
        while (dialogue != null && dialogue.IsPlaying)
        {
            TickGlitchFeedback();
            yield return null;
        }

        t = 0f;
        while (t < blackoutFadeDuration)
        {
            t += Time.deltaTime;
            blackoutAlpha = Mathf.Clamp01(t / blackoutFadeDuration);
            if (glitchAudio != null) glitchAudio.volume = 0.8f * (1f - blackoutAlpha);
            TickGlitchFeedback();
            yield return null;
        }
        blackoutAlpha = 1f;
        glitching = false;
        if (glitchAudio != null) glitchAudio.Stop();

        stage = Stage.Done;
        onEndingFinished?.Invoke();
        if (wakeAtBedroomDesk && !string.IsNullOrEmpty(bedroomSceneName))
        {
            yield return new WaitForSeconds(0.8f);
            DeskWakeUpSequence.QueueForNextLoad();
            SceneManager.LoadScene(bedroomSceneName);
        }
        else if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private void StartGlitchAudio()
    {
        if (fx == null || fx.chargeLoop == null) return;
        glitchAudio = gameObject.AddComponent<AudioSource>();
        glitchAudio.clip = fx.chargeLoop;
        glitchAudio.loop = true;
        glitchAudio.spatialBlend = 0f;
        glitchAudio.volume = 0.8f;
        glitchAudio.Play();
    }

    private void TickGlitchFeedback()
    {
        // Garbled, stuttering computer noise that gets worse with the glitch.
        if (glitchAudio != null) glitchAudio.pitch = Random.Range(0.5f, 0.8f + 1.8f * glitchIntensity);
        if (fx != null && Random.value < 0.08f * glitchIntensity && player != null)
            BossFx.Sfx(BossFx.Pick(fx.boltImpacts), player.position, 0.5f, Random.Range(1.4f, 2.2f), 1f, 10f, 0f);

        var cam = PlayerCamera;
        if (cam != null) CameraShake.Shake(cam.transform.position, 2.5f * glitchIntensity * Time.deltaTime, 0f);
    }

    // ---------------------------------------------------------------- helpers

    private void FreezePlayer()
    {
        if (player == null) return;
        foreach (var b in new Behaviour[] { player.GetComponent<Movement>(), player.GetComponent<CameraLook>(), player.GetComponent<PlayerInput>() })
        {
            if (b == null || !b.enabled) continue;
            b.enabled = false;
            frozenControls.Add(b);
        }
        if (playerBody != null) playerBody.linearVelocity = Vector3.zero;
    }

    private Camera PlayerCamera
    {
        get
        {
            if (playerCamera == null || !playerCamera.isActiveAndEnabled) playerCamera = Camera.main;
            return playerCamera;
        }
    }

    private Vector3 ComputerPoint => computer != null ? BoundsCenter(computer) : transform.position;

    private Vector3 GirlPoint => girl != null ? BoundsCenter(girl.transform) : transform.position;

    private static Vector3 BoundsCenter(Transform root)
    {
        var renderers = root.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return root.position;
        Bounds b = renderers[0].bounds;
        foreach (var r in renderers) b.Encapsulate(r.bounds);
        return b.center;
    }

    private bool IsLookingAt(Vector3 point)
    {
        var cam = PlayerCamera;
        if (cam == null) return false;
        Vector3 to = point - cam.transform.position;
        Vector3 flat = new Vector3(to.x, 0f, to.z);
        if (flat.magnitude > interactRange || Mathf.Abs(to.y) > 1.8f) return false;
        return Vector3.Angle(cam.transform.forward, to) <= interactAngle;
    }

    private void OnGUI()
    {
        GUI.depth = -100;

        if (stage == Stage.Switch || stage == Stage.Talk)
        {
            bool near = stage == Stage.Switch ? IsLookingAt(ComputerPoint) : IsLookingAt(GirlPoint);
            if (near)
            {
                if (promptStyle == null)
                {
                    promptStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 20, fontStyle = FontStyle.Bold };
                    promptStyle.normal.textColor = new Color(0.2f, 0.95f, 1f, 1f);
                }
                GUI.Label(new Rect(Screen.width * 0.5f - 190f, Screen.height * 0.62f, 380f, 34f),
                    stage == Stage.Switch ? computerPrompt : girlPrompt, promptStyle);
            }
        }

        if (glitching) DrawGlitch();

        if (blackoutAlpha > 0.001f)
        {
            GUI.color = new Color(0f, 0f, 0f, blackoutAlpha);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), solid);
            GUI.color = Color.white;
        }
    }

    /// <summary>Same recipe as ComputerUseSequence.DrawGlitchStreaks, plus a flickering colour
    /// split tint, since here it sits over the live 3D view rather than the illustrated monitor.</summary>
    private void DrawGlitch()
    {
        int streaks = Mathf.RoundToInt(Mathf.Lerp(3, glitchStreakCount, glitchIntensity));
        for (int i = 0; i < streaks; i++)
        {
            bool chunky = Random.value < 0.35f;
            float rw = chunky ? Mathf.Lerp(60f, 220f, Random.value) : Mathf.Lerp(60f, 320f, Random.value);
            float rh = chunky ? Mathf.Lerp(30f, 140f, Random.value) : Mathf.Lerp(3f, 16f, Random.value);
            Color c = GlitchStreakColors[Random.Range(0, GlitchStreakColors.Length)];
            GUI.color = new Color(c.r, c.g, c.b, Mathf.Lerp(0.35f, 1f, glitchIntensity) * (chunky ? 0.7f : 1f));
            GUI.DrawTexture(new Rect(Random.value * Screen.width, Random.value * Screen.height, rw, rh), solid);
        }

        // Full-width tear bands, like scanlines slipping.
        int bands = Mathf.RoundToInt(4 * glitchIntensity);
        for (int i = 0; i < bands; i++)
        {
            GUI.color = new Color(0f, 0f, 0f, 0.55f * glitchIntensity);
            GUI.DrawTexture(new Rect(0f, Random.value * Screen.height, Screen.width, Mathf.Lerp(2f, 10f, Random.value)), solid);
        }

        if (Random.value < 0.5f)
        {
            Color split = Random.value < 0.5f ? new Color(1f, 0.1f, 0.6f) : new Color(0.1f, 0.9f, 1f);
            GUI.color = new Color(split.r, split.g, split.b, 0.12f * glitchIntensity);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), solid);
        }

        float flash = (Mathf.Sin(Time.unscaledTime * 55f) + 1f) * 0.5f;
        GUI.color = new Color(1f, 1f, 1f, flash * glitchIntensity * 0.25f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), solid);
        GUI.color = Color.white;
    }
}
