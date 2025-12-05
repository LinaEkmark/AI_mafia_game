using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCInteraction : MonoBehaviour
{
    [Header("Settings")]
    // Type the unique name of this NPC in the Inspector (e.g., "Wizard", "Guard")
    [SerializeField] private string npcName;

    // The name of the scene to load (e.g., "DialogueScene")
    [SerializeField] private string sceneToLoad = "DialogueScene";

    [Header("Visual Cue (Optional)")]
    // Drag a floating text object or icon here (like a "Press E" prompt)
    [SerializeField] private GameObject interactPrompt;

    // --- DATA PASSING ---
    // This static variable belongs to the CLASS, not the object. 
    // It stays in memory even when the scene changes.
    public static string VisitedNPCName;
    // --------------------

    private bool isPlayerClose = false;

    private void Start()
    {
        // Hide the prompt at start
        if (interactPrompt != null) interactPrompt.SetActive(false);
    }

    private void Update()
    {
        // Only allow interaction if player is close AND presses E
        if (isPlayerClose && Input.GetKeyDown(KeyCode.E))
        {
            EnterDialogue();
        }
    }

    private void EnterDialogue()
    {
        // 1. Save the name of THIS specific NPC so the next scene can read it
        VisitedNPCName = npcName;

        // 2. Load the conversation scene
        SceneManager.LoadScene(sceneToLoad);
    }

    // --- PHYSICS DETECTION ---

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerClose = true;
            if (interactPrompt != null) interactPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerClose = false;
            if (interactPrompt != null) interactPrompt.SetActive(false);
        }
    }
}