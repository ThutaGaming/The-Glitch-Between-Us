using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;

namespace Akila.FPSFramework
{
    [System.Serializable]
    [InputControlLayout]
    public class AccelerateVector2Processor : InputProcessor<Vector2>
    {
        // Acceleration factor (how quickly the input accelerates)
        public float accelerationFactor = 2.0f;
        public float deaccelerationFactor = 100;

        // Speed limits for both axes (optional to cap the input speed)
        public float maxSpeedX = 10.0f;
        public float maxSpeedY = 10.0f;

        // Internal variables to track current speed
        private float currentSpeedX = 0.0f;
        private float currentSpeedY = 0.0f;

        // Override the Process method to apply acceleration
        public override Vector2 Process(Vector2 value, InputControl control)
        {
            // Apply acceleration to the X and Y components
            if (value.x != 0)
            {
                currentSpeedX = Mathf.Lerp(currentSpeedX, value.x, accelerationFactor * Time.deltaTime);
            }
            else
            {
                // Decelerate the speed when there's no input
                currentSpeedX = Mathf.MoveTowards(currentSpeedX, value.x, accelerationFactor * Time.deltaTime * deaccelerationFactor);
            }

            if (value.y != 0)
            {
                currentSpeedY = Mathf.Lerp(currentSpeedY, value.y, accelerationFactor * Time.deltaTime);
            }
            else
            {
                // Decelerate the speed when there's no input
                currentSpeedY = Mathf.MoveTowards(currentSpeedY, 0, accelerationFactor * Time.deltaTime * deaccelerationFactor);
            }

            // Multiply the resulting speed by the original multipliers for each axis
            Vector2 processedValue = new Vector2(currentSpeedX, currentSpeedY);

            return processedValue;
        }

        // Optional: Add a description for your processor
        public override string ToString()
        {
            return $"AccelerateVector2Processor (accelerationFactor = {accelerationFactor})";
        }
    }
}