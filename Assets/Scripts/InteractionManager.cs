using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance;

    string currentText = "";
    int currentPriority = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple InteractionManagers in scene, destroying duplicate.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void RequestPrompt(string text, InteractionType type)
    {
        if (UIManager.Instance == null)
            return;

        int priority = (int)type;

        // Only update if this request has equal or higher priority
        if (priority >= currentPriority)
        {
            currentPriority = priority;
            currentText = text;

            UIManager.Instance.ShowInteractionPrompt(text);
        }
    }

    public void ClearPrompt(InteractionType type)
    {
        if (UIManager.Instance == null)
            return;

        // Only clear if the clearing object owns the current priority
        if ((int)type == currentPriority)
        {
            currentPriority = 0;
            currentText = "";
            UIManager.Instance.HideInteractionPrompt();
        }
    }
}
