using UnityEngine;

public class NPCCursorManager : MonoBehaviour
{
    // Call this to enter gameplay mode
    public void EnableGameplayCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Call this to enter UI mode
    public void EnableUICursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}