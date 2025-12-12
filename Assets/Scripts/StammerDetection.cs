using UnityEngine;
using System.Text.RegularExpressions;

public static class StammerDetector
{
    // Returns 0 = no stammer, 1 = heavy stammer
    public static float GetStammerScore(string reply)
    {
        if (string.IsNullOrEmpty(reply))
            return 0f;

        float score = 0f;

        string text = reply.ToLowerInvariant();

        // --- 1. Fragmented words (c-c-can, s-s-sure)
        if (Regex.IsMatch(text, @"\b(\w)-\1"))
            score += 0.5f;

        // --- 2. Repeated syllables (I-I-I, no-no-no)
        if (Regex.IsMatch(text, @"\b(\w+)-\1\b"))
            score += 0.5f;

        // --- 3. Long pauses (...)
        if (text.Contains("..."))
            score += 0.5f;

        // --- 4. Long pauses (...)
        if (text.Contains("!"))
            score += 0.5f;

        // --- 5. Hesitation words
        string[] hesitations = { "uh", "um", "er", "hmm", "hmmm" };
        foreach (string h in hesitations)
        {
            if (text.Contains(" " + h + " "))
            {
                score += 0.5f;
                break;
            }
        }

        // --- 6    . Sentence restart (e.g. "I... I think")
        if (Regex.IsMatch(text, @"\b(\w+)\.\.\.\s*\1\b"))
            score += 0.5f;

        return Mathf.Clamp01(score);
    }
}
