using UnityEngine;
using TMPro;

public class ChatEntryStammerScanner : MonoBehaviour
{
    public TextMeshProUGUI messageText;

    void Start()
    {
        if (messageText == null)
            messageText = GetComponentInChildren<TextMeshProUGUI>();

        if (messageText == null) return;

        string reply = messageText.text;

        // Only analyze NPC messages
        if (reply.StartsWith("[YOU]:"))
            return;

        // Remove "[NPCName]:" prefix
        int idx = reply.IndexOf("]:");
        if (idx != -1)
        {
            reply = reply.Substring(idx + 2).Trim();
        }

        float stammerScore = StammerDetector.GetStammerScore(reply);

        // Send score to your glasses UI (only if it exists)
        StammerBars stammerBars = FindFirstObjectByType<StammerBars>();
        stammerBars?.SetStammerLevel(stammerScore);

        Debug.Log($"[STAMMER] '{reply}' → {stammerScore}");
    }
}
