using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LivesUI : MonoBehaviour
{
    public RawImage[] lifeImages;
    private int lives;
    private bool[] isPopping;
    private int startIndex;

    void Start()
    {
        lives = GameManager.Instance.Lives;
        startIndex = lifeImages.Length - lives;
        HideExtraLives();
        isPopping = new bool[lifeImages.Length];
    }

    public void UpdateLives(int newLives)
    {
        int oldLives = lives;
        lives = newLives;

        if (newLives < oldLives) // Lost life
        {
            for (int i = 0; i < oldLives - newLives; i++)
            {
                int index = startIndex + i;
                if (index >= 0 && index < lifeImages.Length)
                {
                    StartCoroutine(PopEffect(index, false));
                }
            }
        }
        else if (newLives > oldLives) // Gained life
        {
            for (int i = 0; i < newLives - oldLives; i++)
            {
                int index = startIndex - i - 1;
                if (index >= 0 && index < lifeImages.Length)
                {
                    StartCoroutine(PopEffect(index, true));
                }
            }
        }

        startIndex = lifeImages.Length - lives;
    }

    private IEnumerator PopEffect(int lifeIndex, bool isGainingLife)
    {
        if (lifeIndex < 0 || isPopping[lifeIndex] || lifeIndex >= lifeImages.Length)
            yield break;

        isPopping[lifeIndex] = true;
        float duration = 0.25f;
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
        isPopping[lifeIndex] = false;
    }

    private void HideExtraLives()
    {
        for (int i = 0; i < startIndex; i++)
        {
            lifeImages[i].gameObject.SetActive(false);
        }
    }
}