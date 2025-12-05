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
            // Safety Check: Make sure a scene is actually assigned
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("No scene assigned to this NPC!");
                return;
            }

            Debug.Log("Loading scene: " + sceneName);
            SceneManager.LoadScene(sceneName);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // "attachedRigidbody" automatically finds the Parent object that has the physics
        if (other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Player"))
        {
            Debug.Log("Interact with " + sceneName);
            playerIsClose = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Player"))
        {
            playerIsClose = false;
        }
    }
}