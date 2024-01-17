using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class SupportUI : MonoBehaviour, IPointerDownHandler
{
    public CanvasGroup canvasGroup;
    public GameObject popUp;

    private float fadeDuration = 0.25f;
    private float popUpDuration = 0.25f;

    private void Start()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        popUp.transform.localScale = Vector3.zero;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject == gameObject)
        {
            Hide();
        }
    }

    public void ShowPanel()
    {
        gameObject.SetActive(true);

        StartCoroutine(Fade(true));
        StartCoroutine(PopIn(popUp, popUpDuration));
    }

    public void ShowContact()
    {
        OpenUrl("https://www.greenteagaming.com#contact");
    }

    public void ShowTerms()
    {
        OpenUrl("https://www.greenteagaming.com/terms-of-service");
    }

    public void ShowPrivacy()
    {
        OpenUrl("https://www.greenteagaming.com/privacy-policy");
    }

    private void Hide()
    {
        StartCoroutine(Fade(false));
        popUp.transform.localScale = Vector3.zero;
    }

    private IEnumerator Fade(bool fadeIn)
    {
        float targetAlpha = fadeIn ? 1f : 0f;
        float startAlpha = canvasGroup.alpha;
        float startTime = Time.time;
        while (Time.time < startTime + fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, (Time.time - startTime) / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = targetAlpha;
        canvasGroup.blocksRaycasts = fadeIn;

        if (!fadeIn)
        {
            gameObject.SetActive(false);
        }
    }

    private IEnumerator PopIn(GameObject obj, float time)
    {
        Vector3 targetScale = Vector3.one;
        for (float t = 0; t < 1; t += Time.deltaTime / time)
        {
            obj.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);
            yield return null;
        }
        obj.transform.localScale = targetScale;
    }

    private void OpenUrl(string url)
    {
        Application.OpenURL(url);
        Hide();
    }
}