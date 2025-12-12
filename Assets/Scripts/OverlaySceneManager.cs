using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class OverlaySceneManager
{
    private static readonly Stack<string> previousSceneStack = new Stack<string>();
    private static readonly Stack<List<GameObject>> previouslyActiveRootsStack = new Stack<List<GameObject>>();

    // Opens an overlay scene additively while deactivating the world scene
    public static void OpenOverlay(string overlaySceneName)
    {
        var currentActive = SceneManager.GetActiveScene();
        previousSceneStack.Push(currentActive.name);

        // Deactivate all active root objects in the current (world) scene
        var toReactivate = new List<GameObject>();
        var roots = currentActive.GetRootGameObjects();
        for (int i = 0; i < roots.Length; i++)
        {
            if (roots[i] != null && roots[i].activeSelf)
            {
                toReactivate.Add(roots[i]);
                roots[i].SetActive(false);
            }
        }
        previouslyActiveRootsStack.Push(toReactivate);

        var loadOp = SceneManager.LoadSceneAsync(overlaySceneName, LoadSceneMode.Additive);
        loadOp.completed += _ =>
        {
            var overlay = SceneManager.GetSceneByName(overlaySceneName);
            if (overlay.IsValid())
            {
                SceneManager.SetActiveScene(overlay);
            }
        };
    }

    // Closes the top-most overlay and reactivates the world scene exactly as it was
    public static void CloseTopOverlay()
    {
        if (previousSceneStack.Count == 0)
        {
            Debug.LogWarning("OverlaySceneManager: No previous scene to return to.");
            return;
        }

        var activeOverlay = SceneManager.GetActiveScene();
        var previousSceneName = previousSceneStack.Pop();
        var toReactivate = previouslyActiveRootsStack.Count > 0 ? previouslyActiveRootsStack.Pop() : null;

        var prevScene = SceneManager.GetSceneByName(previousSceneName);
        if (prevScene.IsValid())
        {
            // Reactivate exactly what we deactivated earlier
            if (toReactivate != null)
            {
                for (int i = 0; i < toReactivate.Count; i++)
                {
                    if (toReactivate[i] != null)
                    {
                        toReactivate[i].SetActive(true);
                    }
                }
            }

            SceneManager.SetActiveScene(prevScene);
        }

        // Restore gameplay cursor if available
        var cursorManager = Object.FindFirstObjectByType<NPCCursorManager>();
        if (cursorManager != null)
        {
            cursorManager.EnableGameplayCursor();
        }

        // Unload the overlay scene after switching back
        if (activeOverlay.IsValid())
        {
            SceneManager.UnloadSceneAsync(activeOverlay);
        }
    }
}
