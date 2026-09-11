using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NetworkPuzzleTerminal))]
public class TerminalInteractionBridge : MonoBehaviour
{
    [SerializeField] float activationDistance = 3.5f;
    NetworkPuzzleTerminal terminal;
    Camera playerCamera;

    void Awake()
    {
        terminal = GetComponent<NetworkPuzzleTerminal>();
    }

    Camera FindPlayerCamera()
    {
        if (playerCamera != null && playerCamera.isActiveAndEnabled) return playerCamera;
        foreach (var camera in Camera.allCameras)
        {
            if (camera.isActiveAndEnabled && camera.cameraType == CameraType.Game)
            {
                playerCamera = camera;
                return camera;
            }
        }
        return null;
    }

    void Update()
    {
        var cam = FindPlayerCamera();
        if (cam == null || terminal == null) return;

        if (IsPlayerNear(cam) && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            terminal.Interact(cam.gameObject);
    }

    bool IsPlayerNear(Camera cam)
    {
        Vector3 from = cam.transform.position;
        Vector3 to = transform.position;
        from.y = 0f;
        to.y = 0f;
        return Vector3.Distance(from, to) <= activationDistance;
    }

    void OnGUI()
    {
        var cam = FindPlayerCamera();
        if (cam == null || !IsPlayerNear(cam)) return;

        var style = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 20,
            fontStyle = FontStyle.Bold
        };
        style.normal.textColor = new Color(0.2f, 0.95f, 1f, 1f);
        GUI.Label(new Rect(Screen.width * 0.5f - 190f, Screen.height * 0.64f, 380f, 34f),
            "[E]  ACCESS NETWORK TERMINAL", style);
    }
}