#if UNITY_EDITOR
using Akila.FPSFramework.Internal;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;

namespace Akila.FPSFramework
{
    public class FirearmCreatorWindow : EditorWindow
    {
        public enum FirearmCreationPreset
        {
            AssaultRifle,
            Shotgun,
            Sniper,
            Pistol
        }

        private Vector2 scrollPosition; // To keep track of the scrollbar position

        private string firearmName = "Untitled Firearm";
        private FirearmCreationPreset preset;

        private static FirearmCreationPreset currentPreset;

        private static GameObject replacement;
        private static GameObject hud;
        private static GameObject crosshair;

        private bool useAdvanced;

        private Firearm.ShootingMechanism shootingMechanism;
        private Firearm.ShootingDirection shootingDirection;

        private static GameObject projectile;

        private float fireRate = 800;
        private float muzzleVelocity = 500;
        private float damage = 25;
        private float range = 200;

        private static bool autoGenerateAudioProfiles;
        private static bool autoGeneratePrefab;

        private static AudioProfile fireSound;
        private static AudioProfile reloadSound;

        private static AudioClip fireClip;
        private static AudioClip reloadClip;

        private static ParticleSystem muzzleFlash;

        // Add a menu item to open the window
        [MenuItem(MenuPaths.CreateFirearm)]
        public static void ShowWindow()
        {
            // Show existing window instance. If none exists, create one.
            EditorWindow window = GetWindow<FirearmCreatorWindow>(true, "Firearm Wizard", true);

            window.minSize = new Vector2(425, 606);

            hud = FindPrefab("0336a53be1116fe48935ea7a38bfc26e", "Firearm HUD");
            crosshair = FindPrefab("84e86c88a365a1c4ebfdf835fd34e678", "Crosshair");
            projectile = FindPrefab("788a3d3b8ecc33d4989926185282c05d", "Projectile");

            LoadAudioProfiles();
        }

        private void OnGUI()
        {
            // Define the mask area (exclude the last 10 units)
            Rect maskRect = new Rect(0, 0, position.width, position.height - 34);

            // Apply the mask
            GUI.BeginClip(maskRect);

            // Optional: Draw a subtle line to visualize the mask boundary
            Handles.color = Color.gray;
            Handles.DrawLine(new Vector3(0, position.height - 10), new Vector3(position.width, position.height - 10));


            // Start the scroll view
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Width(position.width), GUILayout.Height(position.height));

            currentPreset = preset;

            //Draw default and most basic settings
            DisplayBasic();

            //Draw the fileds that need to be assgined, fields like crosshair and HUD
            DisplayAssignables();

            //Draw advanced settings for people who want to create more diffrente guns
            DisplayAdvanced();

            DisplayAudioAndFX();

            DisplayUtilities();

            // End the scroll view
            EditorGUILayout.EndScrollView();

            GUI.EndClip();


            //Finally draw create button which opens a folder panel and creates a gun
            DisplayCreateButton();
        }

        private void DisplayBasic()
        {
            EditorGUILayout.Space();
            GUILayout.Label("BASIC", EditorStyles.boldLabel);

            firearmName = EditorGUILayout.TextField(new GUIContent("Name", "The name of the firearm to create. This name will be used to name the firearm preset and the firearm object."), firearmName);
            preset = (FirearmCreationPreset)EditorGUILayout.EnumPopup("Preset", preset);
            
        }

        private void DisplayAssignables()
        {
            EditorGUILayout.Space();

            GUILayout.Label("ASSIGNABLES", EditorStyles.boldLabel);
            replacement = (GameObject)EditorGUILayout.ObjectField("Replacement", replacement, typeof(GameObject), true);

            if (replacement?.TryGetComponent<Pickable>(out Pickable p) == false)
            {
                Debug.LogError($"'{replacement?.name}' is not a valid Pickable. Please assign a prefab with the 'Pickable' component attached.");

                replacement = null;
            }

            hud = (GameObject)EditorGUILayout.ObjectField("Firearm HUD", hud, typeof(GameObject), true);

            if (hud?.TryGetComponent<FirearmHUD>(out FirearmHUD h) == false)
            {
                Debug.LogError($"'{hud?.name}' is not a valid FirearmHUD. Please assign a prefab with the 'FirearmHUD' component attached.");

                hud = null;
            }

            crosshair = (GameObject)EditorGUILayout.ObjectField("Crosshair", crosshair, typeof(GameObject), true);

            if (crosshair?.TryGetComponent<Crosshair>(out Crosshair c) == false)
            {
                Debug.LogError($"'{crosshair?.name}' is not a valid Crosshair. Please assign a prefab with the 'Crosshair' component attached.");

                crosshair = null;
            }

            EditorGUILayout.Space();
        }

