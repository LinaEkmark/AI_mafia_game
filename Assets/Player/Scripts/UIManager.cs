using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Interaction Prompt UI")]
    [SerializeField] private GameObject interactionPanel;
    [SerializeField] private TextMeshProUGUI interactionPromptText;
    [SerializeField] private Image keyImage;  // E key icon (static for now)

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple UIManagers in scene, destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (interactionPanel != null)
        {
            interactionPanel.SetActive(false); // start hidden
        }
    }

    public void ShowKey()
    {
        if (keyImage != null)
        {
            keyImage.enabled = true;
        }
    }

    public void HideKey()
    {
        if (keyImage != null)
        {
            keyImage.enabled = false;
        }
    }

    public void ShowInteractionPrompt(string actionText)
    {
        if (interactionPanel == null || interactionPromptText == null)
            return;

        interactionPanel.SetActive(true);
        interactionPromptText.text = actionText;   // e.g. "Talk to Bob"
        // keyImage stays as the E icon, no need to change it here
    }

    public void HideInteractionPrompt()
    {
        if (interactionPanel == null)
            return;

        interactionPanel.SetActive(false);
    }
}
