using UnityEngine;

// Simple marker component for key objects.
public class KeyItem : MonoBehaviour
{
    [Tooltip("Optional identifier for this key. Leave empty to match any key.")]
    public string keyName = "";
}
