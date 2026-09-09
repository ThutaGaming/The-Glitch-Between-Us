using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akila.FPSFramework
{
    public class MenuPaths
    {
#if FPS_FRAMEWORK_SHORTEN_MENUS
        public const string Akila = "Tools/FPS Framework/";
#else
        public const string Akila = "Tools/Akila/FPS Framework/";
#endif

#if FPS_FRAMEWORK_SHORTEN_MENUS
        public const string Create = "GameObject/FPS Framework/";
#else
        public const string Create = "GameObject/Akila/FPS Framework/";
#endif

        public const string CreateFPSController = Create + "FPS Controller";
        public const string CreateProceduralAnimation = Create + "Animation System/Procedural Animation";


        public const string CreateFirearm = Create + "Firearm/Firearm";

        public const string CreateAttachment = Create + "Firearm/Attachment";

        public const string CreateCarouselSelector = Create + "UI/Carousel Selector";
        public const string CreateSlider = Create + "UI/Slider";

        public const string Settings = Akila + "Settings";

        public const string Help = Akila + "Help";
    }
}