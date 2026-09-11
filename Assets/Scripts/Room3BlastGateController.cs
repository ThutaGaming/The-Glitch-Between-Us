using System.Collections;
using TMPro;
using UnityEngine;

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

    private bool unlocked;

    public void Unlock()
    {
        if (unlocked) return;
        unlocked = true;
        StartCoroutine(OpenSequence());
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
                StartCoroutine(Slide(magneticLocks[i], Vector3.up * 0.95f, 0.32f));
            yield return new WaitForSeconds(0.16f);
        }

        foreach (var piston in hydraulicPistons)
            if (piston != null) StartCoroutine(ExtendPiston(piston));

        if (steamBurst != null) steamBurst.Play(true);
        yield return StartCoroutine(FadeLighting());

        if (leftDoor != null) StartCoroutine(Slide(leftDoor, Vector3.left * 6.2f, 2.1f));
        if (rightDoor != null) StartCoroutine(Slide(rightDoor, Vector3.right * 6.2f, 2.1f));
        yield return new WaitForSeconds(1.6f);

        if (levelTwoAccess != null) levelTwoAccess.SetActive(true);
        if (statusDisplay != null) statusDisplay.text = "LEVEL 2 ACCESS OPEN";
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
            for (int i = 0; i < cyanStart.Length; i++) if (cyanFacilityLights[i] != null) cyanFacilityLights[i].intensity = Mathf.Lerp(cyanStart[i], cyanStart[i] * 1.8f, t);
            yield return null;
        }
        foreach (var light in redEmergencyLights) if (light != null) light.enabled = false;
    }

    private IEnumerator ExtendPiston(Transform piston)
    {
        Vector3 start = piston.localScale;
        Vector3 end = new Vector3(start.x, start.y * 1.75f, start.z);
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