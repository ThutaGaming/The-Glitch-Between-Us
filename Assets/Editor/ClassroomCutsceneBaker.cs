using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// Bakes the humanoid clips for School Scene's new-student cutscene (NewStudentCutscene) for
/// TrHtetHtet and MoneMone: standing idle / talk / point / hands-front / bow, and seated idle /
/// wave / thank-you, plus one Animator Controller each (walking reuses Mr. Thiha's Mixamo walk).
///
/// The project has no humanoid sit, wave or bow clips, so the poses are built here on each
/// character's real skeleton - limbs placed with a small two-bone IK, then read back as muscle
/// values through HumanPoseHandler - and saved as ordinary humanoid .anim files.
/// Re-run from Tools > Glitch > Bake Classroom Cutscene Animations after changing a pose.
/// </summary>
public static class ClassroomCutsceneBaker
{
    private const string OutFolder = "Assets/Student/Cutscene";
    private const string WalkClipFbx = "Assets/Student/Mr.Thiha/ThihaWalking.fbx";

    /// <summary>Playback speed of the Walk state; CutsceneActor matches its move speed to it.</summary>
    public const float WalkStateSpeed = 0.8f;

    // Both characters sit in the scene at scale 250 (their FBX units are 1/250 m).
    private const float WorldScale = 250f;
    // (Prb)Chair2: the seat top is 0.61 m above the classroom floor.
    private const float SeatHeightWorld = 0.61f;
    private const float HipJointAboveSeatWorld = 0.13f;

