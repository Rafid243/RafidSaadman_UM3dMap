using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using All.FinalCharacterController;

namespace All.FinalCharacterController
{
    [DefaultExecutionOrder(-1)]
    public class PlayerController : MonoBehaviour
    {
        #region Class Variables
        [Header("Components")]
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private Camera _playerCamera;

        [Header("Base Movement")]
        public float runAcceleration = 35f;
        public float runSpeed = 4f;
        public float sprintAccelerataion = 50f;
        public float sprintSpeed = 7f;
        public float drag = 20f;
        public float movingThreshold = 0.01f;

        [Header("Camera Settings")]
        public float lookSenseH = 0.1f;
        public float lookSenseV = 0.1f;
        public float lookLimitV = 89f;

        private PlayerLocomotionInput _playerLocomotionInput;
        private PlayerState _playerState;

        private Vector2 _cameraRotation = Vector2.zero;
        private Vector2 _playerTargetRotation = Vector2.zero;

        private bool _isTeleporting = false; // Flag to disable movement during teleportation
        #endregion

        #region Startup


        private void Awake() 
        {
            _playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
            _playerState = GetComponent<PlayerState>();
        }
        #endregion

        #region Update Logic

        //Calls other methods if teleportation is not active
        private void Update()
        {
            if (_isTeleporting) return; // Skip movement updates during teleportation
            UpdateMovementState();
            HandleLateralMovement();
        }

        //Determines the player's current movement state
        private void UpdateMovementState()
        {
            bool isMovementInput = _playerLocomotionInput.MovementInput != Vector2.zero; // Order
            bool isMovingLaterally = IsMovingLaterally();                                // Matter
            bool isSprinting = _playerLocomotionInput.SprintToggledOn && isMovingLaterally; // Order matters

            PlayerMovementState lateralState = isSprinting ? PlayerMovementState.Sprinting :
                                               isMovingLaterally || isMovementInput ? PlayerMovementState.Running : PlayerMovementState.Idling;

            _playerState.SetPlayerMovementState(lateralState);
        }

        //Computes and applies the player’s new velocity based on input direction
        private void HandleLateralMovement()
        {
            // Create quick references for current state
            bool isSprinting = _playerState.CurrentPlayerMovementState == PlayerMovementState.Sprinting;

            float lateralAcceleration = isSprinting ? sprintAccelerataion : runAcceleration;
            float clampLateralMagnitude = isSprinting ? sprintSpeed : runSpeed;

            Vector3 cameraForwardXZ = new Vector3(_playerCamera.transform.forward.x, 0f, _playerCamera.transform.forward.z).normalized;
            Vector3 cameraRightXZ = new Vector3(_playerCamera.transform.right.x, 0f, _playerCamera.transform.right.z).normalized;
            Vector3 movementDirection = cameraRightXZ * _playerLocomotionInput.MovementInput.x + cameraForwardXZ * _playerLocomotionInput.MovementInput.y;

            Vector3 movementDelta = movementDirection * lateralAcceleration * Time.deltaTime;
            Vector3 newVelocity = _characterController.velocity + movementDelta;

            // Add drag to player
            Vector3 currentDrag = newVelocity.normalized * drag * Time.deltaTime;
            newVelocity = (newVelocity.magnitude > drag * Time.deltaTime) ? newVelocity - currentDrag : Vector3.zero;
            newVelocity = Vector3.ClampMagnitude(newVelocity, clampLateralMagnitude);

            // Move character (Unity suggests only calling this once per tick)
            _characterController.Move(newVelocity * Time.deltaTime);
        }
        #endregion

        #region Teleportation Logic

        //Methods to enable or disable movement during teleportation.
        public void StartTeleport()
        {
            Debug.Log("Teleportation started. Disabling movement.");
            _isTeleporting = true;
        }

        public void EndTeleport()
        {
            Debug.Log("Teleportation ended. Enabling movement.");
            _isTeleporting = false;
        }

        //Teleports the player to a specified position by temporarily disabling the CharacterController
        public void TeleportPlayer(Vector3 targetPosition)
        {
            Debug.Log($"Teleporting player to: {targetPosition}");

            if (_characterController != null)
            {
                _characterController.enabled = false; // Disable CharacterController temporarily
                transform.position = targetPosition;  // Move the player
                _characterController.enabled = true;  // Re-enable CharacterController
            }
            else
            {
                transform.position = targetPosition; // Directly move the player
            }

            Debug.Log($"Player teleported to: {transform.position}");
        }
        #endregion

        #region Late Update Logic

        //Updates the camera’s rotation based on the player’s input, restricted by defined limits, and aligns the player's rotation accordingly
        private void LateUpdate()
        {
            if (_isTeleporting) return; // Skip camera updates during teleportation
            _cameraRotation.x += lookSenseH * _playerLocomotionInput.LookInput.x;
            _cameraRotation.y = Mathf.Clamp(_cameraRotation.y - lookSenseV * _playerLocomotionInput.LookInput.y, -lookLimitV, lookLimitV);

            _playerTargetRotation.x += transform.eulerAngles.x + lookSenseH * _playerLocomotionInput.LookInput.x;
            transform.rotation = Quaternion.Euler(0f, _playerTargetRotation.x, 0f);

            _playerCamera.transform.rotation = Quaternion.Euler(_cameraRotation.y, _cameraRotation.x, 0f);
        }
        #endregion

        #region State Checks

        //Determines if the player is moving laterally by checking the magnitude of the velocity in the x-z plane against a defined threshold
        private bool IsMovingLaterally()
        {
            Vector3 lateralVelocity = new Vector3(_characterController.velocity.x, 0f, _characterController.velocity.y);

            return lateralVelocity.magnitude > movingThreshold;
        }
        #endregion
    }
}