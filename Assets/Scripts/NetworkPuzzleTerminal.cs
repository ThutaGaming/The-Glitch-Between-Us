using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using InfimaGames.LowPolyShooterPack;

/// <summary>
/// Room 3's "restore the security network" puzzle: player connects four device nodes in the
/// order SERVER -> ROUTER -> FIREWALL -> DOOR CONTROL via a simple click-two-nodes OnGUI screen
/// (mirrors EscapeProtocolHologram's flicker-panel styling). On success fires
/// <see cref="onNetworkRestored"/>, which BlastDoorController listens to in order to swap the
/// emergency lighting and slide the blast door open.
/// </summary>
public class NetworkPuzzleTerminal : MonoBehaviour, IInteractable
{
    private static readonly string[] Devices = { "SERVER", "ROUTER", "FIREWALL", "DOOR CONTROL" };

    [SerializeField] private string prompt = "(E) Access Terminal";
    public UnityEvent onNetworkRestored = new UnityEvent();

    private bool solved;
    private bool active;
    private int progress;
    private int selected = -1;

    private string overlayMessage;
    private Color overlayColor;
    private float overlayTimer;

    private GameObject player;
    private Movement movement;
    private CameraLook cameraLook;
    private PlayerInput playerInput;

    public Transform InteractTransform => solved || active ? null : transform;
    public string GetPrompt() => prompt;

    private void OnEnable() => InteractableRegistry.All.Add(this);
    private void OnDisable() => InteractableRegistry.All.Remove(this);

