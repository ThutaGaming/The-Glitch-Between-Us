using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Slides this transform along its own local axis to reveal a passage (e.g. Grid_06 opening onto
/// the Floor 1 stairwell) - a simpler, standalone alternative to BarnDoorSlider for a flat panel
/// with no Hanger/Wheel rig. Triggered externally via <see cref="Open"/>, not by the player.
/// </summary>
public class SlidingPanel : MonoBehaviour
{
    [Tooltip("Local-space direction the panel slides to open (gets normalized).")]
    [SerializeField] private Vector3 openLocalDirection = Vector3.forward;
    [SerializeField] private float openDistance = 3.4f;
    [SerializeField] private float openDuration = 1.2f;

    public UnityEvent onOpened;

    private Vector3 closedLocalPosition;
    private bool opened;

    private void Awake()
    {
        closedLocalPosition = transform.localPosition;
    }

    /// <summary>Slides the panel open once; safe to call again (later calls are ignored).</summary>
    public void Open()
    {
        if (opened) return;
        opened = true;
        StartCoroutine(SlideOpen());
    }

    private IEnumerator SlideOpen()
    {
        Vector3 target = closedLocalPosition + openLocalDirection.normalized * openDistance;
        Vector3 start = transform.localPosition;

        for (float t = 0f; t < openDuration; t += Time.deltaTime)
        {
            float p = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / openDuration));
            transform.localPosition = Vector3.Lerp(start, target, p);
            yield return null;
        }
        transform.localPosition = target;

        onOpened?.Invoke();
    }
}
