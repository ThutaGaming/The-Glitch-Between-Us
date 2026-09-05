using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Duplicates this object's meshes into a faint blue, always-on-top "X-ray" outline (see
/// Assets/Shaders/ObjectiveHighlight.shader) that a mission script switches on with
/// <see cref="SetGlowing"/> while this object is the current objective. Shows at a constant
/// intensity the whole time it's on, whether or not the player can see the real object.
///
/// Handles both plain meshes and skinned characters: a skinned clone shares the original's
/// bones, so the outline follows the animation instead of standing still in a T-pose.
/// </summary>
public class ObjectiveGlow : MonoBehaviour
{
    [SerializeField] private Material highlightMaterial;

    private readonly List<GameObject> highlights = new List<GameObject>();

    private void Awake()
    {
        // Collect the sources up front: the clones become children too, and cloning a clone
        // would spiral.
        var meshRenderers = new List<MeshRenderer>(GetComponentsInChildren<MeshRenderer>(true));
        var skinnedRenderers = new List<SkinnedMeshRenderer>(GetComponentsInChildren<SkinnedMeshRenderer>(true));

        foreach (var source in meshRenderers)
        {
            var filter = source.GetComponent<MeshFilter>();
            if (filter == null || filter.sharedMesh == null) continue;

            var clone = NewHighlightObject(source.transform);
            clone.AddComponent<MeshFilter>().sharedMesh = filter.sharedMesh;
            Configure(clone.AddComponent<MeshRenderer>(), source.sharedMaterials.Length);
        }

        foreach (var source in skinnedRenderers)
        {
            if (source.sharedMesh == null) continue;

            var clone = NewHighlightObject(source.transform);
            var renderer = clone.AddComponent<SkinnedMeshRenderer>();
            renderer.sharedMesh = source.sharedMesh;
            // Same bones and root bone as the original, so the clone deforms with it.
            renderer.bones = source.bones;
            renderer.rootBone = source.rootBone;
            renderer.localBounds = source.localBounds;
            Configure(renderer, source.sharedMaterials.Length);
        }

        SetGlowing(false);
    }

    private GameObject NewHighlightObject(Transform parent)
    {
        var go = new GameObject("~ObjectiveHighlight") { hideFlags = HideFlags.DontSave };
        go.transform.SetParent(parent, false);
        highlights.Add(go);
        return go;
    }

    private void Configure(Renderer renderer, int materialCount)
    {
        var mats = new Material[Mathf.Max(1, materialCount)];
        for (int i = 0; i < mats.Length; i++) mats[i] = highlightMaterial;
        renderer.sharedMaterials = mats;
        renderer.shadowCastingMode = ShadowCastingMode.Off;
        renderer.receiveShadows = false;
    }

    /// <summary>Turns the objective marker on or off.</summary>
    public void SetGlowing(bool glowing)
    {
        foreach (var go in highlights)
        {
            if (go != null) go.SetActive(glowing);
        }
    }
}
