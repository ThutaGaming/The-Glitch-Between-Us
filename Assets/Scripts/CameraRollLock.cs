using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// The FPS camera rides on the arms rig's head bone (SOCKET_Camera), so any roll that a
/// walk/idle/reload animation clip feeds into the spine/neck/head bones bleeds straight into
/// the camera's render rotation. Bind pose is level, but nothing re-levels it during playback.
/// This re-levels the camera right before it renders, after Animator + CameraLook have posed
/// the rig, by keeping its current look direction but forcing world-up as up - killing roll
/// without touching pitch/yaw.
///
/// URP never raises Camera.onPreCull, so the level-up also hooks
/// RenderPipelineManager.beginCameraRendering; onPreCull stays for the built-in pipeline.
/// </summary>
public class CameraRollLock : MonoBehaviour
{
    private void OnEnable()
    {
        Camera.onPreCull += HandlePreCull;
        RenderPipelineManager.beginCameraRendering += HandleBeginCameraRendering;
    }

    private void OnDisable()
    {
        Camera.onPreCull -= HandlePreCull;
        RenderPipelineManager.beginCameraRendering -= HandleBeginCameraRendering;
    }

    private void HandleBeginCameraRendering(ScriptableRenderContext context, Camera cam) => HandlePreCull(cam);

    private void HandlePreCull(Camera cam)
    {
        if (!Application.isPlaying) return;
        transform.rotation = Quaternion.LookRotation(transform.forward, Vector3.up);
    }
}
