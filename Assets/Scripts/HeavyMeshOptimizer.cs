using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

/// <summary>
/// Some imported characters are film-quality meshes: Sci_Fi_Character_08 (the Level 1-3 soldiers)
/// is about 260k triangles, and the girl in Level 5 over two million. Drawing them into the
/// shadow cascades as well is what held those levels far below the others' frame rate. At every
/// scene load, and for each soldier as EnemyAI2 takes it over, this:
///  - lets only the character's biggest part cast a shadow - a shadow only shows the silhouette;
///  - gives soldiers a distance LOD that keeps just the body and head once they are small on
///    screen, dropping the separate shoe/bag/belt meshes nobody can see at that size.
/// Characters with a normal triangle budget are left alone.
/// </summary>
public static class HeavyMeshOptimizer
{
    // All skinned parts of one character together.
    private const long HeavyTriangles = 60000;
    // Screen height (fraction) below which a soldier drops to body + head.
    private const float SoldierDetailHeight = 0.12f;

    private static readonly HashSet<int> done = new HashSet<int>();
    private static bool subscribed;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        done.Clear();
        subscribed = false;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (!subscribed)
        {
            subscribed = true;
            SceneManager.sceneLoaded += (scene, mode) => OptimizeScene();
        }
        OptimizeScene();
    }

    private static void OptimizeScene()
    {
        done.Clear();
        foreach (var animator in Object.FindObjectsByType<Animator>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            OptimizeCharacter(animator.gameObject, soldierLod: false);
    }

    /// <summary>Safe to call more than once per character; later calls add the soldier LOD if
    /// it wasn't asked for the first time.</summary>
    public static void OptimizeCharacter(GameObject root, bool soldierLod)
    {
        if (root == null) return;
        var parts = root.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        if (parts.Length == 0) return;

        long triangles = 0;
        SkinnedMeshRenderer biggest = null;
        float biggestVolume = -1f;
        foreach (var p in parts)
        {
            var mesh = p.sharedMesh;
            if (mesh == null) continue;
            for (int s = 0; s < mesh.subMeshCount; s++) triangles += mesh.GetIndexCount(s) / 3;
            Vector3 size = Vector3.Scale(p.localBounds.size, p.transform.lossyScale);
            float volume = Mathf.Abs(size.x * size.y * size.z);
            if (volume > biggestVolume) { biggestVolume = volume; biggest = p; }
        }
        if (triangles < HeavyTriangles) return;

        if (done.Add(root.GetInstanceID()))
        {
            foreach (var p in parts)
                if (p != biggest && p.shadowCastingMode == ShadowCastingMode.On)
                    p.shadowCastingMode = ShadowCastingMode.Off;
        }

        if (soldierLod && root.GetComponent<LODGroup>() == null) AddSoldierLod(root, parts);
    }

    private static void AddSoldierLod(GameObject root, SkinnedMeshRenderer[] parts)
    {
        var near = new List<Renderer>();
        var far = new List<Renderer>();
        foreach (var p in parts)
        {
            if (p.sharedMesh == null) continue;
            near.Add(p);
            string n = p.name;
            if (n.Contains("Body") || n.Contains("Head")) far.Add(p);
        }
        // A single-mesh character (or one without these part names) has nothing to drop.
        if (far.Count == 0 || far.Count == near.Count) return;

        var lodGroup = root.AddComponent<LODGroup>();
        lodGroup.SetLODs(new[]
        {
            new LOD(SoldierDetailHeight, near.ToArray()),
            new LOD(0f, far.ToArray()),   // never culled - an enemy must stay visible at any range
        });
        lodGroup.RecalculateBounds();
    }
}
