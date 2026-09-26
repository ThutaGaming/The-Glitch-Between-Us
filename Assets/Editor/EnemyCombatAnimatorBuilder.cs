using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

/// <summary>
/// Builds the combat Animator controllers under Resources/EnemyAnimators that Level 4's
/// Robot_Soldier_White and Ball/Hermit/Blast robots load at runtime.
///
/// The controllers shipped with those assets are demo reels: every state flows into the next on
/// exit time (idle -> walk -> jump -> ... -> Die -> idle), so a robot left alone plays its whole
/// move list, death included, while it is still alive and shooting. These copies keep only the
/// states the combat scripts drive, with no automatic transitions except one-shots returning to
/// idle. Die has no way out, so it holds the last frame.
/// </summary>
public static class EnemyCombatAnimatorBuilder
{
    private const string OutputFolder = "Assets/Resources/EnemyAnimators";

    private struct Spec
    {
        public string source;
        public string output;
        public string idle;
        public string[] loops;
        public string[] oneShots;
    }

    private static readonly Spec[] Specs =
    {
        new Spec { source = "SciFiWarrior", output = "SciFiWarrior Combat", idle = "Idle_Shoot_Ar",
            loops = new[] { "Shoot_BurstShot_AR" }, oneShots = new string[0] },
        new Spec { source = "Blast Robot", output = "Blast Robot Combat", idle = "Idle",
            loops = new string[0], oneShots = new[] { "Left Blast Attack", "Right Blast Attack", "Take Damage" } },
        new Spec { source = "Hermit Robot", output = "Hermit Robot Combat", idle = "Idle",
            loops = new[] { "Machine Gun Attack" }, oneShots = new[] { "Take Damage" } },
        new Spec { source = "Ball Robot", output = "Ball Robot Combat", idle = "Idle Open",
            loops = new[] { "Rapid Fire Attack" }, oneShots = new[] { "Take Damage" } },
    };

    [MenuItem("Tools/Glitch/Build Enemy Combat Animators")]
    public static void BuildAll()
    {
        Directory.CreateDirectory(OutputFolder);
        foreach (var spec in Specs) Build(spec);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void Build(Spec spec)
    {
        var source = FindController(spec.source);
        if (source == null)
        {
            Debug.LogError("[EnemyCombatAnimatorBuilder] Source controller not found: " + spec.source);
            return;
        }

        string path = OutputFolder + "/" + spec.output + ".controller";
        AssetDatabase.DeleteAsset(path);
        var controller = AnimatorController.CreateAnimatorControllerAtPath(path);
        var machine = controller.layers[0].stateMachine;

        var idle = AddState(machine, source, spec.idle);
        machine.defaultState = idle;
        foreach (string name in spec.loops) AddState(machine, source, name);
        foreach (string name in spec.oneShots)
        {
            var state = AddState(machine, source, name);
            var back = state.AddTransition(idle);
            back.hasExitTime = true;
            back.exitTime = 0.9f;
            back.duration = 0.15f;
        }
        AddState(machine, source, "Die");

        EditorUtility.SetDirty(controller);
        Debug.Log("[EnemyCombatAnimatorBuilder] Built " + path);
    }

    private static AnimatorState AddState(AnimatorStateMachine machine, AnimatorController source, string name)
    {
        var original = source.layers[0].stateMachine.states.Select(s => s.state).FirstOrDefault(s => s.name == name);
        var state = machine.AddState(name);
        if (original == null)
        {
            Debug.LogError("[EnemyCombatAnimatorBuilder] " + source.name + " has no state " + name);
            return state;
        }
        state.motion = original.motion;
        state.speed = original.speed;
        state.mirror = original.mirror;
        return state;
    }

    private static AnimatorController FindController(string name)
    {
        return AssetDatabase.FindAssets(name + " t:AnimatorController")
            .Select(g => AssetDatabase.LoadAssetAtPath<AnimatorController>(AssetDatabase.GUIDToAssetPath(g)))
            .FirstOrDefault(c => c != null && c.name == name);
    }
}
