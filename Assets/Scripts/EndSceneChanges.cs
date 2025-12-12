using UnityEngine;

public class ObjectMovementEvent : MonoBehaviour
{
    [Header("Object to Watch")]
    [SerializeField] private Transform targetObject;
    [SerializeField] private Light lightToTurnOn;

    [Header("Lights to Dim/Adjust")]
    [SerializeField] private Light[] lightsToDim;
    [SerializeField] private float newIntensity = 0.2f;
    [SerializeField] private float newRange = 5f;

    [Header("Lights to Turn Off Completely")]
    [SerializeField] private Light[] lightsToTurnOff;

    [Header("Sound Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip newClip;

    private Vector3 initialPosition;
    private bool hasTriggered = false;

    void Start()
    {
        if (!targetObject)
        {
            Debug.LogError("ObjectMovementEvent: No targetObject assigned!");
            enabled = false;
            return;
        }

        initialPosition = targetObject.position;

        // Auto-find lights if you want (optional)
        if (lightsToDim == null || lightsToDim.Length == 0)
            lightsToDim = FindObjectsOfType<Light>();
    }

    void Update()
    {
        if (hasTriggered) return;

        // Trigger if object moved (small threshold to ignore float noise)
        if (Vector3.Distance(targetObject.position, initialPosition) > 0.05f)
        {
            TriggerEffects();
            hasTriggered = true;
        }
    }

    private void TriggerEffects()
    {
        Debug.Log("ObjectMovementEvent: Object moved → applying instant effects.");

        // Change intensity + range instantly
        foreach (var light in lightsToDim)
        {
            if (light != null)
            {
                light.intensity = newIntensity;
                light.range = newRange;
            }
        }

        // Turn off specific lights instantly
        foreach (var light in lightsToTurnOff)
        {
            if (light != null)
                light.enabled = false;
        }

        if (lightToTurnOn != null)
        {
            lightToTurnOn.enabled = true;
            lightToTurnOn.intensity = newIntensity;
            lightToTurnOn.range = newRange;
        }

        // Change sound instantly
        if (audioSource != null && newClip != null)
        {
            audioSource.Stop();
            audioSource.clip = newClip;
            audioSource.Play();
        }
    }
}