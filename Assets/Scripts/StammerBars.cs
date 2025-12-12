using UnityEngine;
using UnityEngine.UI;

public class StammerBars : MonoBehaviour
{
    public Image greenBar;
    public Image yellowBar;
    public Image redBar;

    private void Start()
    {
        // Initialize all bars to inactive
        yellowBar.gameObject.SetActive(false);
        redBar.gameObject.SetActive(false);

    }

    // Update the indicator based on stammer score (0–1)
    public void SetStammerLevel(float value)
    {
        Debug.Log($"[STAMMER BARS] Set level to {value}");
        // Always show green (baseline)
        greenBar.gameObject.SetActive(true);

        // Nervous if above 0.3
        yellowBar.gameObject.SetActive(value > 0.3f);

        // Suspicious if above 0.6
        redBar.gameObject.SetActive(value > 0.6f);
    }
}
