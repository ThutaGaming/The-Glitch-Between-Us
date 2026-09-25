using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// The game's last scene, on the late morning after Level 5: once Thuta sits down in class
/// (LateSchoolArrivalSequence.onSeated), Tr Htet Htet walks in through BarnDoor_03 with a new
/// student behind her and introduces Mone Mone. Mone Mone bows and greets the class, is sent to
/// the empty seat on Thuta's left, walks round behind him to it and sits, says hi, and thanks
/// him for saving her last night - she is the girl from Level 5. Thuta, who has never seen her
/// before, asks who she is, and the game ends on that.
///
/// The player's view is driven like ThihaTeachingSequence does it: yaw on the body, pitch on the
/// camera, so nothing snaps if control were handed back.
/// Lines are English because Unity's IMGUI/TextMeshPro cannot shape Burmese correctly.
/// </summary>
public class NewStudentCutscene : MonoBehaviour
{
    [Header("Cast")]
    [SerializeField] private CutsceneActor teacher;
    [SerializeField] private CutsceneActor newStudent;

    [Header("Marks (on the floor; their forward is the facing)")]
    [Tooltip("Where both appear, in the corridor outside BarnDoor_03.")]
    [SerializeField] private Transform entryMark;
    [Tooltip("The doorway itself - walked through on the way in.")]
    [SerializeField] private Transform doorwayMark;
    [SerializeField] private Transform teacherMark;
    [SerializeField] private Transform studentMark;
    [Tooltip("Mone Mone's way from her mark round behind Thuta to the side of the chair on his left.")]
    [SerializeField] private Transform[] pathToSeat;
    [Tooltip("Under her hips on (Prb)Chair2 (07).")]
    [SerializeField] private Transform seatMark;
    [SerializeField] private BarnDoorSlider classDoor;
    [SerializeField] private float studentFollowDelay = 1.3f;

    [Header("Player")]
    [SerializeField] private SittableChair playerChair;
    [SerializeField] private Transform playerBody;
    [SerializeField] private Transform playerCamera;
    [Tooltip("Off for the whole cutscene - the game ends at the end of it.")]
    [SerializeField] private Behaviour[] disableDuringCutscene;
    [SerializeField] private float cameraTurnDegreesPerSecond = 90f;
    [Tooltip("The quicker turn to Mone Mone when she says hi, so her wave is still going when he sees her.")]
    [SerializeField] private float turnToStudentDegreesPerSecond = 170f;
    [Tooltip("How long she keeps waving after her \"Hi!\" line.")]
    [SerializeField] private float waveHoldAfterHi = 0.8f;

    [Header("Dialogue")]
    [SerializeField] private DialogueHUD dialogue;
    [SerializeField] private string teacherName = "Tr Htet Htet";
    [SerializeField] private string studentName = "Mone Mone";
    [SerializeField] private string introLine = "Everyone, this is our new student. Her name is Mone Mone.";
    [SerializeField] private string greetLine = "Hello, everyone. I'm Mone Mone.";
    [SerializeField] private string niceToMeetLine = "Nice to meet you all.";
    [SerializeField] private string seatLine = "Mone Mone, please take the seat next to Thuta.";
    [SerializeField] private string hiLine = "Hi!";
    [SerializeField] private string thanksLine = "Thank you for saving me last night.";
    [Tooltip("Thuta's reply - the last line of the game.")]
    [SerializeField] private string playerName = "Thuta";
    [SerializeField] private string whoLine = "Who are you?";
    [Tooltip("Silence between her thank-you and his question.")]
    [SerializeField] private float pauseBeforeWho = 0.8f;
    [SerializeField] private float holdPerLine = 2.2f;

    [Header("Ending")]
    [SerializeField] private float holdBeforeEnd = 1.5f;
    [SerializeField] private float fadeDuration = 2.5f;
    [SerializeField] private string endTitle = "THE END";
    [SerializeField] private string endSubtitle = "Thank you for playing";
    [SerializeField] private float endHold = 5f;
    [Tooltip("Loaded after the end card. Leave empty to stay on it.")]
    [SerializeField] private string endSceneName = "Mainmenu Scene";

    /// <summary>Fires when the end card has been up for endHold seconds, before endSceneName loads.</summary>
    public UnityEvent onFinished;

    private bool started;
    private Func<Vector3> view;
    private float viewTurnSpeed;

    private float overlayAlpha;
    private float titleAlpha;
    private Texture2D solid;
    private GUIStyle titleStyle;
    private GUIStyle subtitleStyle;

