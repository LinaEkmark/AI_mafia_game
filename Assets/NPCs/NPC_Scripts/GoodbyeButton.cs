using UnityEngine;

public class GoodbyeButton : MonoBehaviour
{
    // Hook this up to the UI Button's OnClick event
    public void OnGoodbyeClicked()
    {
        OverlaySceneManager.CloseTopOverlay();
    }
}
