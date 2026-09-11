using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Reacts to NetworkPuzzleTerminal.onNetworkRestored: flips the door's status display from
/// LOCKED to ACCESS GRANTED, kills the red emergency lights in favour of the blue facility ones,
/// fires a steam burst, then slides both door panels open.
/// </summary>
public class BlastDoorController : MonoBehaviour
{
    [SerializeField] private SlidingPanel leftPanel;
    [SerializeField] private SlidingPanel rightPanel;
    [SerializeField] private TextMeshPro statusDisplay;
    [SerializeField] private Renderer centralHexEmblem;
    [SerializeField] private Material lockedEmblemMaterial;
    [SerializeField] private Material unlockedEmblemMaterial;
    [SerializeField] private Light[] redEmergencyLights;
    [SerializeField] private Light[] blueFacilityLights;
    [SerializeField] private ParticleSystem steamBurst;

    private bool unlocked;

    public void Unlock()
    {
        if (unlocked) return;
        unlocked = true;
        StartCoroutine(UnlockSequence());
    }

    private IEnumerator UnlockSequence()
    {
        if (statusDisplay != null)
        {
            statusDisplay.text = "ACCESS GRANTED";
            statusDisplay.color = new Color(0.4f, 1f, 0.55f);
        }

        if (centralHexEmblem != null && unlockedEmblemMaterial != null)
            centralHexEmblem.sharedMaterial = unlockedEmblemMaterial;

        if (redEmergencyLights != null)
            foreach (var l in redEmergencyLights)
                if (l != null) l.enabled = false;

        if (blueFacilityLights != null)
            foreach (var l in blueFacilityLights)
                if (l != null) l.enabled = true;

        if (steamBurst != null) steamBurst.Play();

        yield return new WaitForSeconds(0.7f);

        if (leftPanel != null) leftPanel.Open();
        if (rightPanel != null) rightPanel.Open();
    }
}
