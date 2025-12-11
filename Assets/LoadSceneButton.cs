using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneButton : MonoBehaviour
{
    public Object sceneAsset;

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneAsset.name);
    }
}
