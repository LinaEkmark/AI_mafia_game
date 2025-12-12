using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum StoryPhase
{
    Phase1,
    Phase2,
    Phase3,
    Phase4
}

public class StoryManager : MonoBehaviour
{
    public static StoryManager Instance;

    public StoryPhase CurrentPhase = StoryPhase.Phase1;
    public Door door;

    private NPC.Manager NPCManager;
    private HashSet<string> collectedKeywords;

    // Keyword dictionary used for detecting current phase
    private Dictionary<StoryPhase, string[]> phaseKeywords = new Dictionary<StoryPhase, string[]>
    {
        {
            StoryPhase.Phase1,
            new string[] { "red hair" }
        },
        {
            StoryPhase.Phase2,
            new string[] { "breakfast cupcakes", "dating", "lamp" }
        },
        {
            StoryPhase.Phase3,
            new string[] { "interrogation room" }
        },
        {
            StoryPhase.Phase4,
            new string[] {  }
        }
    };


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object across scene loads
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }

        collectedKeywords = new HashSet<string>();
        NPCManager = new NPC.Manager();
    }


    // 🚀 Call this after every player question
    public void EvaluatePlayerMessage(string userMessage)
    {
        if (CurrentPhase == StoryPhase.Phase4)
            return; // Final phase reached

        string lower = userMessage.ToLower();
        string[] currentKeywords = phaseKeywords[CurrentPhase];

        foreach (var key in currentKeywords)
        {
            if (lower.Contains(key))
                collectedKeywords.Add(key);
        }

        if (collectedKeywords.SetEquals(currentKeywords.Select(k => k.ToLower())))
        {
            AdvanceTo((StoryPhase)((int)CurrentPhase + 1));
        }
    }



    private void AdvanceTo(StoryPhase newPhase)
    {
        Debug.Log($"Advancing story from {CurrentPhase} to {newPhase}");

        if (newPhase == StoryPhase.Phase4)
        {
            Debug.Log("Unlocking door for Tom in Phase 4");
            door.UnlockDoor();
        }

        CurrentPhase = newPhase;
        collectedKeywords.Clear();
    }

    public string GetPhasePrompt(string NPCName)
    {
        string prompt = NPCManager.GetNPCBehaviour(NPCName, (int)CurrentPhase);
        return prompt ?? $"Unknown NPC: {NPCName}";
    }
}
