using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using All.FinalCharacterController;

namespace All.FinalCharacterController
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private float locomotionBlendSpeed = 4f;

        private PlayerLocomotionInput _playerLocomotionInput;
        private bool _isTeleporting = false; // Flag to indicate teleportation

        private static int inputXHash = Animator.StringToHash("inputX");
        private static int inputYHash = Animator.StringToHash("inputY");

        private Vector3 _currentBlendInput = Vector3.zero;

        //Initializes _playerLocomotionInput
        //This setup is crucial to link input handling to animation control
        private void Awake()
        {
            _playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
        }

        //Checks if the player is currently teleporting. If true, it returns immediately, skipping any updates to the animation state
        private void Update()
        {
            if (_isTeleporting) return; // Skip animation updates during teleportation
            UpdateAnimationState();
        }

        //The UpdateAnimationState method updates the character's animation based on player movement input
        private void UpdateAnimationState()
        {
            Vector2 inputTarget = _playerLocomotionInput.MovementInput;
            _currentBlendInput = Vector3.Lerp(_currentBlendInput, inputTarget, locomotionBlendSpeed * Time.deltaTime);

            _animator.SetFloat(inputXHash, inputTarget.x);
            _animator.SetFloat(inputYHash, inputTarget.y);
        }

        // Methods for teleportation
        public void StartTeleport()
        {
            Debug.Log("Teleportation started. Pausing animations.");
            _isTeleporting = true;
        }

        public void EndTeleport()
        {
            Debug.Log("Teleportation ended. Resuming animations.");
            _isTeleporting = false;
        }
    }
}
