using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using All.FinalCharacterController;

namespace All.FinalCharacterController
{
    public class CameraSwitcher : MonoBehaviour
    {
        [Header("Cameras")]
        [SerializeField] private Camera dummyCamera;
        [SerializeField] private Camera birdEyeCamera;

        [Header("Bird's Eye Camera Zoom")]
        [SerializeField] private float zoomSpeed = 10f;
        [SerializeField] private float minZoom = 10f;
        [SerializeField] private float maxZoom = 50f;

        private Camera activeCamera;
        private Vector3 dragOrigin;

        private void Start()
        {
            // Set the initial active camera
            SetActiveCamera(dummyCamera);
        }

        //This method checks for player inputs to switch cameras or handle bird’s-eye view controls
        private void Update()
        {
            // Handle camera switching
            if (Input.GetKeyDown(KeyCode.C))
            {
                if (activeCamera == dummyCamera)
                {
                    SetActiveCamera(birdEyeCamera);
                }
                else
                {
                    SetActiveCamera(dummyCamera);
                }
            }

            // Handle zoom for the Bird's Eye Camera
            if (activeCamera == birdEyeCamera)
            {
                HandleBirdEyeZoom();
                HandleCameraDrag();
            }
        }

        
        private void SetActiveCamera(Camera cameraToActivate)
        {
            // Deactivate both cameras first
            dummyCamera.gameObject.SetActive(false);
            birdEyeCamera.gameObject.SetActive(false);

            // Activate the selected camera
            cameraToActivate.gameObject.SetActive(true);
            activeCamera = cameraToActivate;

            // Set the starting size only when the Bird's Eye Camera is activated for the first time
            if (cameraToActivate == birdEyeCamera && Mathf.Abs(birdEyeCamera.orthographicSize - 500f) > 0.1f)
            {
                birdEyeCamera.orthographicSize = 500f; // Only set if it's not already zoomed
            }
        }

        private void HandleBirdEyeZoom()
        {
            if (birdEyeCamera == null) return;

            // Get mouse scroll input
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (Mathf.Abs(scroll) > 0.01f) // Only adjust if there is significant input
            {
                // Adjust the orthographic size based on scroll input
                birdEyeCamera.orthographicSize -= scroll * zoomSpeed;

                // Clamp the zoom level to stay within minZoom and maxZoom
                birdEyeCamera.orthographicSize = Mathf.Clamp(birdEyeCamera.orthographicSize, minZoom, maxZoom);

                Debug.Log($"Zoom Level: {birdEyeCamera.orthographicSize}"); // Debug the current zoom level
            }
        }
        private void HandleCameraDrag()
        {
            if (birdEyeCamera == null || activeCamera != birdEyeCamera) return;

            // Detect left mouse button press to set drag origin
            if (Input.GetMouseButtonDown(0)) // Left mouse button
            {
                dragOrigin = Input.mousePosition;
            }

            // Drag the camera when the left mouse button is held
            if (Input.GetMouseButton(0))
            {
                Vector3 difference = birdEyeCamera.ScreenToViewportPoint(Input.mousePosition - dragOrigin);

                // Move the camera based on drag direction and speed
                Vector3 move = new Vector3(-difference.x, 0, -difference.y) * birdEyeCamera.orthographicSize;
                birdEyeCamera.transform.position += move;

                // Update drag origin for smooth dragging
                dragOrigin = Input.mousePosition;
            }
        }
        
    }
}
