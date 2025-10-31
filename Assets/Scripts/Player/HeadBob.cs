using StarterAssets;
using UnityEngine;

namespace TMM
{
    public class HeadBob : MonoBehaviour
    {
        [Header("References")]
        public CharacterController controller;  // Reference to the player CharacterController
        public FirstPersonController fpController;

        public Transform cameraPivot;           // The pivot that the camera is attached to

        [Header("Headbob Settings")]
        public float walkBobSpeed = 8f;         // Speed of the headbob when walking
        public float walkBobAmount = 0.03f;     // Amplitude of the headbob when walking
        public float runBobSpeed = 13f;         // Speed of the headbob when running
        public float runBobAmount = 0.06f;      // Amplitude of the headbob when running
        public float crouchBobSpeed = 5f;       // Speed of the headbob when crouching
        public float crouchBobAmount = 0.015f;  // Amplitude of the headbob when crouching

        [Header("State Flags")]
        public bool isRunning = false;          // Set this externally from your player movement script
        public bool isCrouching = false;        // Same, set it from your movement script

        private float defaultYPos;              // The default Y position of the pivot
        private float timer;                    // Internal timer for the sine wave

        void Start()
        {
            if (cameraPivot == null) cameraPivot = transform;
            defaultYPos = cameraPivot.localPosition.y;
        }

        void Update()
        {
            if (controller == null) return;

            // Get horizontal velocity (ignore vertical movement)
            Vector3 velocity = new Vector3(controller.velocity.x, 0, controller.velocity.z);
            float speed = velocity.magnitude;

            // Check if the player is moving on the ground
            bool isMoving = speed > 0.1f && controller.isGrounded;

            if (isMoving)
            {
                isRunning = fpController.IsRunning;
                isCrouching = fpController.IsCrouching;

                float bobSpeed;
                float bobAmount;

                // Choose the correct bob settings based on player state
                if (isRunning)
                {
                    bobSpeed = runBobSpeed;
                    bobAmount = runBobAmount;
                }
                else if (isCrouching)
                {
                    bobSpeed = crouchBobSpeed;
                    bobAmount = crouchBobAmount;
                }
                else
                {
                    bobSpeed = walkBobSpeed;
                    bobAmount = walkBobAmount;
                }

                // Apply sine wave to Y position
                timer += Time.deltaTime * bobSpeed;
                float newY = defaultYPos + Mathf.Sin(timer) * bobAmount;
                cameraPivot.localPosition = new Vector3(cameraPivot.localPosition.x, newY, cameraPivot.localPosition.z);
            }
            else
            {
                // Smoothly return to default position when not moving
                timer = 0;
                Vector3 localPos = cameraPivot.localPosition;
                localPos.y = Mathf.Lerp(localPos.y, defaultYPos, Time.deltaTime * 5f);
                cameraPivot.localPosition = localPos;
            }
        }
    }
}