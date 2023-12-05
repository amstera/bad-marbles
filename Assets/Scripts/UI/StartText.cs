using UnityEngine;
using TMPro;
using System.Collections;

public class StartText : MonoBehaviour
{
    public TextMeshProUGUI countdownText;
    public float fadeDuration = 0.25f;
    public float displayDuration = 0.5f;
    private string[] countdownSequence = new string[] { "3", "2", "1", "START" };

    void Start()
    {
        StartCoroutine(CountdownSequence());
    }

    IEnumerator CountdownSequence()
    {
        foreach (var item in countdownSequence)
        {
            countdownText.text = item;
            yield return StartCoroutine(FadeTextToFullAlpha());
            yield return new WaitForSeconds(displayDuration);
            yield return StartCoroutine(FadeTextToZeroAlpha());
        }
    }

    IEnumerator FadeTextToFullAlpha()
    {
        countdownText.color = new Color(countdownText.color.r, countdownText.color.g, countdownText.color.b, 0);
        while (countdownText.color.a < 1.0f)
        {
            countdownText.color = new Color(countdownText.color.r, countdownText.color.g, countdownText.color.b, countdownText.color.a + (Time.deltaTime / fadeDuration));
            yield return null;
        }
    }

    IEnumerator FadeTextToZeroAlpha()
    {
        countdownText.color = new Color(countdownText.color.r, countdownText.color.g, countdownText.color.b, 1);
        while (countdownText.color.a > 0.0f)
        {
            countdownText.color = new Color(countdownText.color.r, countdownText.color.g, countdownText.color.b, countdownText.color.a - (Time.deltaTime / fadeDuration));
            yield return null;
        }
    }
}