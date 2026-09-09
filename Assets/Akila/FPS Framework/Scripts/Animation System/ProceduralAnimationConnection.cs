using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Serialization;

namespace Akila.FPSFramework.Animation
{
    [Serializable]
    public class ProceduralAnimationConnection
    {
        [FormerlySerializedAs("type")]
        public ProceduralAnimationConnectionType type = ProceduralAnimationConnectionType.AvoidInTrigger;
        [FormerlySerializedAs("clip")]
        public ProceduralAnimation target;
    }
}