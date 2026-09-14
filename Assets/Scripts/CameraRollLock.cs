using UnityEngine;

/// <summary>
/// The FPS camera rides on the arms rig's head bone (SOCKET_Camera), so any roll that a
/// walk/idle/reload animation clip feeds into the spine/neck/head bones bleeds straight into
/// the camera's render rotation. Bind pose is level, but nothing re-levels it during playback.
/// This re-levels the camera every frame, after Animator + CameraLook have posed the rig, by
/// keeping its current look direction but forcing world-up as up - killing roll without
/// touching pitch/yaw.
/// </summary>
public class CameraRollLock : MonoBehaviour
{
    private void OnEnable() => Camera.onPreCull += HandlePreCull;
    private void OnDisable() => Camera.onPreCull -= HandlePreCull;

    private void HandlePreCull(Camera cam)
    {
        if (!Application.isPlaying) return;
        transform.rotation = Quaternion.LookRotation(transform.forward, Vector3.up);
    }
}