    [MenuItem("Tools/Glitch/Bake Classroom Cutscene Animations")]
    public static void BakeAll()
    {
        if (!AssetDatabase.IsValidFolder(OutFolder)) AssetDatabase.CreateFolder("Assets/Student", "Cutscene");

        AnimationClip walk = null;
        foreach (var o in AssetDatabase.LoadAllAssetsAtPath(WalkClipFbx))
        {
            if (o is AnimationClip c && !c.name.StartsWith("__preview")) walk = c;
        }

        foreach (var name in new[] { "TrHtetHtet", "MoneMone" }) BakeCharacter(name, walk);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void BakeCharacter(string name, AnimationClip walk)
    {
        string fbx = "Assets/Student/" + name + "/" + name + ".fbx";
        using (var r = new Rig(fbx))
        {
            var clips = new Dictionary<string, AnimationClip>
            {
                ["Idle"] = Clip(r, name + "_Idle", true, (0f, () => Standing(r, 0f)), (2f, () => Standing(r, 1f)), (4f, () => Standing(r, 0f))),
                ["Talk"] = Clip(r, name + "_Talk", true, (0f, () => Talk(r, 0f)), (1.2f, () => Talk(r, 1f)), (2.4f, () => Talk(r, 0f))),
                ["Point"] = Clip(r, name + "_Point", true, (0f, () => Point(r, 0f)), (1.5f, () => Point(r, 1f)), (3f, () => Point(r, 0f))),
                ["HandsFront"] = Clip(r, name + "_HandsFront", true, (0f, () => HandsFront(r, 0f)), (2f, () => HandsFront(r, 1f)), (4f, () => HandsFront(r, 0f))),
                ["Bow"] = Clip(r, name + "_Bow", false, (0f, () => HandsFront(r, 0f)), (0.7f, () => Bow(r)), (1.5f, () => Bow(r)), (2.2f, () => HandsFront(r, 0f))),
                ["SitIdle"] = Clip(r, name + "_SitIdle", true, (0f, () => SitIdle(r, 0f)), (2f, () => SitIdle(r, 1f)), (4f, () => SitIdle(r, 0f))),
                ["SitWave"] = Clip(r, name + "_SitWave", true, (0f, () => SitWave(r, 0f)), (0.35f, () => SitWave(r, 1f)), (0.7f, () => SitWave(r, 0f))),
                ["SitThanks"] = Clip(r, name + "_SitThanks", false, (0f, () => SitThanks(r, 0f)), (0.8f, () => SitThanks(r, 1f)), (1.8f, () => SitThanks(r, 0f))),
            };

            foreach (var key in new List<string>(clips.Keys))
            {
                clips[key] = SaveClip(clips[key], OutFolder + "/" + clips[key].name + ".anim");
            }
            BuildController(name, clips, walk);
        }
    }

    // ------------------------------------------------------------------ poses
    // Character frame: facing +Z, right = +X, up = +Y. Lengths are in model units.

    /// <summary>Standing, arms relaxed at the sides; breath 0..1 lifts the chest a touch.</summary>
    private static void Standing(Rig r, float breath)
    {
        r.Reset();
        RelaxedArm(r, true, breath);
        RelaxedArm(r, false, breath);
        r.Bend(r.UpperChest, -1.2f * breath);
    }

    private static void RelaxedArm(Rig r, bool right, float breath)
    {
        float s = right ? 1f : -1f;
        r.ArmTo(right, r.Shoulder(right) + r.ArmLen * new Vector3(0.10f * s, -0.92f + 0.012f * breath, 0.10f),
            new Vector3(0.3f * s, 0f, -1f), new Vector3(0.05f * s, -1f, 0.12f), new Vector3(-s, 0f, 0f));
    }

    /// <summary>Explaining to the class: hands in front of the waist, palms up.</summary>
    private static void Talk(Rig r, float k)
    {
        r.Reset();
        float L = r.ArmLen;
        r.ArmTo(true, r.Shoulder(true) + L * new Vector3(-0.16f + 0.10f * k, -0.62f + 0.06f * k, 0.52f),
            new Vector3(1f, -0.7f, -0.3f), new Vector3(-0.15f + 0.25f * k, 0.12f, 1f), new Vector3(0.15f * k, 1f, 0f));
        r.ArmTo(false, r.Shoulder(false) + L * new Vector3(0.16f, -0.70f - 0.03f * k, 0.44f),
            new Vector3(-1f, -0.7f, -0.3f), new Vector3(0.15f, 0.08f, 1f), new Vector3(0f, 1f, 0f));
        r.Nod(2f * k);
    }

    /// <summary>Open-palm gesture out to her right (MoneMone stands there); CutsceneActor's hand IK
    /// fine-tunes the aim at runtime.</summary>
    private static void Point(Rig r, float k)
    {
        r.Reset();
        Vector3 dir = new Vector3(0.90f, -0.36f + 0.03f * k, 0.32f).normalized;
        r.ArmTo(true, r.Shoulder(true) + dir * 0.97f * r.ArmLen, new Vector3(0f, -1f, -0.4f),
            dir + new Vector3(0f, 0.08f, 0f), new Vector3(0f, 1f, 0.15f));
        RelaxedArm(r, false, 0f);
        r.Turn(r.UpperChest, 6f);
    }

    /// <summary>Polite stance: hands clasped in front of the lower belly.</summary>
    private static void HandsFront(Rig r, float breath)
    {
        r.Reset();
        float L = r.ArmLen;
        Vector3 c = r.B(HumanBodyBones.Hips).position + L * new Vector3(0f, 0.10f + 0.01f * breath, 0.30f);
        r.ArmTo(true, c + L * new Vector3(0.035f, 0.01f, 0f), new Vector3(1f, -0.3f, -0.6f),
            new Vector3(-0.55f, -0.75f, 0.25f), new Vector3(-0.2f, 0.1f, -1f));
        r.ArmTo(false, c + L * new Vector3(-0.035f, -0.01f, 0.01f), new Vector3(-1f, -0.3f, -0.6f),
            new Vector3(0.55f, -0.75f, 0.25f), new Vector3(0.2f, 0.1f, -1f));
        r.Bend(r.UpperChest, -1f * breath);
    }

    /// <summary>A polite bow from the waist: the pelvis tips forward while the legs stay upright,
    /// and the clasped hands go down with the body.</summary>
    private static void Bow(Rig r)
    {
        HandsFront(r, 0f);
        r.Bend(r.B(HumanBodyBones.Hips), 16f);
        r.Bend(r.B(HumanBodyBones.LeftUpperLeg), -16f);
        r.Bend(r.B(HumanBodyBones.RightUpperLeg), -16f);
        r.Bend(r.B(HumanBodyBones.Spine), 6f);
        r.Bend(r.B(HumanBodyBones.Chest), 6f);
        r.Bend(r.UpperChest, 4f);
        r.Nod(10f);
    }

    /// <summary>Seated on (Prb)Chair2 with the root on the floor straight under the hip joints:
    /// thighs level, shins down to feet flat on the floor.</summary>
    private static void SitBase(Rig r)
    {
        r.Reset();
        Transform hips = r.B(HumanBodyBones.Hips);
        float hipJointY = r.Ground + (SeatHeightWorld + HipJointAboveSeatWorld) / WorldScale;
        hips.position += Vector3.down * (r.B(HumanBodyBones.LeftUpperLeg).position.y - hipJointY);

        foreach (bool right in new[] { true, false })
        {
            float s = right ? 1f : -1f;
            Vector3 hip = r.B(right ? HumanBodyBones.RightUpperLeg : HumanBodyBones.LeftUpperLeg).position;
            float T = r.ThighLen, S = r.ShinLen;
            float ankleY = r.Ground + r.AnkleHeight;

            float alpha = 4f * Mathf.Deg2Rad;
            if (hip.y - T * Mathf.Sin(alpha) - ankleY > S) alpha = Mathf.Asin(Mathf.Clamp01((hip.y - ankleY - S) / T));
            Vector3 knee = hip + T * new Vector3(0.10f * s, -Mathf.Sin(alpha), Mathf.Cos(alpha)).normalized;
            float dy = knee.y - ankleY;
            Vector3 ankle = new Vector3(knee.x, ankleY, knee.z + Mathf.Sqrt(Mathf.Max(0f, S * S - dy * dy)));

            bool r2 = right;
            r.SolveLimb(right ? HumanBodyBones.RightUpperLeg : HumanBodyBones.LeftUpperLeg,
                right ? HumanBodyBones.RightLowerLeg : HumanBodyBones.LeftLowerLeg,
                right ? HumanBodyBones.RightFoot : HumanBodyBones.LeftFoot,
                knee, ankle, () => r.OrientFootFlat(r2),
                right ? HumanBodyBones.RightToes : HumanBodyBones.LeftToes);
        }
    }

    private static void HandOnThigh(Rig r, bool right)
    {
        float s = right ? 1f : -1f;
        Vector3 hip = r.B(right ? HumanBodyBones.RightUpperLeg : HumanBodyBones.LeftUpperLeg).position;
        Vector3 knee = r.B(right ? HumanBodyBones.RightLowerLeg : HumanBodyBones.LeftLowerLeg).position;
        Vector3 onThigh = Vector3.Lerp(hip, knee, 0.60f) + Vector3.up * (0.20f * r.ThighLen) + Vector3.right * (-0.02f * s * r.ArmLen);
        r.ArmTo(right, onThigh, new Vector3(s, -0.2f, -0.8f), new Vector3(-0.12f * s, -0.3f, 1f), new Vector3(0f, -1f, 0.1f));
    }

    private static void SitIdle(Rig r, float breath)
    {
        SitBase(r);
        HandOnThigh(r, true);
        HandOnThigh(r, false);
        r.Bend(r.UpperChest, -1f * breath);
    }

    /// <summary>Seated "Hi!": right hand raised in front of her at head height, fingers up and palm
    /// toward the right (Thuta sits on her right), swaying forward/back. Held forward, not out to
    /// the side, so from his seat it shows beside her face instead of in front of it; the cutscene
    /// turns only her head for this one.</summary>
    private static void SitWave(Rig r, float k)
    {
        SitBase(r);
        HandOnThigh(r, false);
        Vector3 shoulder = r.Shoulder(true);
        Vector3 elbow = shoulder + r.UpperArmLen * new Vector3(0.18f, -0.40f, 0.90f).normalized;
        float th = Mathf.Lerp(-14f, 14f, k) * Mathf.Deg2Rad;
        Vector3 fore = new Vector3(0f, Mathf.Cos(th), 0.10f + Mathf.Sin(th)).normalized;
        r.SolveLimb(HumanBodyBones.RightUpperArm, HumanBodyBones.RightLowerArm, HumanBodyBones.RightHand,
            elbow, elbow + fore * r.ForeArmLen, () => r.OrientHand(true, fore, new Vector3(1f, 0f, 0.15f)),
            HumanBodyBones.RightIndexProximal, HumanBodyBones.RightThumbProximal);
    }

    /// <summary>Seated thank-you: hands together at the chest; k = 1 is the small bow.</summary>
    private static void SitThanks(Rig r, float k)
    {
        SitBase(r);
        Vector3 chest = r.UpperChest.position;
        float L = r.ArmLen;
        foreach (bool right in new[] { true, false })
        {
            float s = right ? 1f : -1f;
            r.ArmTo(right, chest + L * new Vector3(0.04f * s, -0.10f, 0.33f), new Vector3(s, -1f, -0.2f),
                new Vector3(-0.25f * s, 1f, 0.12f), new Vector3(-s, 0f, 0.05f));
        }
        r.Bend(r.B(HumanBodyBones.Spine), 3f * k);
        r.Bend(r.B(HumanBodyBones.Chest), 3f * k);
        r.Nod(15f * k);
    }

    // ------------------------------------------------------------------ clips

    private static AnimationClip Clip(Rig r, string name, bool loop, params (float t, Action pose)[] keys)
    {
        var poses = new List<(float t, HumanPose pose)>();
        foreach (var k in keys)
        {
            k.pose();
            poses.Add((k.t, r.Capture()));
        }

        var clip = new AnimationClip { name = name };
        float groundOffset = -r.Ground / r.HumanScale;   // soles on the Animator's root, like the Mixamo walk

        void Set(string prop, Func<HumanPose, float> value)
        {
            var curve = new AnimationCurve();
            foreach (var p in poses) curve.AddKey(new Keyframe(p.t, value(p.pose)));
            AnimationUtility.SetEditorCurve(clip, EditorCurveBinding.FloatCurve("", typeof(Animator), prop), curve);
        }

        // Keep the root rotation keys on one hemisphere so they don't spin the long way round.
        for (int i = 1; i < poses.Count; i++)
        {
            if (Quaternion.Dot(poses[i].pose.bodyRotation, poses[i - 1].pose.bodyRotation) < 0f)
            {
                var p = poses[i].pose;
                Quaternion q = p.bodyRotation;
                p.bodyRotation = new Quaternion(-q.x, -q.y, -q.z, -q.w);
                poses[i] = (poses[i].t, p);
            }
        }

        Set("RootT.x", p => p.bodyPosition.x);
        Set("RootT.y", p => p.bodyPosition.y + groundOffset);
        Set("RootT.z", p => p.bodyPosition.z);
        Set("RootQ.x", p => p.bodyRotation.x);
        Set("RootQ.y", p => p.bodyRotation.y);
        Set("RootQ.z", p => p.bodyRotation.z);
        Set("RootQ.w", p => p.bodyRotation.w);
        for (int m = 0; m < HumanTrait.MuscleCount; m++)
        {
            int idx = m;
            Set(CurveName(HumanTrait.MuscleName[m]), p => p.muscles[idx]);
        }

        // Everything baked into the pose: the cutscene script moves the root itself.
        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = loop;
        settings.keepOriginalOrientation = true;
        settings.keepOriginalPositionY = true;
        settings.keepOriginalPositionXZ = true;
        settings.loopBlendOrientation = true;
        settings.loopBlendPositionY = true;
        settings.loopBlendPositionXZ = true;
        AnimationUtility.SetAnimationClipSettings(clip, settings);
        return clip;
    }

    /// <summary>HumanTrait names fingers "Left Index 1 Stretched"; clip curves call them
    /// "LeftHand.Index.1 Stretched".</summary>
    private static string CurveName(string muscle)
    {
        string[] parts = muscle.Split(' ');
        if (parts.Length >= 3 && (parts[0] == "Left" || parts[0] == "Right") &&
            Array.IndexOf(new[] { "Thumb", "Index", "Middle", "Ring", "Little" }, parts[1]) >= 0)
        {
            return parts[0] + "Hand." + parts[1] + "." + string.Join(" ", parts, 2, parts.Length - 2);
        }
        return muscle;
    }

    private static AnimationClip SaveClip(AnimationClip clip, string path)
    {
        var existing = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
        if (existing == null)
        {
            AssetDatabase.CreateAsset(clip, path);
            return clip;
        }
        EditorUtility.CopySerialized(clip, existing);
        EditorUtility.SetDirty(existing);
        return existing;
    }

    private static void BuildController(string name, Dictionary<string, AnimationClip> clips, AnimationClip walk)
    {
        string path = OutFolder + "/AC_" + name + ".controller";
        var ctrl = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
        if (ctrl == null) ctrl = AnimatorController.CreateAnimatorControllerAtPath(path);

        var layers = ctrl.layers;
        var sm = layers[0].stateMachine;
        foreach (var s in sm.states) sm.RemoveState(s.state);

        AnimatorState idle = null;
        foreach (var kv in clips)
        {
            var state = sm.AddState(kv.Key);
            state.motion = kv.Value;
            if (kv.Key == "Idle") idle = state;
        }
        var walkState = sm.AddState("Walk");
        walkState.motion = walk;
        walkState.speed = WalkStateSpeed;
        sm.defaultState = idle;

        layers[0].iKPass = true;   // CutsceneActor's head look-at and pointing hand
        ctrl.layers = layers;
        EditorUtility.SetDirty(ctrl);
    }

    // ------------------------------------------------------------------ rig

    /// <summary>A hidden copy of the character at the origin, scale 1, facing +Z, that poses are
    /// built on and read back from.</summary>
    private sealed class Rig : IDisposable
    {
        private struct Snap { public Vector3[] P; public Quaternion[] R; }

        private readonly GameObject go;
        private readonly Animator anim;
        private readonly HumanPoseHandler handler;
        private readonly Transform[] all;
        private readonly Snap bind;
        private readonly Vector3 bindToeDirL, bindToeDirR;

        public readonly float Ground;        // sole height in root space
        public readonly float AnkleHeight;   // ankle joint above the sole
        public readonly float HumanScale;
        public readonly float UpperArmLen, ForeArmLen, ThighLen, ShinLen;
        public float ArmLen => UpperArmLen + ForeArmLen;
        public Transform UpperChest => B(HumanBodyBones.UpperChest) != null ? B(HumanBodyBones.UpperChest) : B(HumanBodyBones.Chest);

        public Rig(string fbxPath)
        {
            Avatar avatar = null;
            foreach (var o in AssetDatabase.LoadAllAssetsAtPath(fbxPath))
            {
                if (o is Avatar a) avatar = a;
            }

            go = (GameObject)Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(fbxPath));
            go.hideFlags = HideFlags.HideAndDontSave;
            go.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            go.transform.localScale = Vector3.one;
            anim = go.GetComponent<Animator>();
            if (anim == null) anim = go.AddComponent<Animator>();
            anim.avatar = avatar;
            handler = new HumanPoseHandler(avatar, go.transform);
            all = go.GetComponentsInChildren<Transform>(true);
            bind = Save();
            HumanScale = anim.humanScale;

            var smr = go.GetComponentInChildren<SkinnedMeshRenderer>();
            var baked = new UnityEngine.Mesh();
            smr.BakeMesh(baked, true);
            float minY = float.MaxValue;
            foreach (var v in baked.vertices) minY = Mathf.Min(minY, smr.transform.TransformPoint(v).y);
            Object.DestroyImmediate(baked);
            Ground = minY;

            AnkleHeight = B(HumanBodyBones.LeftFoot).position.y - Ground;
            UpperArmLen = Dist(HumanBodyBones.LeftUpperArm, HumanBodyBones.LeftLowerArm);
            ForeArmLen = Dist(HumanBodyBones.LeftLowerArm, HumanBodyBones.LeftHand);
            ThighLen = Dist(HumanBodyBones.LeftUpperLeg, HumanBodyBones.LeftLowerLeg);
            ShinLen = Dist(HumanBodyBones.LeftLowerLeg, HumanBodyBones.LeftFoot);
            bindToeDirL = B(HumanBodyBones.LeftToes).position - B(HumanBodyBones.LeftFoot).position;
            bindToeDirR = B(HumanBodyBones.RightToes).position - B(HumanBodyBones.RightFoot).position;
        }

        public void Dispose()
        {
            handler.Dispose();
            Object.DestroyImmediate(go);
        }

        public Transform B(HumanBodyBones b) => anim.GetBoneTransform(b);
        public Vector3 Shoulder(bool right) => B(right ? HumanBodyBones.RightUpperArm : HumanBodyBones.LeftUpperArm).position;
        private float Dist(HumanBodyBones a, HumanBodyBones b) => Vector3.Distance(B(a).position, B(b).position);

        public void Reset() => Load(bind);

        public HumanPose Capture()
        {
            var p = new HumanPose();
            handler.GetHumanPose(ref p);
            return p;
        }

        private Snap Save()
        {
            var s = new Snap { P = new Vector3[all.Length], R = new Quaternion[all.Length] };
            for (int i = 0; i < all.Length; i++)
            {
                s.P[i] = all[i].localPosition;
                s.R[i] = all[i].localRotation;
            }
            return s;
        }

        private void Load(Snap s)
        {
            for (int i = 0; i < all.Length; i++)
            {
                all[i].localPosition = s.P[i];
                all[i].localRotation = s.R[i];
            }
        }

        /// <summary>Forward bend (positive) around the character's right axis.</summary>
        public void Bend(Transform bone, float degrees)
        {
            if (bone != null) bone.rotation = Quaternion.AngleAxis(degrees, Vector3.right) * bone.rotation;
        }

        /// <summary>Turn to the right (positive) around the vertical.</summary>
        public void Turn(Transform bone, float degrees)
        {
            if (bone != null) bone.rotation = Quaternion.AngleAxis(degrees, Vector3.up) * bone.rotation;
        }

        public void Nod(float degrees)
        {
            Bend(B(HumanBodyBones.Neck), degrees * 0.5f);
            Bend(B(HumanBodyBones.Head), degrees * 0.5f);
        }

        /// <summary>Two-bone IK for an arm: wrist to <paramref name="target"/>, elbow toward
        /// <paramref name="pole"/>, fingers along <paramref name="fingerDir"/>, palm facing
        /// <paramref name="palmNormal"/>.</summary>
        public void ArmTo(bool right, Vector3 target, Vector3 pole, Vector3 fingerDir, Vector3 palmNormal)
        {
            Vector3 s = Shoulder(right);
            float a = UpperArmLen, b = ForeArmLen;
            Vector3 to = target - s;
            float d = Mathf.Clamp(to.magnitude, Mathf.Abs(a - b) + 1e-6f, (a + b) * 0.998f);
            Vector3 dir = to.normalized;
            float cosA = Mathf.Clamp((a * a + d * d - b * b) / (2f * a * d), -1f, 1f);
            Vector3 pn = Vector3.ProjectOnPlane(pole, dir);
            if (pn.sqrMagnitude < 1e-10f) pn = Vector3.ProjectOnPlane(Vector3.back, dir);
            pn.Normalize();
            Vector3 elbow = s + (dir * cosA + pn * Mathf.Sqrt(1f - cosA * cosA)) * a;
            Vector3 wrist = s + dir * d;

            SolveLimb(right ? HumanBodyBones.RightUpperArm : HumanBodyBones.LeftUpperArm,
                right ? HumanBodyBones.RightLowerArm : HumanBodyBones.LeftLowerArm,
                right ? HumanBodyBones.RightHand : HumanBodyBones.LeftHand,
                elbow, wrist, () => OrientHand(right, fingerDir, palmNormal),
                right ? HumanBodyBones.RightIndexProximal : HumanBodyBones.LeftIndexProximal,
                right ? HumanBodyBones.RightThumbProximal : HumanBodyBones.LeftThumbProximal);
        }

        /// <summary>
        /// Puts the middle joint at <paramref name="midPos"/> and the end at <paramref name="endPos"/>.
        /// Aiming the two bones fixes the directions but not the upper bone's twist, and a humanoid
        /// elbow/knee only bends one way - a wrong twist is silently lost when the pose is read
        /// back as muscles. So every twist is tried, the pose round-tripped through the muscle
        /// system, and the one that survives it best is kept.
        /// </summary>
        public void SolveLimb(HumanBodyBones upper, HumanBodyBones mid, HumanBodyBones end,
            Vector3 midPos, Vector3 endPos, Action orientEnd, params HumanBodyBones[] extra)
        {
            Transform u = B(upper), m = B(mid), e = B(end), hips = B(HumanBodyBones.Hips);
            var checks = new List<Transform> { m, e };
            foreach (var x in extra)
            {
                if (B(x) != null) checks.Add(B(x));
            }

            Snap start = Save();
            float best = float.MaxValue, bestTwist = 0f;
            var expected = new Vector3[checks.Count];
            for (int pass = 0; pass < 2; pass++)
            {
                float from = pass == 0 ? -180f : bestTwist - 6f;
                float to = pass == 0 ? 180f : bestTwist + 6f;
                float step = pass == 0 ? 6f : 1f;
                for (float tw = from; tw < to; tw += step)
                {
                    Load(start);
                    Aim(u, m, e, midPos, endPos, tw);
                    orientEnd?.Invoke();
                    for (int i = 0; i < checks.Count; i++) expected[i] = hips.InverseTransformPoint(checks[i].position);

                    var p = Capture();
                    handler.SetHumanPose(ref p);
                    float err = 0f;
                    for (int i = 0; i < checks.Count; i++) err += (hips.InverseTransformPoint(checks[i].position) - expected[i]).magnitude;
                    if (err < best)
                    {
                        best = err;
                        bestTwist = tw;
                    }
                }
            }

            Load(start);
            Aim(u, m, e, midPos, endPos, bestTwist);
            orientEnd?.Invoke();
        }

        private static void Aim(Transform u, Transform m, Transform e, Vector3 midPos, Vector3 endPos, float twist)
        {
            Vector3 upperDir = midPos - u.position;
            u.rotation = Quaternion.FromToRotation(m.position - u.position, upperDir) * u.rotation;
            u.rotation = Quaternion.AngleAxis(twist, upperDir.normalized) * u.rotation;
            m.rotation = Quaternion.FromToRotation(e.position - m.position, endPos - m.position) * m.rotation;
        }

        /// <summary>Fingers along <paramref name="fingerDir"/>, palm facing <paramref name="palmNormal"/>.</summary>
        public void OrientHand(bool right, Vector3 fingerDir, Vector3 palmNormal)
        {
            Transform hand = B(right ? HumanBodyBones.RightHand : HumanBodyBones.LeftHand);
            Transform index = B(right ? HumanBodyBones.RightIndexProximal : HumanBodyBones.LeftIndexProximal);
            Transform thumb = B(right ? HumanBodyBones.RightThumbProximal : HumanBodyBones.LeftThumbProximal);
            Vector3 f = fingerDir.normalized;
            Vector3 thumbDir = right ? Vector3.Cross(palmNormal, f) : Vector3.Cross(f, palmNormal);
            Vector3 cur = index.position - hand.position;
            Vector3 curThumb = Vector3.ProjectOnPlane(thumb.position - hand.position, cur);
            hand.rotation = Quaternion.LookRotation(f, Vector3.ProjectOnPlane(thumbDir, f))
                            * Quaternion.Inverse(Quaternion.LookRotation(cur, curThumb)) * hand.rotation;
        }

        /// <summary>Foot back to its standing angle, flat on the floor.</summary>
        public void OrientFootFlat(bool right)
        {
            Transform foot = B(right ? HumanBodyBones.RightFoot : HumanBodyBones.LeftFoot);
            Transform toe = B(right ? HumanBodyBones.RightToes : HumanBodyBones.LeftToes);
            foot.rotation = Quaternion.FromToRotation(toe.position - foot.position, right ? bindToeDirR : bindToeDirL) * foot.rotation;
        }
    }
}
