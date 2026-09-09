using UnityEngine;
using UnityEngine.InputSystem;

namespace Akila.FPSFramework
{
    public class ProcessorInitializer
    {
        [RuntimeInitializeOnLoadMethod]
        private static void InitializeProcessors()
        {
            // Register the custom Vector2 processor
            InputSystem.RegisterProcessor<AccelerateVector2Processor>();
        }
    }
}