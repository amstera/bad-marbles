using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreText : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    private int currentDisplayedScore = 0;
    private List<int> scoreUpdates = new List<int>();
    private Coroutine updateRoutine;
    private bool isPopping = false;

    public void UpdateScore(int newScore)
    {
        int index = scoreUpdates.BinarySearch(newScore);
        if (index < 0) index = ~index;
        scoreUpdates.Insert(index, newScore);

        if (updateRoutine == null)
        {
            updateRoutine = StartCoroutine(AnimatedUpdate());
        }
    }

    public void SetScoreImmediately(int newScore)
    {
        currentDisplayedScore = newScore;
        scoreText.text = newScore.ToString();
        scoreText.color = GetColorForScore(newScore);
    }

    private IEnumerator AnimatedUpdate()
    {
        while (scoreUpdates.Count > 0)
        {
            int newScore = scoreUpdates[0];
            scoreUpdates.RemoveAt(0);

            float maxDuration = 0.35f;

            // Calculate the duration for the score update animation
            int scoreDifference = Mathf.Abs(newScore - currentDisplayedScore);
            float duration = Mathf.Clamp((float)scoreDifference / 10, 0.1f, maxDuration);

            // Animate the score update
            float elapsed = 0;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                currentDisplayedScore = (int)Mathf.Lerp(currentDisplayedScore, newScore, elapsed / duration);
                scoreText.text = currentDisplayedScore.ToString();
                scoreText.color = GetColorForScore(currentDisplayedScore);

                TriggerPopEffect();

                yield return null;
            }

            currentDisplayedScore = newScore;
            scoreText.text = newScore.ToString();
        }

        updateRoutine = null;
    }

    private Color GetColorForScore(int score)
    {
        return Color.Lerp(Color.white, Color.green, Mathf.Clamp01(score / 1000f));
    }

    private void TriggerPopEffect()
    {
        if (!isPopping) // Only start the pop effect if it's not already happening
        {
            StartCoroutine(PopEffect());
        }
    }

    private IEnumerator PopEffect()
    {
        isPopping = true;
        float duration = 0.1f;
        float elapsed = 0;
        RectTransform rectTransform = scoreText.GetComponent<RectTransform>();

        Vector3 originalScale = rectTransform.localScale;
        Vector3 targetScale = originalScale * 1.2f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            rectTransform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / duration);
            yield return null;
        }

        rectTransform.localScale = originalScale;
        isPopping = false;
    }
}