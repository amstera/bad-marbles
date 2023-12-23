using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using EasyTransition;
using UnityEngine.iOS;
using OneManEscapePlan.ModalDialogs.Scripts;

public class GameOverUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public Image newIcon;
    public GameObject newIndicator;
    public CanvasGroup canvasGroup;
    public TransitionSettings transition;
    public ParticleSystem confettiPS;
    public Button noAdsButton;
    public Button secondChanceButton;

    public AudioSource highScoreSound;
    public AudioSource plopSound;

    private int score, tier;
    private float marbleSpawnSpeed;
    private SaveObject savedData;

    private void Awake()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void ShowGameOver(int score, int tier, SaveObject savedData, bool isExtraChance, float marbleSpawnSpeed)
    {
        StartCoroutine(FadeInCanvasGroup());
        UpdateScoreText(score);
        UpdateHighScoreText(score, tier, savedData);
        ShowNewIndicator();
        InitializeAdEvents();

        this.score = score;
        this.tier = tier - 1;
        this.marbleSpawnSpeed = marbleSpawnSpeed;
        this.savedData = savedData;

        if (!savedData.CanShowAds)
        {
            noAdsButton.gameObject.SetActive(false);
        }
        if (isExtraChance)
        {
            secondChanceButton.gameObject.SetActive(false);
            savedData.ExtraChance.IsActive = false;

            SaveManager.Save(savedData);
        }
    }

    public void LoadGame()
    {
        DeinitializeAdEvents();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadPerks()
    {
        TransitionManager.Instance().Transition("Perks", transition, 0);

        plopSound?.Play();
    }

    public void WatchChanceAd()
    {
        GameMusicPlayer.Instance.Pause();
        AdsManager.Instance.rewardedAd.ShowAd();
    }

    private void RewardedAdSkipped()
    {
        GameMusicPlayer.Instance.Play();
    }

    private void RewardedAdCompleted()
    {
        GameMusicPlayer.Instance.Play();

        secondChanceButton.enabled = false;
        savedData.ExtraChance.IsActive = true;
        savedData.ExtraChance.Score = score;
        savedData.ExtraChance.Tier = tier;
        savedData.ExtraChance.MarbleSpawnSpeed = marbleSpawnSpeed;
        SaveManager.Save(savedData);

        LoadGame();
    }

    private void RewardedAdFailedToLoad()
    {
        GameMusicPlayer.Instance.Play();

        DialogManager.Instance.ShowDialog("Alert", "Couldn't load rewards ad!");
        secondChanceButton.enabled = false;
    }

    private void InitializeAdEvents()
    {
        AdsManager.Instance.rewardedAd.OnAdCompleted += RewardedAdCompleted;
        AdsManager.Instance.rewardedAd.OnAdSkipped += RewardedAdSkipped;
        AdsManager.Instance.rewardedAd.OnAdFailed += RewardedAdFailedToLoad;
    }

    private void DeinitializeAdEvents()
    {
        if (AdsManager.Instance?.rewardedAd != null)
        {
            AdsManager.Instance.rewardedAd.OnAdCompleted -= RewardedAdCompleted;
            AdsManager.Instance.rewardedAd.OnAdSkipped -= RewardedAdSkipped;
            AdsManager.Instance.rewardedAd.OnAdFailed -= RewardedAdFailedToLoad;
        }
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

        savedData.Points += savedData.ExtraChance.IsActive ? score - savedData.ExtraChance.Score : score;

        if (!savedData.ExtraChance.IsActive)
        {
            savedData.GamesPlayed++;
        }
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

    private void OnDestroy()
    {
        DeinitializeAdEvents();
    }
}