using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using EasyTransition;
using UnityEngine.iOS;

public class GameOverUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public Image newIcon;
    public GameObject newIndicator;
    public CanvasGroup canvasGroup;
    public TransitionSettings transition;
    public ParticleSystem confettiPS;
    public GameObject noAdsButton;

    public AudioSource highScoreSound;
    public AudioSource plopSound;

    private void Awake()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void ShowGameOver(int score, int tier, SaveObject savedData)
    {
        StartCoroutine(FadeInCanvasGroup());
        UpdateScoreText(score);
        UpdateHighScoreText(score, tier, savedData);
        ShowNewIndicator();

        if (!savedData.CanShowAds)
        {
            noAdsButton.SetActive(false);
        }
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadPerks()
    {
        TransitionManager.Instance().Transition("Perks", transition, 0);

        plopSound?.Play();
    }

    public void WatchChanceAd()
    {
        //todo: show rewards ad
    }

    private IEnumerator FadeInCanvasGroup()
    {
        yield return new WaitForSeconds(0.1f);

        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime / 0.35f;
            yield return null;
        }
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private void UpdateScoreText(int score)
    {
        scoreText.text = $"{score}";
    }

    private void UpdateHighScoreText(int score, int tier, SaveObject savedData)
    {
        if (score > savedData.HighScore)
        {
            newIcon.enabled = true;
            savedData.HighScore = score;

            highScoreSound?.Play();
            confettiPS?.Play();

            if (savedData.GamesPlayed > 5 && !savedData.HasShownRateApp)
            {
                savedData.HasShownRateApp = Device.RequestStoreReview();
            }
        }

        savedData.Points += score;
        savedData.GamesPlayed++;
        if (tier > savedData.HighTier)
        {
            savedData.HighTier = tier;
        }
        SaveManager.Save(savedData);

        highScoreText.text = $"{savedData.HighScore}";
    }

    private void ShowNewIndicator()
    {
        var categories = PerkService.Instance.GetUnlockedPerks().categories;
        if (categories.Count > 0)
        {
            newIndicator.SetActive(true);
        }
    }
}