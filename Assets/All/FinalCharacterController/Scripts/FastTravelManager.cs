using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class FastTravelManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject fastTravelMenu;
    [SerializeField] private Button fastTravelButton;

    [Header("Player")]
    [SerializeField] private Transform player;

    [Header("Places")]
    [SerializeField] private Transform[] places;

    private void Start()
    {
        Debug.Log("Initializing Fast Travel Menu...");
        fastTravelButton.onClick.AddListener(OpenFastTravelMenu);
        fastTravelMenu.SetActive(false); // Ensure menu is hidden initially
        PopulateMenu();
    }

    //It iterates through the places array, logs each place, and creates a button for it
    private void PopulateMenu()
    {
        Debug.Log($"Populating Fast Travel Menu with {places.Length} places...");

        // Clear existing buttons to avoid duplicates
        foreach (Transform child in fastTravelMenu.transform)
        {
            Destroy(child.gameObject);
        }

        // Add buttons for each place
        foreach (Transform place in places)
        {
            Debug.Log($"Adding button for: {place.name}");
            Button placeButton = CreatePlaceButton(place.name);
            placeButton.onClick.AddListener(() => TeleportPlayerTo(place));
        }
    }

    //sets the button text, and returns the Button component
    private Button CreatePlaceButton(string placeName)
    {
        Debug.Log($"Creating button for: {placeName}");
        GameObject buttonObj = new GameObject(placeName, typeof(Button), typeof(RectTransform), typeof(Text));
        buttonObj.transform.SetParent(fastTravelMenu.transform);

        Text buttonText = buttonObj.GetComponent<Text>();
        buttonText.text = placeName;
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.alignment = TextAnchor.MiddleCenter;

        return buttonObj.GetComponent<Button>();
    }

    //Not functioning
    public void OpenFastTravelMenu()
    {
        Debug.Log("Opening Fast Travel Menu...");
        fastTravelMenu.SetActive(true); // Show the menu
    }

    public void CloseFastTravelMenu()
    {
        Debug.Log("Closing Fast Travel Menu...");
        fastTravelMenu.SetActive(false); // Hide the menu
    }

    //Teleport to the target place
    public void TeleportPlayerTo(Transform targetPlace)
    {
        Debug.Log($"Teleporting player to: {targetPlace.name}");
        if (player != null && targetPlace != null)
        {
            player.position = targetPlace.position; // Move the player
        }
        else
        {
            Debug.LogError("Player or Target Place is not assigned!");
        }

        // Close the menu after teleportation
        CloseFastTravelMenu();
    }
}
