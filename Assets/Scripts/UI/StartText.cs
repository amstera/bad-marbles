using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StartText : MonoBehaviour
{
    public TextMeshProUGUI countdownText;
    public List<string> texts;
    private float animationDuration = 0.25f;

    void Start()
    {
        StartCoroutine(CountdownCoroutine());
    }

    IEnumerator CountdownCoroutine()
    {
        foreach (var text in texts)
        {
            yield return StartCoroutine(ShowText(text));
        }

        Destroy(gameObject);
    }

    IEnumerator ShowText(string number)
    {
        countdownText.text = number;
        float time = 0;

        // Custom scale up effect
        while (time < animationDuration)
        {
            float scale = EaseOutBack(time / animationDuration);
            countdownText.transform.localScale = new Vector3(scale, scale, scale);
            time += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f); // Pause at full scale

        // Custom scale down effect
        time = 0;
        while (time < animationDuration)
        {
            float scale = EaseInBack(time / animationDuration);
            countdownText.transform.localScale = new Vector3(1 - scale, 1 - scale, 1 - scale);
            time += Time.deltaTime;
            yield return null;
        }
    }

    float EaseOutBack(float x)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1;

        return 1 + c3 * Mathf.Pow(x - 1, 3) + c1 * Mathf.Pow(x - 1, 2);
    }

    float EaseInBack(float x)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1;

        return c3 * x * x * x - c1 * x * x;
    }
}