    public void Interact(GameObject playerGo)
    {
        if (solved || active) return;
        active = true;
        player = playerGo;

        movement = player.GetComponent<Movement>();
        cameraLook = player.GetComponent<CameraLook>();
        playerInput = player.GetComponent<PlayerInput>();
        SetFrozen(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void SetFrozen(bool frozen)
    {
        if (movement != null) movement.enabled = !frozen;
        if (cameraLook != null) cameraLook.enabled = !frozen;
        if (playerInput != null) playerInput.enabled = !frozen;
    }

    private void Update()
    {
        if (!active) return;

        if (overlayTimer > 0f)
        {
            overlayTimer -= Time.unscaledDeltaTime;
        }

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame && !solved)
        {
            Close();
        }
    }

    private void Close()
    {
        active = false;
        SetFrozen(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void TryConnect(int a, int b)
    {
        int expectedFrom = progress;
        int expectedTo = progress + 1;
        bool correct = (a == expectedFrom && b == expectedTo) || (a == expectedTo && b == expectedFrom);

        if (correct)
        {
            progress++;
            if (progress >= Devices.Length - 1)
                StartCoroutine(CompleteSequence());
        }
        else
        {
            ShowMessage("INVALID CONNECTION", new Color(1f, 0.25f, 0.2f), 1.1f);
        }
    }

    private void ShowMessage(string text, Color color, float duration)
    {
        overlayMessage = text;
        overlayColor = color;
        overlayTimer = duration;
    }

    private IEnumerator CompleteSequence()
    {
        yield return new WaitForSecondsRealtime(0.3f);
        ShowMessage("NETWORK RESTORED", new Color(0.3f, 0.85f, 1f), 1.4f);
        yield return new WaitForSecondsRealtime(1.4f);
        ShowMessage("ACCESS GRANTED", new Color(0.4f, 1f, 0.5f), 1.4f);
        yield return new WaitForSecondsRealtime(1.4f);

        solved = true;
        Close();
        onNetworkRestored?.Invoke();
    }

    #region GUI

    private Texture2D pixel;
    private GUIStyle boxStyle, titleStyle, hintStyle, deviceStyle, overlayStyle;

    private Texture2D Pixel
    {
        get
        {
            if (pixel == null)
            {
                pixel = new Texture2D(1, 1);
                pixel.SetPixel(0, 0, Color.white);
                pixel.Apply();
            }
            return pixel;
        }
    }

    private void DrawRect(Rect r, Color c)
    {
        var old = GUI.color;
        GUI.color = c;
        GUI.DrawTexture(r, Pixel);
        GUI.color = old;
    }

    private void OnGUI()
    {
        if (!active) return;

        if (titleStyle == null)
        {
            titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 26, alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };
            titleStyle.normal.textColor = new Color(0.3f, 0.85f, 1f);

            hintStyle = new GUIStyle(GUI.skin.label) { fontSize = 15, alignment = TextAnchor.MiddleCenter };
            hintStyle.normal.textColor = new Color(0.6f, 0.75f, 0.8f);

            deviceStyle = new GUIStyle(GUI.skin.button) { fontSize = 16, alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };

            overlayStyle = new GUIStyle(GUI.skin.label) { fontSize = 34, alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };
        }

        DrawRect(new Rect(0, 0, Screen.width, Screen.height), new Color(0f, 0f, 0f, 0.55f));

        float panelW = 720f, panelH = 420f;
        Rect panel = new Rect((Screen.width - panelW) * 0.5f, (Screen.height - panelH) * 0.5f, panelW, panelH);
        DrawRect(panel, new Color(0.02f, 0.05f, 0.07f, 0.92f));
        DrawRect(new Rect(panel.x, panel.y, panel.width, 3f), new Color(0.2f, 0.7f, 1f, 0.9f));
        DrawRect(new Rect(panel.x, panel.yMax - 3f, panel.width, 3f), new Color(0.2f, 0.7f, 1f, 0.9f));

        GUI.Label(new Rect(panel.x, panel.y + 20f, panel.width, 32f), "SECURITY NETWORK OFFLINE", titleStyle);
        GUI.Label(new Rect(panel.x, panel.y + 54f, panel.width, 24f), "RESTORE NETWORK CONNECTION", hintStyle);

        float rowY = panel.y + 130f;
        float btnW = 140f, btnH = 70f, gap = 30f;
        float totalW = Devices.Length * btnW + (Devices.Length - 1) * gap;
        float startX = panel.x + (panel.width - totalW) * 0.5f;

        for (int i = 0; i < Devices.Length; i++)
        {
            Rect btn = new Rect(startX + i * (btnW + gap), rowY, btnW, btnH);

            bool online = i <= progress;
            GUI.color = online ? new Color(0.35f, 1f, 0.55f) : new Color(1f, 0.35f, 0.3f);
            if (selected == i) GUI.color = new Color(0.3f, 0.85f, 1f);

            if (GUI.Button(btn, Devices[i] + "\n" + (online ? "ONLINE" : "OFFLINE"), deviceStyle))
            {
                if (selected == -1) selected = i;
                else if (selected == i) selected = -1;
                else { TryConnect(selected, i); selected = -1; }
            }
            GUI.color = Color.white;

            if (i < Devices.Length - 1)
            {
                Rect link = new Rect(btn.xMax, rowY + btnH * 0.5f - 2f, gap, 4f);
                DrawRect(link, i < progress ? new Color(0.3f, 0.85f, 1f) : new Color(0.4f, 0.15f, 0.15f));
            }
        }

        GUI.Label(new Rect(panel.x, panel.yMax - 70f, panel.width, 24f), "Restore the security network connection.", hintStyle);
        GUI.Label(new Rect(panel.x, panel.yMax - 44f, panel.width, 24f), "Click a device, then click the next device to connect them.  [Esc] to step back.", hintStyle);

        if (overlayTimer > 0f)
        {
            var c = overlayColor; c.a = Mathf.Clamp01(overlayTimer);
            var old = overlayStyle.normal.textColor;
            overlayStyle.normal.textColor = c;
            GUI.Label(new Rect(panel.x, panel.y + panel.height * 0.5f - 20f, panel.width, 40f), overlayMessage, overlayStyle);
            overlayStyle.normal.textColor = old;
        }
    }

    #endregion
}
