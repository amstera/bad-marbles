using UnityEngine;
using TMPro;
using System.Collections;

public class TutorialPanel : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI tutorialText;
    public TextMeshProUGUI titleText;
    private float fadeDuration = 0.2f;
    private bool isAttentionLoopActive = false;

    private void Awake()
    {
        canvasGroup.alpha = 0;
    }

    public void Show(string title, string text)
    {
        gameObject.SetActive(true);
        tutorialText.text = text;
        titleText.text = title;
        StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 1, fadeDuration));
        StopAttentionDrawing();
    }

    public void Hide()
    {
        StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 0, fadeDuration, () => gameObject.SetActive(false)));
        StopAttentionDrawing();
    }

    public void DrawAttention()
    {
        if (!isAttentionLoopActive)
        {
            StartCoroutine(PopTitleText());
        }
    }

    private IEnumerator PopTitleText()
    {
        isAttentionLoopActive = true;
        Vector3 originalScale = titleText.transform.localScale;
        Vector3 targetScale = originalScale * 1.5f;
        float popDuration = 0.25f;

        // Smoothly scale up
        float elapsedTime = 0;
        while (elapsedTime < popDuration)
        {
            titleText.transform.localScale = Vector3.Lerp(originalScale, targetScale, (elapsedTime / popDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure it reaches the target scale in case of timing discrepancies
        titleText.transform.localScale = targetScale;

        // Reset elapsed time for scale down
        elapsedTime = 0;
        while (elapsedTime < popDuration)
        {
            titleText.transform.localScale = Vector3.Lerp(targetScale, originalScale, (elapsedTime / popDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure it returns to original scale
        titleText.transform.localScale = originalScale;
        isAttentionLoopActive = false;
    }

    private void StopAttentionDrawing()
    {
        isAttentionLoopActive = false;
        titleText.fontSize = titleText.fontSize;
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration, System.Action onComplete = null)
    {
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }
        cg.alpha = end;

        onComplete?.Invoke();
    }
}