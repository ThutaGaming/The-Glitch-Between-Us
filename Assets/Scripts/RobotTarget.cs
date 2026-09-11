using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Generic "shoot N times to destroy" target for Basic Training's Robot_grey spawns. Hits are
/// registered externally by BasicTrainingRange (a raycast fired the instant the player's
/// equipped weapon's ammo count drops - Infima's Weapon has no hit/damage event of its own).
/// Flashes with a scale punch per hit (shader-agnostic - the robot's material setup is unknown)
/// and collapses before being destroyed.
/// </summary>
public class RobotTarget : MonoBehaviour
{
    public int hitsToKill = 3;
    public UnityEvent onDied;

    private int hitsTaken;
    private bool dead;
    private Vector3 baseScale;
    private Coroutine flashRoutine;

    private void Awake()
    {
        baseScale = transform.localScale;
    }

    public void RegisterHit()
    {
        if (dead) return;
        hitsTaken++;

        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashHit());

        if (hitsTaken >= hitsToKill) Die();
    }

    private void Die()
    {
        dead = true;
        onDied?.Invoke();
        StartCoroutine(CollapseAndDestroy());
    }

    private IEnumerator FlashHit()
    {
        const float duration = 0.12f;
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float p = Mathf.Sin((t / duration) * Mathf.PI);
            transform.localScale = baseScale * (1f + p * 0.18f);
            yield return null;
        }
        transform.localScale = baseScale;
    }

    private IEnumerator CollapseAndDestroy()
    {
        const float duration = 0.25f;
        Vector3 start = transform.localScale;
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            transform.localScale = Vector3.Lerp(start, Vector3.zero, t / duration);
            yield return null;
        }
        Destroy(gameObject);
    }
}
