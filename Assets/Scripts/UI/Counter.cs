using UnityEngine;
using TMPro;
using System.Collections;

public class Counter : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;

    public void Count(int start, int end, float totalTime)
    {
        if (textDisplay == null)
        {
            Debug.LogError("Text display not set!");
            return;
        }

        StartCoroutine(CountCoroutine(start, end, totalTime));
    }

    private IEnumerator CountCoroutine(int start, int end, float totalTime)
    {
        float currentTime = 0;
        while (currentTime < totalTime)
        {
            currentTime += Time.deltaTime;
            float t = currentTime / totalTime;
            int currentValue = Mathf.FloorToInt(Mathf.Lerp(start, end, t));
            textDisplay.text = currentValue.ToString();
            yield return null;
        }

        // Ensure the final value is set
        textDisplay.text = end.ToString();
    }
}