using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Akila.FPSFramework
{
    /// <summary>
    /// Defines a spray pattern used for controlling weapon recoil behavior.
    /// </summary>
    [CreateAssetMenu(fileName = "New Spray Pattern", menuName = "Akila/FPS Framework/Weapons/Spray Pattern")]
    public class SprayPattern : ScriptableObject
    {
        [Serializable]
        public class SprayPatternPoint
        {
            /// <summary>
            /// Vertical component of the recoil (up/down).
            /// </summary>
            [Range(-1, 1)]
            public float upDown;

            /// <summary>
            /// Horizontal component of the recoil (right/left).
            /// </summary>
            [Range(-1, 1)]
            public float rightLeft;
        }

        public enum FixedPatternSource
        {
            Local = 0,
            External = 1
        }

        /// <summary>
        /// Maximum spread amount applied to the weapon.
        /// </summary>
        [FormerlySerializedAs("totalAmount")]
        public float maxAmount = 5f;
        public float verticalMultiplier = 1;
        public float horizontalMultiplier = 1;

        /// <summary>
        /// Obsolete: use <see cref="maxAmount"/> instead.
        /// </summary>
        [HideInInspector, Obsolete("Use maxAmount instead.")]
        public float totalAmount;

        /// <summary>
        /// Spread multiplier when player is not actively firing.
        /// </summary>
        [Range(0, 1)]
        public float passiveMultiplier = 0.1f;

        /// <summary>
        /// Time it takes for the spread to reach maximum after shooting begins.
        /// </summary>
        public float rampUpTime = 0.1f;

        /// <summary>
        /// Time it takes for the spread to recover when not shooting.
        /// </summary>
        public float recoveryTime = 0.05f;

        /// <summary>
        /// If true, uses random spray instead of fixed pattern.
        /// </summary>
        public bool isRandomized = true;

        /// <summary>
        /// Determines how the fixed spray pattern is sourced:
        /// - Independent: Uses this asset's own spray pattern.
        /// - Shared: Copies the spray pattern from another asset.
        /// </summary>
        public FixedPatternSource fixedPatternSource = FixedPatternSource.Local;

        /// <summary>
        /// The external spray pattern asset to use when the fixed pattern source is set to External.
        /// </summary>
        public SprayPattern externalSprayPatternToCopy;


        /// <summary>
        /// If enabled, the spray pattern will automatically generate intermediate points between spray dots,
        /// creating a smoother and more continuous pattern with less manual effort.
        /// </summary>
        public bool autoFill = false;

        /// <summary>
        /// If true and not randomized, the spray pattern will loop after reaching the end.
        /// </summary>
        public bool loop = true;

        [Space]

        /// <summary>
        /// Fixed spray pattern points for recoil calculation.
        /// </summary>
        public SprayPatternPoint[] points = new SprayPatternPoint[] {};

        /// <summary>
        /// Gradually ramps up the spread magnitude to full strength.
        /// </summary>
        /// <param name="value">Current spread value (updated).</param>
        /// <param name="velocity">Current ramp-up velocity (updated).</param>
        public void RampupMagnitude(ref float value, float deltaTime)
        {
            float rate = 1f / rampUpTime;

            value = Mathf.MoveTowards(value, 1f, rate * deltaTime);
        }

        /// <summary>
        /// Calculates the recoil offset based on the spray pattern or randomization.
        /// </summary>
        /// <param name="firearm">The firearm instance.</param>
        /// <param name="forward">The weapon's forward direction.</param>
        /// <param name="right">The weapon's right direction.</param>
        /// <param name="up">The weapon's up direction.</param>
        /// <param name="curvePosition">Unused (for future use or external curve support).</param>
        /// <param name="pointIndex">Index of the current spray point (updated).</param>
        /// <param name="amount">Custom spread amount override (-1 to use maxAmount).</param>
        /// <returns>New direction with applied spread.</returns>
        public Vector3 CalculatePattern(Firearm firearm, Vector3 forward, Vector3 right, Vector3 up, float curvePosition, ref int pointIndex, float amount = -1f)
        {
            float finalAmount = amount >= 0f ? amount : maxAmount;

            Vector3 offset = Vector3.zero;

            if (isRandomized)
            {
                offset = UnityEngine.Random.insideUnitSphere;
            }
            else
            {
                if (points != null && points.Length > 0)
                {
                    SprayPatternPoint[] points = null;

                    if (fixedPatternSource == FixedPatternSource.Local)
                    {
                        points = this.points;
                    }
                    else if (fixedPatternSource == FixedPatternSource.External)
                    {
                        if (externalSprayPatternToCopy != null)
                        {
                            points = externalSprayPatternToCopy.points;
                        }
                        else
                        {
                            points = this.points;

                            Debug.LogError("Failed to load external spray pattern: no asset assigned. Reverting to local pattern.", this);
                        }
                    }

                    int realPointCount = points.Length;

                    if (autoFill && points.Length >= 2)
                    {
                        int expandedLength = (realPointCount - 1) * 2 + 1;

                        // Include extra midpoint between last and first if looping
                        bool hasClosingMidpoint = loop;
                        if (hasClosingMidpoint) expandedLength += 1;

                        int expandedIndex = Mathf.Clamp(pointIndex, 0, expandedLength - 1);
                        bool isMidpoint = (expandedIndex % 2 == 1);
                        int baseIndex = expandedIndex / 2;

                        if (isMidpoint)
                        {
                            SprayPatternPoint a, b;

                            // Midpoint between last and first
                            if (hasClosingMidpoint && expandedIndex == expandedLength - 1)
                            {
                                a = points[realPointCount - 1];
                                b = points[0];
                            }
                            else
                            {
                                a = points[baseIndex];
                                b = points[baseIndex + 1];
                            }

                            float upDown = (a.upDown + b.upDown) * 0.5f;
                            float rightLeft = (a.rightLeft + b.rightLeft) * 0.5f;
                            offset = (up * upDown) + (right * rightLeft);
                        }
                        else
                        {
                            offset = (up * points[baseIndex].upDown) + (right * points[baseIndex].rightLeft);
                        }

                        pointIndex++;

                        if (pointIndex >= expandedLength && loop)
                            pointIndex = 0;
                    }
                    else
                    {
                        pointIndex = Mathf.Clamp(pointIndex, 0, realPointCount - 1);

                        var currPoint = points[pointIndex];
                        offset = (up * currPoint.upDown) + (right * currPoint.rightLeft);

                        pointIndex++;

                        if (pointIndex >= realPointCount && loop)
                            pointIndex = 0;
                    }
                }
            }

            // Apply spread and multipliers
            offset *= finalAmount * firearm.firearmAttachmentsManager.spread / 180f;
            offset.x *= horizontalMultiplier;
            offset.y *= verticalMultiplier;

            return forward + (offset * 2f); // Multiply to exaggerate effect for tuning
        }


        /// <summary>
        /// Smoothly recovers the spread value back toward the passive multiplier.
        /// </summary>
        /// <param name="currentValue">Current spread value (updated).</param>
        /// <param name="currentVelocity">Current recovery velocity (updated).</param>
        /// <param name="index">Index for current spray point (updated).</param>
        public void Recover(ref float currentValue, float deltaTime, ref int index)
        {
            float rate = 1f / recoveryTime;

            currentValue = Mathf.MoveTowards(currentValue, passiveMultiplier, rate * deltaTime);

            if (points != null && points.Length > 0)
                index = Mathf.Clamp(Mathf.RoundToInt((currentValue - passiveMultiplier) / (1f - passiveMultiplier) * (points.Length - 1)), 0, points.Length - 1);
            else
                index = 0;
        }
    }
}
