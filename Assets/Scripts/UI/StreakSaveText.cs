using UnityEngine;
using System.Collections;
using TMPro;

public class StreakSaveText : MonoBehaviour
{
    public GameObject streakSave;
    public TextMeshProUGUI streakSaveText;
    public CanvasGroup canvasGroup;
    public AudioSource streakSound;

    private bool isShowing = false;
    private Coroutine showRoutine = null;

    public void Show()
    {
        if (!isShowing)
        {
            isShowing = true;
            streakSave.SetActive(true);
            streakSaveText.gameObject.SetActive(true);
            canvasGroup.alpha = 1;
            streakSound.Play();

            if (showRoutine != null)
            {
                StopCoroutine(showRoutine);
            }
            showRoutine = StartCoroutine(ShowAndHideRoutine());
        }
    }

    private IEnumerator ShowAndHideRoutine()
    {
        yield return StartCoroutine(PopAnimation());

        yield return new WaitForSeconds(2.5f);

        yield return StartCoroutine(FadeOutAndDeactivate());
    }

    private IEnumerator PopAnimation()
    {
        // Enhanced scale animation
        float animationTime = 0.3f; // Shorter time for snappier animation
        Vector3 originalScale = streakSave.transform.localScale;
        Vector3 targetScale = originalScale * 1.2f; // Scale up a bit more for emphasis

        // Scale up
        float timer = 0;
        while (timer < animationTime)
        {
            streakSave.transform.localScale = Vector3.Lerp(originalScale, targetScale, timer / animationTime);
            timer += Time.deltaTime;
            yield return null;
        }

        // Scale down
        timer = 0;
        while (timer < animationTime)
        {
            streakSave.transform.localScale = Vector3.Lerp(targetScale, originalScale, timer / animationTime);
            timer += Time.deltaTime;
            yield return null;
        }
    }


    private IEnumerator FadeOutAndDeactivate()
    {
        float fadeOutTime = 1.0f;

        float timer = 0;
        while (timer < fadeOutTime)
        {
            canvasGroup.alpha = 1 - (timer / fadeOutTime);
            timer += Time.deltaTime;
            yield return null;
        }

        streakSave.SetActive(false);
        isShowing = false;
    }

    public void QuickHide()
    {
        if (isShowing)
        {
            StopCoroutine(showRoutine);
            streakSave.SetActive(false);
            isShowing = false;
        }
    }
}