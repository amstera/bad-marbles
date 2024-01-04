using UnityEngine;
using TMPro;
using System.Collections;

public class TutorialPanel : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI tutorialText;
    public TextMeshProUGUI titleText;
    private float fadeDuration = 0.2f;

    private void Awake()
    {
        canvasGroup.alpha = 0;
    }

    public void Show(string titleText, string text)
    {
        gameObject.SetActive(true);
        tutorialText.text = text;
        this.titleText.text = titleText;
        StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 1, fadeDuration));
    }

    public void Hide()
    {
        StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 0, fadeDuration, () => gameObject.SetActive(false)));
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