        private void DisplayAdvanced()
        {
            EditorGUILayout.BeginVertical("Box");

            useAdvanced = EditorGUILayout.ToggleLeft("ADVANCED", useAdvanced, EditorStyles.boldLabel);

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("Box");
            EditorGUI.BeginDisabledGroup(!useAdvanced);

            shootingMechanism = (Firearm.ShootingMechanism)EditorGUILayout.EnumPopup("Shooting Mechanism", shootingMechanism);
            shootingDirection = (Firearm.ShootingDirection)EditorGUILayout.EnumPopup("Shooting Direction", shootingDirection);

            if(shootingMechanism == Firearm.ShootingMechanism.Projectiles)
            {
                projectile = (GameObject)EditorGUILayout.ObjectField("Projectile", projectile, typeof(GameObject), true);
            }

            if (projectile?.TryGetComponent<Projectile>(out Projectile p) == false)
            {
                Debug.LogError($"'{projectile?.name}' is not a valid Projectile. Please assign a prefab with the 'Projectile' component attached.");

                projectile = null;
            }

            EditorGUI.indentLevel++;
            fireRate = EditorGUILayout.FloatField("Fire Rate", fireRate);
            muzzleVelocity = EditorGUILayout.FloatField("Muzzle Velocity", muzzleVelocity);
            damage = EditorGUILayout.FloatField("Damage", damage);
            range = EditorGUILayout.FloatField("Range", range);
            EditorGUI.indentLevel--;

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
        }

        private FirearmCreationPreset prevPreset;

        private void DisplayAudioAndFX()
        {
            EditorGUILayout.BeginVertical("Box");

            GUILayout.Label("Audio & FX", EditorStyles.boldLabel);

            EditorGUILayout.EndVertical();



            EditorGUILayout.BeginVertical("Box");
            
            if(prevPreset != preset)
            {
                LoadAudioProfiles();
            }

            EditorGUI.BeginDisabledGroup(autoGenerateAudioProfiles);
            fireSound = (AudioProfile)EditorGUILayout.ObjectField("Fire Sound", fireSound, typeof(AudioProfile), true);
            reloadSound = (AudioProfile)EditorGUILayout.ObjectField("Reload Sound", reloadSound, typeof(AudioProfile), true);
            EditorGUI.EndDisabledGroup();


            EditorGUILayout.Space();
            autoGenerateAudioProfiles = EditorGUILayout.Toggle("Auto Generate", autoGenerateAudioProfiles);

            EditorGUI.indentLevel++;
            
            EditorGUI.BeginDisabledGroup(!autoGenerateAudioProfiles);

            fireClip = (AudioClip)EditorGUILayout.ObjectField("Fire Audio Clip", fireClip, typeof(AudioClip), true);
            reloadClip = (AudioClip)EditorGUILayout.ObjectField("Reload Audio Clip", reloadClip, typeof(AudioClip), true);


            EditorGUI.EndDisabledGroup();

            EditorGUI.indentLevel--;

            EditorGUILayout.EndVertical();

            prevPreset = preset;
        }

        private void DisplayUtilities()
        {
            EditorGUILayout.BeginVertical("Box");

            GUILayout.Label("Utilities", EditorStyles.boldLabel);

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("Box");

            autoGeneratePrefab = EditorGUILayout.Toggle("Generate Prefab", autoGeneratePrefab);

            EditorGUILayout.HelpBox("Generating a prefab is usful because then you can use that prefab in a pickable and make your firearm a pickable weapon. Use the pickable wizard for faster results.", MessageType.Info);

            EditorGUILayout.EndVertical();
        }

