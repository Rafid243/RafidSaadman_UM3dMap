using UnityEngine;
using All.FinalCharacterController; // Import the namespace

public class TeleportSystem : MonoBehaviour
{
    public Transform player; // Reference to ThirdPersonController
    public Transform startLocation;
    public Transform location1;
    public Transform location2; // Reference to the second new location
    public Transform location3; // Reference to the third new location
    public Transform location4; // Reference to the third new location

    private PlayerController _playerController;
    private PlayerAnimation _playerAnimation;
    private PlayerLocomotionInput _playerLocomotionInput;

    private void Start()
    {
        // Cache components from the player
        _playerController = player.GetComponent<PlayerController>();
        _playerAnimation = player.GetComponent<PlayerAnimation>();
        _playerLocomotionInput = player.GetComponent<PlayerLocomotionInput>();
    }

    public void TeleportToStart()
    {
        TeleportPlayer(startLocation.position);
    }

    public void TeleportToLocation1()
    {
        TeleportPlayer(location1.position);
    }
    public void TeleportToLocation2()
    {
        TeleportPlayer(location2.position);
    }

    public void TeleportToLocation3()
    {
        TeleportPlayer(location3.position);
    }
    public void TeleportToLocation4()
    {
        TeleportPlayer(location4.position);
    }

    //This private method performs the teleportation
    private void TeleportPlayer(Vector3 targetPosition)
    {
        var playerController = player.GetComponent<PlayerController>();

        if (playerController != null)
        {
            playerController.StartTeleport();
            playerController.TeleportPlayer(targetPosition);
            playerController.EndTeleport();
        }
    }

}
