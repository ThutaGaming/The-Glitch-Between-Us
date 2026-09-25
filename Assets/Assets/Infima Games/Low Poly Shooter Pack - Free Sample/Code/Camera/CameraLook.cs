// Copyright 2021, Infima Games. All Rights Reserved.

using UnityEngine;

namespace InfimaGames.LowPolyShooterPack
{
    /// <summary>
    /// Camera Look. Handles the rotation of the camera.
    /// Standard FPS look: up/down is a clamped angle on this transform (the camera pivot), and
    /// left/right turns the character around world up only, so the body always stays upright
    /// and the view can never roll or flip over.
    /// </summary>
    public class CameraLook : MonoBehaviour
    {
        #region FIELDS SERIALIZED

        [Header("Settings")]

        [Tooltip("Sensitivity when looking around.")]
        [SerializeField]
        private Vector2 sensitivity = new Vector2(1, 1);

        [Tooltip("Minimum and maximum up/down rotation angle the camera can have. Negative looks up, positive looks down.")]
        [SerializeField]
        private Vector2 yClamp = new Vector2(-60, 60);

        [Tooltip("Should the look rotation be interpolated?")]
        [SerializeField]
        private bool smooth;

        [Tooltip("The speed at which the look rotation is interpolated.")]
        [SerializeField]
        private float interpolationSpeed = 25.0f;

        #endregion

        #region FIELDS

        /// <summary>
        /// Player Character.
        /// </summary>
        private CharacterBehaviour playerCharacter;
        /// <summary>
        /// The player character's rigidbody component.
        /// </summary>
        private Rigidbody playerCharacterRigidbody;

        /// <summary>
        /// Up/down angle of this transform, in degrees. Positive looks down.
        /// </summary>
        private float pitch;
        /// <summary>
        /// Left/right angle the character is turning towards. Only used when smoothing.
        /// </summary>
        private float yaw;

        /// <summary>
        /// True when this component sits on the character itself instead of on the camera pivot.
        /// Pitching the character would tilt the whole body, so such a copy only turns it.
        /// </summary>
        private bool onCharacter;

        #endregion

        #region UNITY

        private void Awake()
        {
            //Get Player Character.
            playerCharacter = ServiceLocator.Current.Get<IGameModeService>().GetPlayerCharacter();
            //Cache the rigidbody.
            playerCharacterRigidbody = playerCharacter.GetComponent<Rigidbody>();
            //Pitch belongs on the camera pivot, never on the body.
            onCharacter = transform == playerCharacter.transform;
            if (onCharacter)
                Debug.LogWarning("CameraLook on the character root only turns it left/right. Keep a single CameraLook, on the camera pivot.", this);
        }
        private void OnEnable()
        {
            //Pick up wherever the view was left while this was switched off (cutscenes, terminals).
            if (playerCharacterRigidbody != null)
                SyncFromTransforms();
        }
        private void Start()
        {
            SyncFromTransforms();
        }
        private void LateUpdate()
        {
            //Frame Input. The Input to add this frame!
            Vector2 frameInput = playerCharacter.IsCursorLocked() ? playerCharacter.GetInputLook() : default;
            //Reject a NaN/Infinity input sample (seen from the Input System on some focus/frame
            //edge cases) before it corrupts the look angles for every frame afterwards.
            if (float.IsNaN(frameInput.x) || float.IsNaN(frameInput.y)
                || float.IsInfinity(frameInput.x) || float.IsInfinity(frameInput.y))
                frameInput = Vector2.zero;
            //Sensitivity.
            frameInput *= sensitivity;

            //Pitch. A plain clamped angle, so the view stops at the limits instead of flipping over.
            pitch = Mathf.Clamp(pitch - frameInput.y, yClamp.x, yClamp.y);
            Quaternion rotationPitch = Quaternion.Euler(pitch, 0.0f, 0.0f);

            //Smooth.
            if (smooth)
            {
                yaw += frameInput.x;
                //Interpolate local rotation.
                if (!onCharacter)
                    transform.localRotation = Quaternion.Slerp(transform.localRotation, rotationPitch, Time.deltaTime * interpolationSpeed);
                //Interpolate character rotation. Yaw only, so the body stays upright.
                playerCharacterRigidbody.MoveRotation(Quaternion.Slerp(playerCharacterRigidbody.rotation,
                    Quaternion.Euler(0.0f, yaw, 0.0f), Time.deltaTime * interpolationSpeed));
            }
            else
            {
                //Rotate local. Pitch only - nothing else can build up on the camera pivot.
                if (!onCharacter)
                    transform.localRotation = rotationPitch;
                //Rotate character around world up. Any tilt it picked up is dropped here too.
                yaw = playerCharacterRigidbody.rotation.eulerAngles.y + frameInput.x;
                playerCharacterRigidbody.MoveRotation(Quaternion.Euler(0.0f, yaw, 0.0f));
            }
        }

        #endregion

        #region FUNCTIONS

        /// <summary>
        /// Reads the current up/down and left/right angles back from the transforms.
        /// </summary>
        private void SyncFromTransforms()
        {
            yaw = playerCharacterRigidbody.rotation.eulerAngles.y;
            if (onCharacter)
                return;

            float x = transform.localEulerAngles.x;
            pitch = Mathf.Clamp(x > 180.0f ? x - 360.0f : x, yClamp.x, yClamp.y);
        }

        #endregion
    }
}
