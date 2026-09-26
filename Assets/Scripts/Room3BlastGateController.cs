using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public sealed class Room3BlastGateController : MonoBehaviour
{
    public Transform leftDoor;
    public Transform rightDoor;
    public Transform[] magneticLocks;
    public Transform[] hydraulicPistons;
    public TMP_Text statusDisplay;
    public Light[] redEmergencyLights;
    public Light[] cyanFacilityLights;
    public ParticleSystem steamBurst;
    public GameObject levelTwoAccess;
    [Tooltip("Fires once the blast doors have finished sliding open.")]
    public UnityEvent onOpened;

    private const float LockRise = 0.95f;
    private const float PistonStretch = 1.75f;
    private const float DoorSlide = 6.2f;
    private const float CyanBoost = 1.8f;

    private bool unlocked;

    public void Unlock()
    {
        if (unlocked) return;
        unlocked = true;
        StartCoroutine(OpenSequence());
    }

    /// <summary>
    /// Jumps straight to the end state of <see cref="Unlock"/> - locks up, pistons out, lights
    /// swapped, doors apart - and fires <see cref="onOpened"/>. For debug skips.
    /// </summary>
    public void OpenInstantly()
    {
        if (unlocked) return;
        unlocked = true;

        foreach (var magneticLock in magneticLocks)
            if (magneticLock != null) magneticLock.localPosition += Vector3.up * LockRise;
        foreach (var piston in hydraulicPistons)
            if (piston != null) piston.localScale = new Vector3(piston.localScale.x, piston.localScale.y * PistonStretch, piston.localScale.z);
        foreach (var light in redEmergencyLights)
            if (light != null) { light.intensity = 0f; light.enabled = false; }
        foreach (var light in cyanFacilityLights)
            if (light != null) light.intensity *= CyanBoost;
        if (leftDoor != null) leftDoor.localPosition += Vector3.left * DoorSlide;
        if (rightDoor != null) rightDoor.localPosition += Vector3.right * DoorSlide;

        if (statusDisplay != null) statusDisplay.color = new Color(0.25f, 1f, 0.7f);
        FinishOpening();
    }

    private IEnumerator OpenSequence()
    {
        if (statusDisplay != null)
        {
            statusDisplay.text = "ACCESS GRANTED";
            statusDisplay.color = new Color(0.25f, 1f, 0.7f);
        }

        for (int i = 0; i < magneticLocks.Length; i++)
        {
            if (magneticLocks[i] != null)
                StartCoroutine(Slide(magneticLocks[i], Vector3.up * LockRise, 0.32f));
            yield return new WaitForSeconds(0.16f);
        }

        foreach (var piston in hydraulicPistons)
            if (piston != null) StartCoroutine(ExtendPiston(piston));

        if (steamBurst != null) steamBurst.Play(true);
        yield return StartCoroutine(FadeLighting());

        if (leftDoor != null) StartCoroutine(Slide(leftDoor, Vector3.left * DoorSlide, 2.1f));
        if (rightDoor != null) StartCoroutine(Slide(rightDoor, Vector3.right * DoorSlide, 2.1f));
        yield return new WaitForSeconds(1.6f);

        FinishOpening();
    }

    private void FinishOpening()
    {
        if (levelTwoAccess != null) levelTwoAccess.SetActive(true);
        if (statusDisplay != null) statusDisplay.text = "LEVEL 2 ACCESS OPEN";
        onOpened?.Invoke();
    }

    private IEnumerator FadeLighting()
    {
        float[] redStart = new float[redEmergencyLights.Length];
        float[] cyanStart = new float[cyanFacilityLights.Length];
        for (int i = 0; i < redStart.Length; i++) if (redEmergencyLights[i] != null) redStart[i] = redEmergencyLights[i].intensity;
        for (int i = 0; i < cyanStart.Length; i++) if (cyanFacilityLights[i] != null) cyanStart[i] = cyanFacilityLights[i].intensity;

        for (float time = 0f; time < 1.2f; time += Time.deltaTime)
        {
            float t = Mathf.SmoothStep(0f, 1f, time / 1.2f);
            for (int i = 0; i < redStart.Length; i++) if (redEmergencyLights[i] != null) redEmergencyLights[i].intensity = Mathf.Lerp(redStart[i], 0f, t);
            for (int i = 0; i < cyanStart.Length; i++) if (cyanFacilityLights[i] != null) cyanFacilityLights[i].intensity = Mathf.Lerp(cyanStart[i], cyanStart[i] * CyanBoost, t);
            yield return null;
        }
        foreach (var light in redEmergencyLights) if (light != null) light.enabled = false;
    }

    private IEnumerator ExtendPiston(Transform piston)
    {
        Vector3 start = piston.localScale;
        Vector3 end = new Vector3(start.x, start.y * PistonStretch, start.z);
        for (float time = 0f; time < 0.7f; time += Time.deltaTime)
        {
            piston.localScale = Vector3.Lerp(start, end, Mathf.SmoothStep(0f, 1f, time / 0.7f));
            yield return null;
        }
        piston.localScale = end;
    }

    private IEnumerator Slide(Transform target, Vector3 offset, float duration)
    {
        Vector3 start = target.localPosition;
        Vector3 end = start + offset;
        for (float time = 0f; time < duration; time += Time.deltaTime)
        {
            target.localPosition = Vector3.Lerp(start, end, Mathf.SmoothStep(0f, 1f, time / duration));
            yield return null;
        }
        target.localPosition = end;
    }
}