using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class Toast : MonoBehaviour, IPointerClickHandler
{
    public TextMeshProUGUI textMeshPro;
    private Vector2 startAnchoredPosition;
    private float moveDuration = 0.5f;
    private float displayTime = 4.0f;
    private bool isRetracting = false;

    void Awake()
    {
        gameObject.SetActive(false);
        startAnchoredPosition = GetComponent<RectTransform>().anchoredPosition;
    }

    public void Show(string text)
    {
        gameObject.SetActive(true);
        textMeshPro.text = text;
        isRetracting = false;
        StartCoroutine(ShowToast());
    }

    private IEnumerator ShowToast()
    {
        // Move down
        yield return MoveToAnchoredPosition(new Vector2(0, -45), moveDuration);

        // Wait for display time or early click
        float timer = 0;
        while (timer < displayTime && !isRetracting)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // Move back to start position
        yield return MoveToAnchoredPosition(startAnchoredPosition, moveDuration);

        gameObject.SetActive(false);
    }

    private IEnumerator MoveToAnchoredPosition(Vector2 targetAnchoredPosition, float duration)
    {
        float time = 0;
        Vector2 currentAnchoredPosition = GetComponent<RectTransform>().anchoredPosition;

        while (time < duration)
        {
            GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(currentAnchoredPosition, targetAnchoredPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        GetComponent<RectTransform>().anchoredPosition = targetAnchoredPosition;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Start retracting early if clicked/tapped
        if (!isRetracting)
        {
            isRetracting = true;
            StopCoroutine(ShowToast()); // Stop the current coroutine
            StartCoroutine(MoveToAnchoredPosition(startAnchoredPosition, moveDuration)); // Start moving up immediately
        }
    }
}