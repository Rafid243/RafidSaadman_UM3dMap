using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using All.FinalCharacterController;

namespace All.FinalCharacterController
{
    [DefaultExecutionOrder(-2)]
    public class PlayerLocomotionInput : MonoBehaviour, PlayerControls.IPlayerLocomotionMapActions
    {
        [SerializeField] private bool holdToSprint = true;

        public bool SprintToggledOn { get; private set; }
        public PlayerControls PlayerControls { get; private set; }
        public Vector2 MovementInput { get; private set; }
        public Vector2 LookInput { get; private set; }

        private bool _isTeleporting = false; // Flag to block input during teleportation

        //initializes the PlayerControls, enables them.
        private void OnEnable()
        {
            PlayerControls = new PlayerControls();
            PlayerControls.Enable();

            PlayerControls.PlayerLocomotionMap.Enable();
            PlayerControls.PlayerLocomotionMap.SetCallbacks(this);
        }

        //reverses this setup by disabling the input actions, ensuring clean deactivation and resource management
        private void OnDisable()
        {
            PlayerControls.PlayerLocomotionMap.Disable();
            PlayerControls.PlayerLocomotionMap.RemoveCallbacks(this);
        }

        //Handles movement inputs by reading the vector value from the input context and setting it to MovementInput
        public void OnMovement(InputAction.CallbackContext context)
        {
            if (_isTeleporting) return; // Ignore input during teleportation
            MovementInput = context.ReadValue<Vector2>();
            print(MovementInput);
        }

        //Similar to OnMovement, this method handles the direction the player is looking by updating LookInput
        public void OnLook(InputAction.CallbackContext context)
        {
            if (_isTeleporting) return; // Ignore input during teleportation
            LookInput = context.ReadValue<Vector2>();
        }

        //Not functioning
        public void OnToggleSprint(InputAction.CallbackContext context)
        {
            if (_isTeleporting) return; // Ignore input during teleportation

            if (context.performed)
            {
                SprintToggledOn = holdToSprint || !SprintToggledOn;
            }
            else if (context.canceled)
            {
                SprintToggledOn = !holdToSprint && SprintToggledOn;
            }
        }

        // Methods for teleportation
        public void StartTeleport()
        {
            Debug.Log("Teleportation started. Blocking input.");
            _isTeleporting = true;
        }

        public void EndTeleport()
        {
            Debug.Log("Teleportation ended. Resuming input.");
            _isTeleporting = false;
        }
    }
}


