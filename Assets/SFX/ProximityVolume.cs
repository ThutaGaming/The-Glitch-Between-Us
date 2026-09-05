using UnityEngine;

/// <summary>
/// Drop this on any GameObject with an AudioSource: it gets louder as the player walks closer
/// and quieter as they walk away. Drives volume directly (not Unity's own 3D rolloff), so it
/// works the same whether the source's Spatial Blend is 2D or 3D.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class ProximityVolume : MonoBehaviour
{
    [Header("Range")]
    [Tooltip("At or inside this distance, the sound plays at maxVolume.")]
    [SerializeField] private float fullVolumeDistance = 3f;
    [Tooltip("At or beyond this distance, the sound is silent.")]
    [SerializeField] private float silentDistance = 15f;

    [Header("Volume")]
    [SerializeField] private float maxVolume = 1f;

    private AudioSource source;
    private Transform player;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    private void Start()
    {
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        float range = Mathf.Max(0.01f, silentDistance - fullVolumeDistance);
        float t = Mathf.Clamp01((distance - fullVolumeDistance) / range); // 0 = close, 1 = far
        float closeness = Mathf.SmoothStep(1f, 0f, t);

        source.volume = closeness * maxVolume;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, fullVolumeDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, silentDistance);
    }
}
