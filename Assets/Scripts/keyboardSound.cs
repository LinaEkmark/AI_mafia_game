using UnityEngine;

public class KeyboardSound : MonoBehaviour
{

    [Header("Components")]
    public AudioSource audioSource;

    [Header("Settings")]
    public AudioClip keyboardSound;

    bool typing = false;

    void Awake()
    {
        if (!audioSource)
            audioSource = GetComponentInParent<AudioSource>();
    }

    public void Keyboard()
    {
        if (keyboardSound != null)
        {
            typing = !typing;  // Toggle the bool

            if (typing)
            {
                audioSource.clip = keyboardSound;
                audioSource.Play();
            }
            else
            {
                audioSource.Stop();
            }
        }
    }
}
