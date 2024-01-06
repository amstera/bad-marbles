using UnityEngine;
using TMPro;
using System.Collections;

public class ComplimentText : MonoBehaviour
{
    public TextMeshProUGUI complimentText;

    private Coroutine fadeOutCoroutine;
    private string[] compliments = new string[] { "Awesome!", "Nice One!", "Don't Stop!", "Well Done!", "Keep Going!", "Marble-ous!", "Crushing It!", "Rock Solid!", "Smashing!", "Perfect!" };

    private void Awake()
    {
        complimentText.gameObject.SetActive(false);
    }

    public void ShowCompliment()
    {
        int randomIndex = Random.Range(0, compliments.Length);
        complimentText.text = compliments[randomIndex];
        complimentText.gameObject.SetActive(true);
        StartCoroutine(PopAnimation());
    }

    public void Hide()
    {
        if (fadeOutCoroutine != null)
        {
            StopCoroutine(fadeOutCoroutine);
        }
        complimentText.gameObject.SetActive(false);
    }

    private IEnumerator PopAnimation()
    {
        float animationTime = 0.3f;
        Vector3 originalScale = complimentText.transform.localScale;
        Vector3 targetScale = originalScale * 1.2f;

        Color originalColor = complimentText.color;
        Color fullColor = new Color(originalColor.r, originalColor.g, originalColor.b, 1);
        complimentText.color = fullColor;

        // Scale up
        float timer = 0;
        while (timer < animationTime)
        {
            complimentText.transform.localScale = Vector3.Lerp(originalScale, targetScale, timer / animationTime);
            timer += Time.deltaTime;
            yield return null;
        }

        // Scale down
        timer = 0;
        while (timer < animationTime)
        {
            complimentText.transform.localScale = Vector3.Lerp(targetScale, originalScale, timer / animationTime);
            timer += Time.deltaTime;
            yield return null;
        }

        // Start fade out coroutine
        if (fadeOutCoroutine != null)
        {
            StopCoroutine(fadeOutCoroutine);
        }
        fadeOutCoroutine = StartCoroutine(FadeOutText());
    }

    private IEnumerator FadeOutText()
    {
        float fadeDuration = 1f;
        Color originalColor = complimentText.color;
        Color transparentColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0);

        for (float t = 0; t < 1; t += Time.deltaTime / fadeDuration)
        {
            complimentText.color = Color.Lerp(originalColor, transparentColor, t);
            yield return null;
        }

        complimentText.gameObject.SetActive(false);
    }
}