        private void DisplayCreateButton()
        {
            float buttonWidth = 70,
                buttonHeight = 20,
                padding = 7;

            Rect buttonRect = new Rect(
                position.width - buttonWidth - padding,
                position.height - buttonHeight - padding,
                buttonWidth,
                buttonHeight);

            if (GUI.Button(buttonRect, "Create"))
                Create();
        }

        public void Create()
        {
            FirearmPreset asset = ScriptableObject.CreateInstance<FirearmPreset>();

            string path = EditorUtility.SaveFilePanelInProject(
                "Save ScriptableObject",
                firearmName,
                "asset",
                "Please enter a file name to save the ScriptableObject."
            );

            if (string.IsNullOrEmpty(path)) return;

            ApplyFirearmPreset(asset);

            if (crosshair)
                asset.crosshair = crosshair.GetComponent<Crosshair>();
            else
                asset.crosshair = null;

            if (hud)
                asset.firearmHud = hud.GetComponent<FirearmHUD>();
            else
                asset.firearmHud = null;

            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();

            GameObject defaultFirearm = FindPrefab("e3adfec6173060940ba6ad6097b94972", "Default Firearm");

            GameObject newFirearm = Instantiate(defaultFirearm);

            newFirearm.GetComponent<Firearm>().preset = asset;
            newFirearm.GetComponent<Firearm>().name = firearmName;
            newFirearm.GetComponent<Firearm>().replacement = replacement.GetComponent<Pickable>();

            newFirearm.transform.SetParent(Selection.activeTransform, false);

            newFirearm.transform.Reset();

            newFirearm.name = firearmName;

            if (autoGeneratePrefab)
            {
                bool result = false;

                GameObject prefab = PrefabUtility.SaveAsPrefabAsset(newFirearm, $"{System.IO.Path.GetDirectoryName(path)}/{firearmName}.prefab", out result);

                AssetDatabase.SaveAssets();

                DestroyImmediate(newFirearm.gameObject);

                GameObject newFirearmAsPrefab = (GameObject)PrefabUtility.InstantiatePrefab(prefab, Selection.activeTransform);

                Selection.activeObject = newFirearmAsPrefab;
            }
            else
            {
                Selection.activeObject = newFirearm;
            }

            if (autoGenerateAudioProfiles)
            {
                GenerateAudioProfiles(fireClip, reloadClip, System.IO.Path.GetDirectoryName(path), asset);
            }
            else
            {
                asset.fireSound = fireSound;
                asset.reloadSound = reloadSound;
            }

            if (useAdvanced)
            {
                asset.shootingDirection = shootingDirection;
                asset.shootingMechanism = shootingMechanism;
                asset.projectile = projectile.GetComponent<Projectile>();
                asset.fireRate = fireRate;
                asset.muzzleVelocity = muzzleVelocity;
                asset.range = range;
                asset.damage = damage;
            }

            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();

            FPSFrameworkEditor.EnterRenameMode();

            Close();
        }

        private void GenerateAudioProfiles(AudioClip fire, AudioClip reload, string path, FirearmPreset preset)
        {
            if (autoGenerateAudioProfiles == false) return;

            AudioProfile au_f = ScriptableObject.CreateInstance<AudioProfile>();
            AudioProfile au_r = ScriptableObject.CreateInstance<AudioProfile>();

            AssetDatabase.CreateAsset(au_f, $"{path}/{firearmName} Fire.asset");
            AssetDatabase.CreateAsset(au_r, $"{path}/{firearmName} Reload.asset");

            au_f.audioClip = fire;
            au_r.audioClip = reload;

            au_f.useRandomPitchOffset = true;
            au_r.useRandomPitchOffset = true;

            au_f.randomPitchOffset = 0.1f;
            au_r.randomPitchOffset = 0.1f;

            preset.fireSound = au_f;
            preset.reloadSound = au_r;

            AssetDatabase.SaveAssets();
        }
        

