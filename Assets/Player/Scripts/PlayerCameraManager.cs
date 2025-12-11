using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCameraManager : MonoBehaviour
{
    private Camera playerCamera;

    void Awake()
    {
        // Find the camera in this player's children
        playerCamera = GetComponentInChildren<Camera>();
        if (playerCamera == null)
        {
            Debug.LogWarning("PlayerCameraManager: No Camera found in player children.");
            return;
        }

        // Listen for scene load
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (playerCamera == null) return;

        // Disable all other cameras in the newly loaded scene
        Camera[] allCameras = FindObjectsOfType<Camera>();
        foreach (Camera cam in allCameras)
        {
            if (cam != playerCamera)
                cam.gameObject.SetActive(false);
        }

        // Ensure player camera is active and tagged as MainCamera
        playerCamera.gameObject.SetActive(true);
        playerCamera.tag = "MainCamera";

        // Ensure only one AudioListener (on player camera)
        AudioListener[] listeners = FindObjectsOfType<AudioListener>();
        foreach (AudioListener listener in listeners)
        {
            if (listener.gameObject != playerCamera.gameObject)
                listener.enabled = false;
        }
    }
}