    private void Awake()
    {
        solid = new Texture2D(1, 1, TextureFormat.RGBA32, false) { hideFlags = HideFlags.HideAndDontSave };
        solid.SetPixel(0, 0, Color.white);
        solid.Apply();

        // Nobody is in front of the class until the cutscene brings them in.
        if (teacher != null) teacher.gameObject.SetActive(false);
        if (newStudent != null) newStudent.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (solid != null) Destroy(solid);
    }

    /// <summary>Plays the cutscene once. Hook to LateSchoolArrivalSequence.onSeated.</summary>
    public void Begin()
    {
        if (started) return;
        started = true;
        viewTurnSpeed = cameraTurnDegreesPerSecond;
        StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        if (dialogue == null) dialogue = DialogueHUD.Instance != null ? DialogueHUD.Instance : FindFirstObjectByType<DialogueHUD>();
        if (playerChair != null) playerChair.SetStandUpBlocked(true);
        foreach (var b in disableDuringCutscene)
        {
            if (b != null) b.enabled = false;
        }
        if (playerCamera != null) playerCamera.localRotation = Quaternion.identity;

        if (classDoor != null) classDoor.Open();
        yield return new WaitForSeconds(1f);

        // In they come: the teacher first, the new student a few steps behind.
        Vector3 firstStop = doorwayMark != null ? doorwayMark.position : teacherMark.position;
        teacher.gameObject.SetActive(true);
        teacher.Place(entryMark.position, YawTowards(entryMark.position, firstStop));
        view = () => teacher.Head.position;
        Coroutine teacherIn = StartCoroutine(WalkInAndFaceClass(teacher, teacherMark, "Idle"));

        yield return new WaitForSeconds(studentFollowDelay);
        newStudent.gameObject.SetActive(true);
        newStudent.Place(entryMark.position, YawTowards(entryMark.position, firstStop));
        yield return WalkInAndFaceClass(newStudent, studentMark, "HandsFront");
        yield return teacherIn;

        view = () => Vector3.Lerp(teacher.Head.position, newStudent.Head.position, 0.5f);
        yield return new WaitForSeconds(0.6f);

        // "This is our new student..." with an open hand toward her.
        teacher.Play("Point", 0.35f);
        teacher.LookAt(newStudent.Head, 0.25f);
        teacher.Reach(newStudent.Chest);
        yield return Say(teacherName, introLine);
        teacher.StopReaching();
        teacher.Play("Idle", 0.45f);
        teacher.LookAt(playerCamera, 0.15f);

        // Mone Mone bows and greets the class.
        view = () => newStudent.Head.position;
        newStudent.StopLooking();
        newStudent.Play("Bow", 0.3f);
        dialogue.Say(studentName, greetLine, holdPerLine);
        yield return new WaitForSeconds(2.3f);
        newStudent.Play("HandsFront", 0.35f);
        newStudent.LookAt(playerCamera, 0.1f);
        yield return WaitForDialogue();
        yield return Say(studentName, niceToMeetLine);

        // The teacher points her to the empty seat next to Thuta.
        view = () => teacher.Head.position;
        teacher.Play("Point", 0.35f);
        teacher.LookAt(newStudent.Head, 0.2f);
        teacher.Reach(seatMark.position + Vector3.up * 0.9f);
        yield return Say(teacherName, seatLine);
        teacher.StopReaching();
        teacher.Play("Idle", 0.45f);

        // She goes round behind Thuta to the seat on his left; he keeps watching the teacher,
        // who watches her go.
        teacher.LookAt(newStudent.Head, 0.15f);
        newStudent.StopLooking();
        var path = new List<Vector3>();
        foreach (var p in pathToSeat) path.Add(p.position);
        yield return newStudent.WalkPath(path, "Idle");
        yield return newStudent.TurnTo(seatMark.eulerAngles.y);
        newStudent.Play("SitIdle", 0.9f);
        yield return newStudent.Slide(seatMark.position, 0.9f);
        teacher.LookAt(playerCamera, 0.15f);
        yield return new WaitForSeconds(0.8f);

        // "Hi!" - and Thuta turns to her. Only her head turns for the wave: any body turn swings
        // the raised hand round in front of her face.
        newStudent.LookAt(playerCamera, 0f);
        newStudent.Play("SitWave", 0.3f);
        dialogue.Say(studentName, hiLine, holdPerLine);
        yield return new WaitForSeconds(0.3f);
        viewTurnSpeed = turnToStudentDegreesPerSecond;
        view = () => newStudent.Head.position;
        yield return WaitForDialogue();
        yield return new WaitForSeconds(waveHoldAfterHi);

        newStudent.Play("SitThanks", 0.4f);
        newStudent.LookAt(playerCamera, 0.35f);
        yield return Say(studentName, thanksLine);

        // She lowers her hands and waits - and Thuta has no idea who she is.
        newStudent.Play("SitIdle", 0.6f);
        yield return new WaitForSeconds(pauseBeforeWho);
        yield return Say(playerName, whoLine);
        yield return new WaitForSeconds(holdBeforeEnd);

        yield return EndGame();
    }