        private static void ApplyFirearmPreset(FirearmPreset firearmPreset)
        {
            Preset preset = null;

            string ar_guid = "64c9b02392daca0498df648bcccdb079";
            string sniper_guid = "9fe2cb05b34212942bd255db80b314f8";
            string shotgun_guid = "c985f3f4496731d41b02d6ee73020c0f";
            string pistol_guid = "6e916598804de1940a08b55f937a6ef6";

            string final_guid = null;

            switch(currentPreset)
            {
                case FirearmCreationPreset.AssaultRifle:
                    final_guid = ar_guid;
                    break;
                case FirearmCreationPreset.Sniper:
                    final_guid = sniper_guid;
                    break;
                case FirearmCreationPreset.Shotgun:
                    final_guid = shotgun_guid;
                    break;
                    case FirearmCreationPreset.Pistol:
                    final_guid = pistol_guid;
                    break;
            }

            string path = AssetDatabase.GUIDToAssetPath(final_guid);

            preset = AssetDatabase.LoadAssetAtPath<Preset>(path);

            preset.ApplyTo(firearmPreset);
        }

        private static void LoadAudioProfiles()
        {
            string ar_f_guid = "32a2f80fb0c67e64081a8e973e0f22c5";
            string ar_r_guid = "cd3d8a42beb20e64d90c2402498f37e3";

            string sni_f_guid = "e5e1842243349a545bda2d90cc685395";
            string sni_r_guid = "98c362cd6ac2c4e48845533df6f2af88";

            string shot_f_guid = "31e52a32b0f9831408c3126de7814af7";
            string shot_r_guid = "669d23cb73bae3b49b628eb231fd07c4";

            string pi_f_guid = "6fc414a5c7974da40a76e86234b199f4";
            string pi_r_guid = "31e2cd0af72bb72479102f6cd5972cfb";

            switch (currentPreset)
            {
                case FirearmCreationPreset.AssaultRifle:
                    fireSound = FindAudioProfile(ar_f_guid, "Assault Rifle_1 Fire");
                    reloadSound = FindAudioProfile(ar_r_guid, "Assault Rifle_1 Reload");
                    break;

                case FirearmCreationPreset.Sniper:
                    fireSound = FindAudioProfile(sni_f_guid, "Sniper_1 Fire");
                    reloadSound = FindAudioProfile(sni_r_guid, "Sniper_1_1 Reload");
                    break;

                case FirearmCreationPreset.Shotgun:
                    fireSound = FindAudioProfile(shot_f_guid, "Shotgun_1 Fire");
                    reloadSound = FindAudioProfile(shot_r_guid, "Shotgun_1_1 Reload");
                    break;

                case FirearmCreationPreset.Pistol:
                    fireSound = FindAudioProfile(pi_f_guid, "Pistol_1 Fire");
                    reloadSound = FindAudioProfile(pi_r_guid, "Pistol_1 Reload");
                    break;
            }
        }

        public static GameObject FindPrefab(string GUID, string name)
        {
            // Replace this with your prefab's GUID (from its .meta file)
            string targetGUID = GUID;

            // Convert GUID to an asset path
            string assetPath = AssetDatabase.GUIDToAssetPath(targetGUID);

            if (!string.IsNullOrEmpty(assetPath))
            {
                // Load the prefab at the asset path
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                if (prefab != null)
                {
                    return prefab;
                }
                else
                {
                    Debug.LogWarning("The asset is not a prefab or couldn't be loaded.");
                }
            }
            else
            {
                Debug.LogError($"Couldn't find '{name}' prefab. Make sure to reimport 'Prefabs' folder from the package manager.");
            }

            return null;
        }

        public static AudioProfile FindAudioProfile(string GUID, string name)
        {
            // Replace this with your prefab's GUID (from its .meta file)
            string targetGUID = GUID;

            // Convert GUID to an asset path
            string assetPath = AssetDatabase.GUIDToAssetPath(targetGUID);

            if (!string.IsNullOrEmpty(assetPath))
            {
                // Load the prefab at the asset path
                AudioProfile profile = AssetDatabase.LoadAssetAtPath<AudioProfile>(assetPath);

                if (profile != null)
                {
                    return profile;
                }
                else
                {
                    Debug.LogWarning("The asset is not a prefab or couldn't be loaded.");
                }
            }
            else
            {
                Debug.LogError($"Couldn't find '{name}' AudioProfile. Make sure to reimport 'Art' folder from the package manager.");
            }

            return null;
        }
    }
}
#endif