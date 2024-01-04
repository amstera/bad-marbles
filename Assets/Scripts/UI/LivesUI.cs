using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LivesUI : MonoBehaviour
{
    public RawImage[] lifeImages;
    private int lives;
    private bool isPopping = false;

    void Start()
    {
        lives = GameManager.Instance.Lives;
        HideExtraLives();
        UpdateLivesUI();
    }

    public void UpdateLives(int newLives)
    {
        int oldLives = lives;
        lives = newLives;
        UpdateLivesUI();

        // Determine which heart to animate
        if (newLives < oldLives) // Lost life
        {
            StartCoroutine(PopEffect(FindLastFadedHeartIndex(), false));
        }
        else if (newLives > oldLives) // Gained life
        {
            StartCoroutine(PopEffect(FindFirstNonFadedHeartIndex(), true));
        }
    }

    private int FindLastFadedHeartIndex()
    {
        for (int i = lifeImages.Length - 1; i >= 0; i--)
        {
            if (lifeImages[i].color.a < 1f)
                return i;
        }
        return -1;
    }

    private int FindFirstNonFadedHeartIndex()
    {
        for (int i = 0; i < lifeImages.Length; i++)
        {
            if (lifeImages[i].color.a == 1f)
                return i;
        }
        return -1;
    }

    private IEnumerator PopEffect(int lifeIndex, bool isGainingLife)
    {
        if (isPopping || lifeIndex < 0 || lifeIndex >= lifeImages.Length)
            yield break;

        isPopping = true;
        float duration = 0.2f;
        float elapsed = 0;
        RectTransform rectTransform = lifeImages[lifeIndex].GetComponent<RectTransform>();

        Vector3 originalScale = rectTransform.localScale;
        Vector3 targetScale = originalScale * 2f;

        Color startColor = lifeImages[lifeIndex].color;
        Color endColor = isGainingLife ? Color.white : new Color(1, 1, 1, 0.25f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            rectTransform.localScale = Vector3.Lerp(originalScale, targetScale, progress);
            lifeImages[lifeIndex].color = Color.Lerp(startColor, endColor, progress);
            yield return null;
        }

        rectTransform.localScale = originalScale;
        lifeImages[lifeIndex].color = endColor;
        isPopping = false;

        UpdateLivesUI();
    }

    private void UpdateLivesUI()
    {
        int startIndex = lifeImages.Length - lives;
        for (int i = 0; i < startIndex; i++)
        {
            lifeImages[i].color = lifeImages[i].color.a == 1 ? new Color(1, 1, 1, 0.99f) : new Color(1, 1, 1, 0.25f);
        }
        for (int i = startIndex; i < lifeImages.Length; i++)
        {
            lifeImages[i].color = Color.white;
        }
    }

    private void HideExtraLives()
    {
        int startIndex = lifeImages.Length - lives;
        for (int i = 0; i < startIndex; i++)
        {
            lifeImages[i].gameObject.SetActive(false);
        }
    }
}