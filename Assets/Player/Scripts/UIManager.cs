using UnityEngine;
using TMPro; // Make sure to use TextMeshPro for modern UI

public class UIManager : MonoBehaviour
{
    // The TextMeshPro component that will display the prompt
    [Header("Interaction Prompt")]
    public TextMeshProUGUI interactionPromptText;

    // Use a static instance (Singleton pattern) for easy access from other scripts
    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        // Enforce the Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            // Optionally, make sure this object persists across scenes if needed
            // DontDestroyOnLoad(this.gameObject);
        }

        // Initially hide the prompt
        if (interactionPromptText != null)
        {
            interactionPromptText.gameObject.SetActive(false);
        }
    }

    // Public method for other scripts (like NPCInteraction) to call
    public void ShowInteractionPrompt(string message)
    {
        if (interactionPromptText != null)
        {
            interactionPromptText.text = message;
            interactionPromptText.gameObject.SetActive(true);
        }
    }

    // Public method to hide the prompt
    public void HideInteractionPrompt()
    {
        if (interactionPromptText != null)
        {
            interactionPromptText.gameObject.SetActive(false);
        }
    }
}