    private IEnumerator WalkInAndFaceClass(CutsceneActor actor, Transform mark, string arriveState)
    {
        var path = doorwayMark != null ? new[] { doorwayMark.position, mark.position } : new[] { mark.position };
        yield return actor.WalkPath(path, arriveState);
        yield return actor.TurnTo(mark.eulerAngles.y);
        actor.LookAt(playerCamera, 0.1f);
    }

    private IEnumerator Say(string speaker, string line)
    {
        dialogue.Say(speaker, line, holdPerLine);
        yield return WaitForDialogue();
    }

    private IEnumerator WaitForDialogue()
    {
        // Say() only starts its queue at the end of the frame, so IsPlaying is still false now.
        yield return null;
        yield return new WaitWhile(() => dialogue.IsPlaying);
    }

    private IEnumerator EndGame()
    {
        for (float t = 0f; t < fadeDuration; t += Time.deltaTime)
        {
            overlayAlpha = t / fadeDuration;
            yield return null;
        }
        overlayAlpha = 1f;

        for (float t = 0f; t < 1.2f; t += Time.deltaTime)
        {
            titleAlpha = t / 1.2f;
            yield return null;
        }
        titleAlpha = 1f;
        yield return new WaitForSeconds(endHold);

        onFinished?.Invoke();
        if (!string.IsNullOrEmpty(endSceneName))
        {
            // MouseLook locked the cursor; the menu needs it back.
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene(endSceneName);
        }
    }

    private void LateUpdate()
    {
        if (view != null && playerBody != null && playerCamera != null) AimPlayerViewAt(view());
    }

    /// <summary>Yaw on the body and pitch on the camera - the same split MouseLook uses.</summary>
    private void AimPlayerViewAt(Vector3 worldPoint)
    {
        Vector3 toTarget = worldPoint - playerCamera.position;
        Vector3 flat = new Vector3(toTarget.x, 0f, toTarget.z);
        if (flat.sqrMagnitude < 0.0001f) return;

        float maxStep = viewTurnSpeed * Time.deltaTime;
        playerBody.rotation = Quaternion.RotateTowards(playerBody.rotation,
            Quaternion.LookRotation(flat.normalized, Vector3.up), maxStep);

        float targetPitch = -Mathf.Atan2(toTarget.y, flat.magnitude) * Mathf.Rad2Deg;
        float currentPitch = playerCamera.localEulerAngles.x;
        if (currentPitch > 180f) currentPitch -= 360f;
        playerCamera.localRotation = Quaternion.Euler(Mathf.MoveTowards(currentPitch, targetPitch, maxStep), 0f, 0f);
    }

    private static float YawTowards(Vector3 from, Vector3 to)
    {
        Vector3 d = to - from;
        return Mathf.Atan2(d.x, d.z) * Mathf.Rad2Deg;
    }

    private void OnGUI()
    {
        if (overlayAlpha <= 0.001f) return;

        GUI.depth = -100;
        Color previous = GUI.color;
        GUI.color = new Color(0f, 0f, 0f, overlayAlpha);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), solid);

        if (titleAlpha > 0.001f)
        {
            if (titleStyle == null)
            {
                titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 56, alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };
                titleStyle.normal.textColor = Color.white;
                subtitleStyle = new GUIStyle(GUI.skin.label) { fontSize = 24, alignment = TextAnchor.MiddleCenter };
                subtitleStyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f);
            }
            GUI.color = new Color(1f, 1f, 1f, titleAlpha);
            GUI.Label(new Rect(0, Screen.height * 0.5f - 70f, Screen.width, 80f), endTitle, titleStyle);
            GUI.Label(new Rect(0, Screen.height * 0.5f + 10f, Screen.width, 40f), endSubtitle, subtitleStyle);
        }
        GUI.color = previous;
    }
}
