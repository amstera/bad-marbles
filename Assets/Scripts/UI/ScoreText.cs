using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreText : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    private int currentDisplayedScore = 0;
    private Queue<int> scoreUpdates = new Queue<int>();
    private Coroutine updateRoutine;
    private bool isPopping = false;

    public void UpdateScore(int newScore)
    {
        scoreUpdates.Enqueue(newScore);

        if (updateRoutine == null)
        {
            updateRoutine = StartCoroutine(AnimatedUpdate());
        }
    }

    private IEnumerator AnimatedUpdate()
    {
        while (scoreUpdates.Count > 0)
        {
            int newScore;
            float maxDuration = 0.35f;

            if (scoreUpdates.Count <= 2) // Small queue size
            {
                yield return new WaitForSeconds(maxDuration);
                newScore = scoreUpdates.Dequeue();
            }
            else // Large queue size
            {
                // Sum up the scores for batch processing
                newScore = currentDisplayedScore;
                while (scoreUpdates.Count > 0)
                {
                    newScore += scoreUpdates.Dequeue() - currentDisplayedScore;
                }
            }

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
                scoreText.color = Color.Lerp(Color.white, Color.green, Mathf.Clamp01((float)currentDisplayedScore / 1000f));

                TriggerPopEffect();

                yield return null;
            }

            // Set the final score
            currentDisplayedScore = newScore;
            scoreText.text = newScore.ToString();
        }

        updateRoutine = null;
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