using System.Collections;
using UnityEngine;
using TMPro;

public class BuildingInteraction : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buildingInfoText; // Text element for displaying building info
    [SerializeField] private float fadeStartTime = 9f; // Time after which fading starts
    [SerializeField] private float fadeDuration = 1f; // Duration of fading

    private Coroutine activeCoroutine;
    private GameObject lastClickedBuilding; // To track the last clicked building

    //This method is called once per frame and checks for mouse clicks
    private void Update()
    {
        // Check for mouse click
        if (Input.GetMouseButtonDown(0)) // Left-click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                BuildingInfo buildingInfo = hit.collider.GetComponent<BuildingInfo>();
                if (buildingInfo != null)
                {
                    HandleBuildingClick(hit.collider.gameObject, buildingInfo);
                }
            }
        }
    }

    //Checks if the same building was clicked again while its information panel is active
    //If a different building or the same building (with its panel currently inactive) is clicked, it updates the last clicked building reference
    private void HandleBuildingClick(GameObject clickedBuilding, BuildingInfo buildingInfo)
    {
        // Check if the same building is clicked and the panel is active
        if (lastClickedBuilding == clickedBuilding && buildingInfoText.transform.parent.gameObject.activeSelf)
        {
            // Deactivate the panel
            HideInfoBox();
            return;
        }

        // Update the last clicked building
        lastClickedBuilding = clickedBuilding;

        // Show the building info
        ShowBuildingInfo(buildingInfo);
    }

    //Sets the text of the buildingInfoText component to display relevant information
    private void ShowBuildingInfo(BuildingInfo buildingInfo)
    {
        // Set the text with extra spacing
        buildingInfoText.text = $"<b>Building Name:</b> {buildingInfo.buildingName}\n\n" +
                                $"<b>Information:</b> {buildingInfo.information}\n\n" +
                                $"<b>Email:</b> {buildingInfo.adminEmail}";

        // Show the panel (box)
        buildingInfoText.transform.parent.gameObject.SetActive(true);

        // Stop any previous coroutine
        if (activeCoroutine != null) StopCoroutine(activeCoroutine);

        // Start fading coroutine
        activeCoroutine = StartCoroutine(HideInfoBoxAfterDelay());
    }

    //Immediately hides the panel
    private void HideInfoBox()
    {
        // Stop any ongoing fade-out coroutine
        if (activeCoroutine != null) StopCoroutine(activeCoroutine);

        // Hide the panel immediately
        buildingInfoText.transform.parent.gameObject.SetActive(false);

        // Reset alpha in case it was mid-fade
        CanvasGroup canvasGroup = buildingInfoText.transform.parent.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }
    }

    private IEnumerator HideInfoBoxAfterDelay()
    {
        float elapsedTime = 0f;

        // Wait for fadeStartTime
        while (elapsedTime < fadeStartTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Start fading
        float fadeElapsed = 0f;
        CanvasGroup canvasGroup = buildingInfoText.transform.parent.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            while (fadeElapsed < fadeDuration)
            {
                fadeElapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, fadeElapsed / fadeDuration);
                yield return null;
            }
        }

        // Hide the panel
        buildingInfoText.transform.parent.gameObject.SetActive(false);
        if (canvasGroup != null) canvasGroup.alpha = 1f; // Reset alpha for next use
    }
}
