using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

/// <summary>
/// Place this on a door GameObject (with a Trigger collider). When the player is inside the trigger
/// and presses E (or the Interact key), the script checks the player's held item for a KeyItem.
/// If the key matches (or any key if no name set), the door consumes the key and loads the specified scene.
/// </summary>
public class DoorInteract : MonoBehaviour
{
    [Tooltip("Optional. If empty any KeyItem will open the door.")]
    public string requiredKeyName = "";

    [Tooltip("Scene name to load when door opens.")]
    public string sceneToLoad = "Party Room";

    [Tooltip("Optional delay before loading the scene (seconds)")]
    public float openDelay = 0f;

    [Header("Player transfer")]
    [Tooltip("If true, the player's root GameObject (the one containing the ItemHolder) will be preserved across the scene load via DontDestroyOnLoad and moved to a spawn point in the new scene.")]
    public bool preservePlayerAcrossScenes = false;
    [Tooltip("Tag to find a spawn Transform in the destination scene. If empty, DoorInteract will not relocate the preserved player.")]
    public string playerSpawnTag = "PlayerSpawn";

    ItemHolder currentPlayerHolder;
    bool playerInRange = false;

    void OnTriggerEnter(Collider other)
    {
        var ih = other.GetComponentInParent<ItemHolder>();
        if (ih != null)
        {
            currentPlayerHolder = ih;
            playerInRange = true;
            UpdatePrompt();
        }
    }

    void OnTriggerExit(Collider other)
    {
        var ih = other.GetComponentInParent<ItemHolder>();
        if (ih != null && ih == currentPlayerHolder)
        {
            playerInRange = false;
            currentPlayerHolder = null;
            UIManager.Instance?.HideInteractionPrompt();
        }
    }

    void Update()
    {
        if (!playerInRange || currentPlayerHolder == null) return;

        bool pressed = false;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame) pressed = true;
        }

        if (Input.GetKeyDown(KeyCode.E)) pressed = true;

        if (pressed)
            TryOpen();
    }

    void UpdatePrompt()
    {
        if (currentPlayerHolder == null)
        {
            UIManager.Instance?.HideInteractionPrompt();
            return;
        }

        var held = currentPlayerHolder.HeldItem;
        if (held == null)
        {
            UIManager.Instance?.ShowInteractionPrompt("Door is locked");
            return;
        }

        var key = held.GetComponent<KeyItem>();
        if (key != null && (string.IsNullOrEmpty(requiredKeyName) || key.keyName == requiredKeyName))
            UIManager.Instance?.ShowInteractionPrompt("Press E to open door");
        else
            UIManager.Instance?.ShowInteractionPrompt("Door is locked");
    }

    void TryOpen()
    {
        if (currentPlayerHolder == null) return;

        var held = currentPlayerHolder.HeldItem;
        var key = held?.GetComponent<KeyItem>();
        if (key != null && (string.IsNullOrEmpty(requiredKeyName) || key.keyName == requiredKeyName))
        {
            // capture the player root that holds the ItemHolder
            GameObject playerRoot = currentPlayerHolder.gameObject;
            while (playerRoot.transform.parent != null)
                playerRoot = playerRoot.transform.parent.gameObject;

            currentPlayerHolder.Drop();
            if (held != null)
                Destroy(held.gameObject);

            UIManager.Instance?.HideInteractionPrompt();

            if (preservePlayerAcrossScenes)
            {
                DontDestroyOnLoad(playerRoot);
            }

            if (openDelay > 0f)
                StartCoroutine(OpenAndHandlePlayerAfterDelay(openDelay, preservePlayerAcrossScenes));
            else
                StartCoroutine(OpenAndHandlePlayerAfterDelay(0f, preservePlayerAcrossScenes));
        }
        else
        {
            UIManager.Instance?.ShowInteractionPrompt("Door is locked");
        }
    }

    IEnumerator OpenAndHandlePlayerAfterDelay(float delay, bool preservePlayer)
    {
        if (delay > 0f) yield return new WaitForSeconds(delay);

        // Validate scene exists in Build Settings
        if (!Application.CanStreamedLevelBeLoaded(sceneToLoad))
        {
            Debug.LogError($"DoorInteract: Scene '{sceneToLoad}' is not in Build Settings or cannot be loaded. Add it to File->Build Settings.");
            yield break;
        }

        // Load the scene (single mode replaces current scene)
        var async = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Single);
        if (async == null)
        {
            Debug.LogError($"DoorInteract: Failed to start loading scene '{sceneToLoad}'");
            yield break;
        }

        while (!async.isDone)
            yield return null;

        // After load: relocate preserved player if requested
        if (preservePlayer)
        {
            // Find the preserved player root (should have been marked DontDestroyOnLoad)
            // We attempt to find any root objects that survived and contain an ItemHolder.
            ItemHolder preservedHolder = null;
            foreach (var root in Resources.FindObjectsOfTypeAll<ItemHolder>())
            {
                // skip those that belong to the newly loaded scene (they'll be bound to a scene)
                if (root.gameObject.scene.isLoaded) continue;
                preservedHolder = root;
                break;
            }

            if (preservedHolder != null)
            {
                GameObject preservedRoot = preservedHolder.gameObject;
                while (preservedRoot.transform.parent != null)
                    preservedRoot = preservedRoot.transform.parent.gameObject;

                if (!string.IsNullOrEmpty(playerSpawnTag))
                {
                    var spawn = GameObject.FindWithTag(playerSpawnTag);
                    if (spawn != null)
                    {
                        preservedRoot.transform.position = spawn.transform.position;
                        preservedRoot.transform.rotation = spawn.transform.rotation;
                    }
                    else
                    {
                        Debug.LogWarning($"DoorInteract: preservePlayerAcrossScenes is enabled but no object with tag '{playerSpawnTag}' was found in the destination scene. Player will not be relocated.");
                    }
                }

                // If desired, you can also re-parent preservedRoot into the new scene root, but DontDestroyOnLoad objects remain in a special scene.
            }
            else
            {
                Debug.LogWarning("DoorInteract: preservePlayerAcrossScenes was requested but preserved ItemHolder wasn't found after scene load.");
            }
        }
    }
}
