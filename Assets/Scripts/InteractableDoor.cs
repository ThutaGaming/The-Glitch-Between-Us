using System.Collections;
using UnityEngine;

/// <summary>
/// Swings this transform open/closed around its local Y axis on interact. Assumes the
/// transform's pivot already sits at the hinge edge (true for the Door_1m_A_left leaf
/// meshes) — rotating a center-pivoted mesh would look wrong. If a door opens through
/// the wrong side, flip the sign of `openAngle` in the Inspector.
/// </summary>
public class InteractableDoor : MonoBehaviour, IInteractable
{
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openDuration = 0.8f;

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool isOpen;
    private Coroutine animRoutine;

    public Transform InteractTransform => transform;

    private void Awake()
    {
        closedRotation = transform.localRotation;
        openRotation = closedRotation * Quaternion.Euler(0f, openAngle, 0f);
    }

    private void OnEnable() => InteractableRegistry.All.Add(this);
    private void OnDisable() => InteractableRegistry.All.Remove(this);

    public string GetPrompt() => isOpen ? "(E) Close Door" : "(E) Open Door";

    public void Interact(GameObject player)
    {
        isOpen = !isOpen;
        if (animRoutine != null) StopCoroutine(animRoutine);
        animRoutine = StartCoroutine(Animate(isOpen ? openRotation : closedRotation));
    }

    private IEnumerator Animate(Quaternion target)
    {
        Quaternion start = transform.localRotation;
        float t = 0f;
        while (t < openDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / openDuration));
            transform.localRotation = Quaternion.Slerp(start, target, p);
            yield return null;
        }

        transform.localRotation = target;
    }
}
