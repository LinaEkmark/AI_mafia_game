using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneButton : MonoBehaviour
{
    // Set the scene name in Inspector
    public string sceneName = "Main Scene";
    public Button button;

    void Start()
    {
        if (button != null)
        {
            button.onClick.AddListener(LoadScene);
        }
    }

    public void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName); // loads scene by name
        }
        else
        {
            Debug.LogError("Scene name not set in the Inspector!");
        }
    }
}
