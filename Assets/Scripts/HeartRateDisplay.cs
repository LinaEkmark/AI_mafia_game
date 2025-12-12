using System.Collections;
using UnityEngine;
using TMPro;

public class HeartRateDisplay : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI heartRateText;
    public TMP_InputField listenInputField;

    [Header("Keyword Trigger")]
    [Tooltip("If this keyword appears in the input field text, stressed mode activates.")]
    public string stressedKeyword = "alert";

    [Header("Pace Settings (BPM)")]
    public Vector2 restingRange = new Vector2(60, 80);
    public Vector2 stressedRange = new Vector2(95, 130);

    [Header("Variability")]
    [Tooltip("Maximum change per update (BPM) for natural variability.")]
    public float maxDeltaPerTick = 3f;
    [Tooltip("Random noise amplitude added per update (BPM).")]
    public float noiseAmplitude = 1.5f;

    [Header("Update Rate")]
    [Tooltip("Updates per second. Use 2 for twice per second.")]
    public float updatesPerSecond = 2f;

    [Header("Mode")]
    [Tooltip("True = stressed mode. False = passive/resting.")]
    public bool stressedMode = false;
    [Tooltip("Latched state: once keyword triggers stress, remain stressed until manually reset.")]
    public bool stressedLatched = false;

    float currentBpm;
    Coroutine updateRoutine;

    void Awake()
    {
        // Initialize BPM within resting range
        currentBpm = Random.Range(restingRange.x, restingRange.y);
    }

    void OnEnable()
    {
        // Start periodic updates
        updateRoutine = StartCoroutine(UpdateLoop());

        // Listen to input changes for keyword detection
        if (listenInputField != null)
        {
            listenInputField.onValueChanged.AddListener(OnInputChanged);
            listenInputField.onSubmit.AddListener(OnInputSubmitted);
        }
    }

    void OnDisable()
    {
        if (updateRoutine != null)
        {
            StopCoroutine(updateRoutine);
            updateRoutine = null;
        }
        if (listenInputField != null)
        {
            listenInputField.onValueChanged.RemoveListener(OnInputChanged);
            listenInputField.onSubmit.RemoveListener(OnInputSubmitted);
        }
    }

    void OnInputChanged(string text)
    {
        CheckKeyword(text);
    }

    void OnInputSubmitted(string text)
    {
        CheckKeyword(text);
    }

    void CheckKeyword(string text)
    {
        if (string.IsNullOrEmpty(stressedKeyword)) return;
        if (string.IsNullOrEmpty(text)) { UpdateStressedMode(false); return; }

        // Case-insensitive contains
        if (text.ToLowerInvariant().Contains(stressedKeyword.ToLowerInvariant()))
        {
            // Latch stressed state on first detection
            stressedLatched = true;
            UpdateStressedMode(true);
        }
        else
        {
            // Do not clear if latched
            UpdateStressedMode(false);
        }
    }

    // Centralized mode update that respects latch
    void UpdateStressedMode(bool keywordDetected)
    {
        if (stressedLatched)
        {
            stressedMode = true;
            return;
        }

        stressedMode = keywordDetected;
    }

    // Call this to reset stress latch, e.g., at end of conversation
    public void ResetStress()
    {
        stressedLatched = false;
        stressedMode = false;
    }

    IEnumerator UpdateLoop()
    {
        float interval = 1f / Mathf.Max(0.1f, updatesPerSecond);
        while (true)
        {
            StepHeartRate();
            RenderText();
            yield return new WaitForSeconds(interval);
        }
    }

    void StepHeartRate()
    {
        // Choose target range based on mode
        Vector2 targetRange = stressedMode ? stressedRange : restingRange;

        // Aim towards the center of the range with a gentle pull
        float targetCenter = (targetRange.x + targetRange.y) * 0.5f;

        // Compute desired step toward target
        float toCenter = targetCenter - currentBpm;
        float drift = Mathf.Clamp(toCenter, -maxDeltaPerTick, maxDeltaPerTick);

        // Add believable small random noise
        float noise = Random.Range(-noiseAmplitude, noiseAmplitude);

        currentBpm += drift + noise;

        // Clamp to the range so it stays believable
        currentBpm = Mathf.Clamp(currentBpm, targetRange.x, targetRange.y);
    }

    void RenderText()
    {
        if (heartRateText == null) return;
        // Round to integer BPM for display
        int bpmRounded = Mathf.RoundToInt(currentBpm);
        heartRateText.text = $"Heartrate: {bpmRounded} bpm";
    }
}
