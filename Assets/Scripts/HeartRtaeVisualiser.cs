using UnityEngine;
using UnityEngine.UI;

public class HeartPulseVisualiser : MonoBehaviour
{
    [Header("References")]
    public HeartRateDisplay heartRateDisplay; // your existing script
    public Image heartImage;                  // UI Image component of the heart

    [Header("Pulse Settings")]
    public float baseScale = 1f;
    public float scaleAmplitude = 0.2f;

    [Header("Color Settings")]
    public float minBpm = 60f;   // BPM considered "safe" (green)
    public float maxBpm = 130f;  // BPM considered "high" (red)
    public Color lowColor = Color.green;
    public Color highColor = Color.red;

    private float timer = 0f;

    void Update()
    {
        if (heartRateDisplay == null || heartImage == null) return;

        // --- Pulse the heart ---
        float bps = heartRateDisplay.currentBpm / 60f;
        timer += Time.deltaTime * bps * Mathf.PI * 2f;
        float scale = baseScale + Mathf.Sin(timer) * scaleAmplitude;
        heartImage.transform.localScale = new Vector3(scale, scale, 1f);

        // --- Color interpolation ---
        float t = Mathf.InverseLerp(minBpm, maxBpm, heartRateDisplay.currentBpm);
        heartImage.color = Color.Lerp(lowColor, highColor, t);
    }
}
