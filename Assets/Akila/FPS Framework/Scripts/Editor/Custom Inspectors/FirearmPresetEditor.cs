#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Akila.FPSFramework.Internal
{
    [CustomEditor(typeof(FirearmPreset))]
    public class FirearmPresetEditor : Editor
    {
        private readonly string fireRateUnitLabel = "RPM";
        public static bool Foldout_Fire;
        public static bool Foldout_Recoil;
        public static bool Foldout_Movement;
        public static bool Foldout_Audio;
        private ReorderableList reorderableList;

        private void OnEnable()
        {
            // Initialize the ReorderableList
            reorderableList = new ReorderableList(
                serializedObject,
                serializedObject.FindProperty("restrictedAnimations"), // The serialized property of the string array
                true, // Display add/remove buttons
                true, // Display element dragger
                true, // Display header
                true  // Display element index
            );

            reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, element, GUIContent.none);
            };

            reorderableList.drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "Restricted Animations");
            };
        }

        public override void OnInspectorGUI()
        {
            FirearmPreset weapon = (FirearmPreset)target;

            Undo.RecordObject(weapon, $"Modified {weapon}");
            EditorGUI.BeginChangeCheck();

            UpdateBase(weapon);
            EditorGUILayout.Space();

            UpdateFire(weapon);
            UpdateRecoil(weapon);
            UpdateMovement(weapon);
            UpdateAudio(weapon);

            if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(weapon);
        }

        private void UpdateBase(FirearmPreset weapon)
        {
            EditorGUILayout.LabelField("Base", EditorStyles.boldLabel);

            weapon.firearmHud = EditorGUILayout.ObjectField(new GUIContent("Firearm HUD"), weapon.firearmHud, typeof(FirearmHUD), true) as FirearmHUD;
            weapon.crosshair = EditorGUILayout.ObjectField(new GUIContent("Crosshair"), weapon.crosshair, typeof(Crosshair), true) as Crosshair;
        }

        private LayerMask LayerMaskField(string label, LayerMask selected)
        {
            List<string> layers = new List<string>();
            List<int> layerNumbers = new List<int>();

            for (int i = 0; i < 32; i++)
            {
                string layerName = LayerMask.LayerToName(i);
                if (!string.IsNullOrEmpty(layerName))
                {
                    layers.Add(layerName);
                    layerNumbers.Add(i);
                }
            }

            int mask = selected.value;
            mask = EditorGUILayout.MaskField(label, mask, layers.ToArray());

            selected.value = mask;
            return selected;
        }

        private void UpdateFire(FirearmPreset weapon)
        {
            EditorGUILayout.BeginHorizontal("box");
            weapon.isFireActive = EditorGUILayout.Toggle(weapon.isFireActive, GUILayout.MaxWidth(28));
            Foldout_Fire = EditorGUILayout.Foldout(Foldout_Fire, "Fire", true);
            EditorGUILayout.EndHorizontal();

            if (!Foldout_Fire) return;
            EditorGUILayout.BeginVertical("box");

            EditorGUI.BeginDisabledGroup(!weapon.isFireActive);

            weapon.shootingMechanism = (Firearm.ShootingMechanism)EditorGUILayout.EnumPopup(new GUIContent(" Shooting Mechanism"), weapon.shootingMechanism);
            weapon.shootingDirection = (Firearm.ShootingDirection)EditorGUILayout.EnumPopup(new GUIContent(" Shooting Direction"), weapon.shootingDirection);
            weapon.fireMode = (Firearm.FireMode)EditorGUILayout.EnumPopup(new GUIContent(" Mode"), weapon.fireMode);
            weapon.casingDirection = (Vector3Direction)EditorGUILayout.EnumPopup(new GUIContent(" Casing Direction"), weapon.casingDirection);
            weapon.hittableLayers = LayerMaskField("Hittable Layers", weapon.hittableLayers);


            if (weapon.shootingMechanism == Firearm.ShootingMechanism.Projectiles)
            weapon.projectile = EditorGUILayout.ObjectField(new GUIContent(" Projectile"), weapon.projectile, typeof(Projectile), false) as Projectile;

            if (!weapon.projectile && weapon.shootingMechanism == Firearm.ShootingMechanism.Projectiles)
            {
                EditorGUILayout.HelpBox("Projectile must be assgined in order to use this section.", MessageType.Error);
                EditorGUILayout.Space();
            }

            if (weapon.projectile && weapon.shootingMechanism == Firearm.ShootingMechanism.Projectiles || weapon.shootingMechanism == Firearm.ShootingMechanism.Hitscan)
            {
                weapon.casing = EditorGUILayout.ObjectField(new GUIContent(" Casing"), weapon.casing, typeof(GameObject), false) as GameObject;

                EditorGUILayout.BeginHorizontal();
                weapon.fireRate = EditorGUILayout.FloatField(new GUIContent(" Fire Rate"), weapon.fireRate);

                EditorGUILayout.LabelField(fireRateUnitLabel, GUILayout.MaxWidth(33));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                weapon.muzzleVelocity = EditorGUILayout.FloatField(new GUIContent(" Muzzle Velocity"), weapon.muzzleVelocity);
                EditorGUILayout.LabelField("M/S", GUILayout.MaxWidth(33));
                EditorGUILayout.EndHorizontal();

                if (weapon.casing != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    weapon.casingVelocity = EditorGUILayout.FloatField(new GUIContent(" Casing Velocity"), weapon.casingVelocity);
                    EditorGUILayout.LabelField("M/S", GUILayout.MaxWidth(33));
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginHorizontal();
                weapon.range = EditorGUILayout.FloatField(new GUIContent(" Range"), weapon.range);
                EditorGUILayout.LabelField("M", GUILayout.MaxWidth(33));
                EditorGUILayout.EndHorizontal();

                weapon.impactForce = EditorGUILayout.FloatField(new GUIContent(" Impact Force"), weapon.impactForce);

                weapon.damage = EditorGUILayout.FloatField(new GUIContent(" Damage"), weapon.damage);
                weapon.maxAimDeviation = EditorGUILayout.FloatField(new GUIContent(" Max Aim Deviation"), weapon.maxAimDeviation);


                if (weapon.shootingMechanism == Firearm.ShootingMechanism.Hitscan)
                {
                    weapon.defaultDecal = EditorGUILayout.ObjectField(" Default Decal", weapon.defaultDecal, typeof(GameObject), true) as GameObject;
                    weapon.decalDirection = (Vector3Direction)EditorGUILayout.EnumPopup(new GUIContent(" Decal Direction"), weapon.decalDirection);
                }

                weapon.sprayPattern = EditorGUILayout.ObjectField(" Spray Pattern", weapon.sprayPattern, typeof(SprayPattern), true) as SprayPattern;
                weapon.aimSprayPattern = EditorGUILayout.ObjectField(" Aim Spray Pattern", weapon.aimSprayPattern, typeof(SprayPattern), true) as SprayPattern;
                
                EditorGUILayout.Space();
                serializedObject.Update();
                reorderableList.DoLayoutList();
                serializedObject.ApplyModifiedProperties();
                EditorGUILayout.Space();
               
                EditorGUILayout.LabelField(" Gamepad Settings", EditorStyles.boldLabel);
                weapon.gamepadVibrationAmountRight = EditorGUILayout.FloatField(new GUIContent(" Vibration Amount Right"), weapon.gamepadVibrationAmountRight);
                weapon.gamepadVibrationAmountLeft = EditorGUILayout.FloatField(new GUIContent(" Vibration Amount Left"), weapon.gamepadVibrationAmountLeft);
                weapon.gamepadVibrationDuration = EditorGUILayout.FloatField(new GUIContent(" Vibration Duration"), weapon.gamepadVibrationDuration);


                EditorGUILayout.Space();
                EditorGUILayout.LabelField(" Other Settings", EditorStyles.boldLabel);
                weapon.fireTransition = EditorGUILayout.FloatField(new GUIContent(" Fire Transition"), weapon.fireTransition);

                weapon.tracerRounds = EditorGUILayout.Toggle(new GUIContent(" Tracer Rounds"), weapon.tracerRounds);
                weapon.projectileSize = EditorGUILayout.FloatField(new GUIContent(" Projectile Size"), weapon.projectileSize);
                weapon.decalSize = EditorGUILayout.FloatField(new GUIContent(" Decal Size"), weapon.decalSize);
                weapon.shotCount = EditorGUILayout.IntField(new GUIContent(" Shot Count"), weapon.shotCount);

                if (weapon.shotCount <= 0)
                    weapon.shotCount = 1;

                if (weapon.shotCount > 1)
                {

                    EditorGUILayout.BeginHorizontal();
                    weapon.shotDelay = EditorGUILayout.FloatField(new GUIContent(" Shots Delay"), weapon.shotDelay);

                    if (GUILayout.Button(new GUIContent(" Calculate", "Calculates shot delay from fire rate")))
                    {
                        weapon.shotDelay = 1 / (weapon.fireRate / 60);
                    }

                    EditorGUILayout.EndHorizontal();

                    weapon.alwaysApplyFire = EditorGUILayout.Toggle(new GUIContent(" Always Apply Fire"), weapon.alwaysApplyFire);
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField(" Ammunition", EditorStyles.boldLabel);

                weapon.ammoType = EditorGUILayout.ObjectField("Ammo Profile", weapon.ammoType, typeof(AmmoProfileData), true) as AmmoProfileData;

                weapon.magazineCapacity = EditorGUILayout.IntField(new GUIContent(" Magazine Capacity"), weapon.magazineCapacity);
                weapon.reserve = EditorGUILayout.IntField(new GUIContent(" Reserve"), weapon.reserve);

                weapon.canAutomaticallyReload = EditorGUILayout.Toggle(" Automatic Reload", weapon.canAutomaticallyReload);
                weapon.canCancelReloading = EditorGUILayout.Toggle(" Reload Canceling", weapon.canCancelReloading);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField(" Reload", EditorStyles.boldLabel);

                weapon.reloadMethod = (Firearm.ReloadType)EditorGUILayout.EnumPopup(new GUIContent(" Reload Method"), weapon.reloadMethod);
                weapon.reloadTime = EditorGUILayout.FloatField(new GUIContent(" Reload Time"), weapon.reloadTime);

                if(weapon.reloadMethod == Firearm.ReloadType.Scripted)
                {
                    weapon.reloadStateName = EditorGUILayout.TextField(" Reload Animation State Name", weapon.reloadStateName);
                    weapon.reloadTransitionTime = EditorGUILayout.FloatField(" Reload Animation Transition Time", weapon.reloadTransitionTime);
                    EditorGUILayout.HelpBox("The scripted reloads are the type of reloads used in guns like shotguns and revolvers. In this type of reload, the firearm is only responsible for setting the reloading state and playing the reload animation. Reloading itself is done by throwing an event in the reload animation, calling the method \"Reload\" from the class \"WeaponeEvents.cs.\" This class must be attached to the game object that has the animator on it.", MessageType.Info);
                }

                if (weapon.reloadMethod == Firearm.ReloadType.Default)
                {

                    weapon.emptyReloadTime = EditorGUILayout.FloatField(new GUIContent(" Empty Reload Time"), weapon.emptyReloadTime);
                }
            }

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
        }

        private void UpdateRecoil(FirearmPreset weapon)
        {
            EditorGUILayout.BeginHorizontal("box");
            weapon.isRecoilActive = EditorGUILayout.Toggle(weapon.isRecoilActive, GUILayout.MaxWidth(28));
            Foldout_Recoil = EditorGUILayout.Foldout(Foldout_Recoil, "Recoil", true);
            EditorGUILayout.EndHorizontal();

            if (!Foldout_Recoil) return;
            EditorGUILayout.BeginVertical("box");

            EditorGUI.BeginDisabledGroup(!weapon.isRecoilActive);

            EditorGUILayout.LabelField(" Camera", EditorStyles.boldLabel);
            weapon.cameraRecoil = EditorGUILayout.FloatField(" Recoil", weapon.cameraRecoil);
            weapon.cameraShakeAmount = EditorGUILayout.FloatField(" Camera Shake Amount", weapon.cameraShakeAmount);
            weapon.cameraShakeRoughness = EditorGUILayout.FloatField(" Camera Shake Roughness", weapon.cameraShakeRoughness);
            weapon.cameraShakeStartTime = EditorGUILayout.FloatField(" Camera Shake Start Time", weapon.cameraShakeStartTime);
            weapon.cameraShakeDuration = EditorGUILayout.FloatField(" Camera Shake Duration", weapon.cameraShakeDuration);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(" Camera Recoil", EditorStyles.boldLabel);
            weapon.horizontalRecoil = EditorGUILayout.FloatField(" Horizontal Recoil", weapon.horizontalRecoil);
            weapon.verticalRecoil = EditorGUILayout.FloatField(" Vertical Recoil", weapon.verticalRecoil);

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
        }

        private void UpdateMovement(FirearmPreset weapon)
        {
            EditorGUILayout.BeginHorizontal("box");
            weapon.isCharacterActive = EditorGUILayout.Toggle(weapon.isCharacterActive, GUILayout.MaxWidth(28));
            Foldout_Movement = EditorGUILayout.Foldout(Foldout_Movement, "Character", true);
            EditorGUILayout.EndHorizontal();

            if (!Foldout_Movement) return;
            EditorGUILayout.BeginVertical("box");
            EditorGUI.BeginDisabledGroup(!weapon.isCharacterActive);

            weapon.basePlayerSpeed = EditorGUILayout.Slider(" Base Speed", weapon.basePlayerSpeed, 0, 1);
            weapon.aimWalkPlayerSpeed = EditorGUILayout.Slider(" Aim Walk Speed", weapon.aimWalkPlayerSpeed, 0, 1);
            weapon.fireWalkPlayerSpeed = EditorGUILayout.Slider(" Fire Walk Speed", weapon.fireWalkPlayerSpeed, 0, 1);

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
        }

        private void UpdateAudio(FirearmPreset weapon)
        {
            EditorGUILayout.BeginHorizontal("box");
            weapon.isAudioActive = EditorGUILayout.Toggle(weapon.isAudioActive, GUILayout.MaxWidth(28));
            Foldout_Audio = EditorGUILayout.Foldout(Foldout_Audio, "Audio", true);
            EditorGUILayout.EndHorizontal();

            if (!Foldout_Audio) return;
            EditorGUILayout.BeginVertical("box");
            EditorGUI.BeginDisabledGroup(!weapon.isAudioActive);

            weapon.fireSound = EditorGUILayout.ObjectField(new GUIContent(" Fire Sound"), weapon.fireSound, typeof(AudioProfile), true) as AudioProfile;

            weapon.fireTailSound = EditorGUILayout.ObjectField(new GUIContent(" Fire Tail Sound"), weapon.fireTailSound, typeof(AudioProfile), true) as AudioProfile;

            weapon.reloadSound = EditorGUILayout.ObjectField(new GUIContent(" Reload Sound"), weapon.reloadSound, typeof(AudioProfile), true) as AudioProfile;


            if (weapon.reloadMethod == Firearm.ReloadType.Default)
            {
                weapon.reloadEmptySound = EditorGUILayout.ObjectField(new GUIContent(" Reload Empty Sound"), weapon.reloadEmptySound, typeof(AudioProfile), true) as AudioProfile;
            }

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
        }
    }
}
#endif