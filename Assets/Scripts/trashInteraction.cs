using UnityEngine;

public class TrashInteraction : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private Camera mainCamera;    // Your regular gameplay camera
    [SerializeField] private Camera zoomCamera;    // Your zoomed-in camera
    public TMPro.TextMeshProUGUI zoomText;

    [Header("Settings")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private bool playerIsClose = false;
    private bool isZoomedIn = false;

    void Awake()
    {
        // Fallback if you forget to assign the main camera in Inspector
        if (!mainCamera)
            mainCamera = Camera.main;

        // Make sure zoom camera starts disabled
        if (zoomCamera)
            zoomCamera.enabled = false;
    }

    void Update()
    {
        if (!playerIsClose) return;

        if (Input.GetKeyDown(interactKey))
        {
            if (!isZoomedIn)
                EnterZoom();
            else
                ExitZoom();
        }
    }

    private void EnterZoom()
    {
        isZoomedIn = true;
        zoomText.gameObject.SetActive(true);

        if (mainCamera) mainCamera.enabled = false;
        if (zoomCamera) zoomCamera.enabled = true;

        // Change prompt text
        InteractionManager.Instance?.RequestPrompt(
            $"Stop staring at the {gameObject.name}",
            InteractionType.NPC
        );
    }

    private void ExitZoom()
    {
        isZoomedIn = false;
        zoomText.gameObject.SetActive(false);

        if (mainCamera) mainCamera.enabled = true;
        if (zoomCamera) zoomCamera.enabled = false;

        // If player is still close, revert to "look closer" text
        if (playerIsClose)
        {
            InteractionManager.Instance?.RequestPrompt(
                $"Look closer at the {gameObject.name}",
                InteractionType.NPC
            );
        }
        else
        {
            // If they already walked away, just clear
            InteractionManager.Instance?.ClearPrompt(InteractionType.NPC);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Player"))
        {
            playerIsClose = true;

            // Show correct text depending on current zoom state
            if (isZoomedIn)
            {
                InteractionManager.Instance?.RequestPrompt(
                    $"Stop staring at the {gameObject.name}",
                    InteractionType.NPC
                );
            }
            else
            {
                InteractionManager.Instance?.RequestPrompt(
                    $"Look closer at the {gameObject.name}",
                    InteractionType.NPC
                );
            }

            Debug.Log("Player can now interact with " + gameObject.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Player"))
        {
            playerIsClose = false;

            // If player leaves while zoomed in, force exit zoom
            if (isZoomedIn)
            {
                ExitZoom();
            }
            else
            {
                InteractionManager.Instance?.ClearPrompt(InteractionType.NPC);
            }
        }
    }
}
