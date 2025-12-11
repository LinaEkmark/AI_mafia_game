using UnityEngine;
using UnityEngine.SceneManagement;


public class NPCInteraction : MonoBehaviour
{
    [Header("Settings")]
    // field where you drag your Scene Asset
    // The #if UNITY_EDITOR part means this variable only exists while you are making the game.
#if UNITY_EDITOR
    public Object sceneAsset;
#endif

    [HideInInspector]
    [SerializeField] private string sceneName;

    private bool playerIsClose = false;

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (sceneAsset != null)
        {
            // Takes file and extracts the name
            sceneName = sceneAsset.name;
        }
#endif
    }

    // --- GAME LOGIC ---

    void Update()
    {
        if (playerIsClose && Input.GetKeyDown(KeyCode.E))
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("No scene assigned to this NPC!");
                return;
            }

            // Let InteractionManager clear the current NPC prompt
            InteractionManager.Instance?.ClearPrompt(InteractionType.NPC);

            Debug.Log("Loading scene: " + sceneName);
            NPCCursorManager cursorManager = FindFirstObjectByType<NPCCursorManager>();
            if (cursorManager != null)
            {
                cursorManager.EnableUICursor();
            }
            SceneManager.LoadScene(sceneName);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Player"))
        {
            playerIsClose = true;

            InteractionManager.Instance?.RequestPrompt(
                $"Press E to talk to {gameObject.name}",
                InteractionType.NPC
            );

            Debug.Log("Player can now interact with " + gameObject.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Player"))
        {
            playerIsClose = false;

            InteractionManager.Instance?.ClearPrompt(InteractionType.NPC);
        }
    }
}