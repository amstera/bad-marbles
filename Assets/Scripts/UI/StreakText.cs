using UnityEngine;
using TMPro;
using System.Collections;

public class StreakText : MonoBehaviour
{
    public TextMeshProUGUI streakText;

    public AudioSource streakSound;

    private int currentMultiplier = 1;

    private void Awake()
    {
        streakText.gameObject.SetActive(false);
    }

    public void UpdateStreak(int streak)
    {
        if (streak >= 10)
        {
            int newMultiplier = streak / 10 + 1;
            if (newMultiplier != currentMultiplier)
            {
                currentMultiplier = newMultiplier;
                streakText.text = $"{currentMultiplier}X STREAK";
                streakText.gameObject.SetActive(true);
                StartCoroutine(PopAnimation());

                streakSound?.Play();
            }
        }
        else
        {
            currentMultiplier = 1; // Reset multiplier
            streakText.gameObject.SetActive(false);
        }
    }

    private IEnumerator PopAnimation()
    {
        // Enhanced scale animation
        float animationTime = 0.3f; // Shorter time for snappier animation
        Vector3 originalScale = streakText.transform.localScale;
        Vector3 targetScale = originalScale * 1.2f; // Scale up a bit more for emphasis

        // Scale up
        float timer = 0;
        while (timer < animationTime)
        {
            streakText.transform.localScale = Vector3.Lerp(originalScale, targetScale, timer / animationTime);
            timer += Time.deltaTime;
            yield return null;
        }

        // Scale down
        timer = 0;
        while (timer < animationTime)
        {
            streakText.transform.localScale = Vector3.Lerp(targetScale, originalScale, timer / animationTime);
            timer += Time.deltaTime;
            yield return null;
        }
    }
}