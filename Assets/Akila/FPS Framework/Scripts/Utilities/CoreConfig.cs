using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

namespace Akila.FPSFramework
{
    /// <summary>
    /// An internal class, that holds the data for all settings of the framework settings. Settings are accessed from here.
    /// This class should not be used for any other purposes other than the purpose it exists for. Please don't modify.
    /// </summary>
    [CreateAssetMenu]
    internal class CoreConfig : ScriptableObject
    {
        internal bool shortenMenus = false;
        internal bool automaticUpdatesChecking = true;
        internal float masterAudioVolume = 1;
        internal float globalAnimationWeight = 1;
        internal float globalAnimationSpeed = 1;
        internal int maxAnimationFramerate = 120;
        internal bool hasBeenSetup = false;

        internal MessageLevel debugLevel = MessageLevel.Normal;

        internal bool checkForUpdateThiSession = true;

        internal PackageSource packageSource;

        internal RenderPipelineType renderPipelineType;

        internal string url
        {
            get
            {
#if FPSFRAMEWORK_PRO
                string uas = "https://assetstore.unity.com/packages/templates/systems/fps-framework-pro-2-0-290322";
#else
                string uas = "https://assetstore.unity.com/packages/templates/systems/fps-framework-2-0-278978";
#endif
                string fab = "https://www.fab.com/listings/6350228f-f014-43d3-b0dd-3bd80d504ca8";

                return packageSource == PackageSource.UnityAssetStore ? uas : fab;
            }
        }

        internal string reviewUrl
        {
            get
            {
#if FPSFRAMEWORK_PRO
                string uas = "https://assetstore.unity.com/packages/templates/systems/fps-framework-pro-2-0-290322#reviews";
#else
                string uas = "https://assetstore.unity.com/packages/templates/systems/fps-framework-2-0-278978?srsltid=AfmBOopOvWUokOp_f7MVwHYRyhi1-HBmE45mEuc-wVVs2t8V028qqKSq#reviews";
#endif
                string fab = "https://www.fab.com/listings/6350228f-f014-43d3-b0dd-3bd80d504ca8";

                return packageSource == PackageSource.UnityAssetStore ? uas : fab;
            }
        }

        private void Awake()
        {
            checkForUpdateThiSession = true;

        }

        internal void ResetAllSettings()
        {
            shortenMenus = false;
            automaticUpdatesChecking = true;
            masterAudioVolume = 1;
            globalAnimationSpeed = 1;
            maxAnimationFramerate = 120;

#if UNITY_EDITOR
            FPSFrameworkCore.RemoveCustomDefineSymbol("FPS_FRAMEWORK_SHORTEN_MENUS");
#endif
        }

        internal enum PackageSource
        {
            UnityAssetStore,
            Fab
        }

        internal enum RenderPipelineType
        {
            BuiltIn,
            UniversalRP,
            HighDefinitionRP
        }

        [ContextMenu("Internal/Setup For UAS")]
        internal void SetupForUAS()
        {
            packageSource = PackageSource.UnityAssetStore;
        }

        [ContextMenu("Internal/Setup For Fab")]
        internal void SetupForFab()
        {
            packageSource = PackageSource.Fab;
        }
    }
}   