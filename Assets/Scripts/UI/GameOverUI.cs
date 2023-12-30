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
    public Button playAgainButton;
    public Button noAdsButton;
    public Button secondChanceButton;
    public GameObject arrow;

    public AudioSource highScoreSound;
    public AudioSource plopSound;

    private int score, tier;
    private float marbleSpawnSpeed;
    private SaveObject savedData;
    private float swayAngle = 5f;
    private float swaySpeed = 4f;

    private void Awake()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void ShowGameOver(int score, int tier, SaveObject savedData, bool isExtraChance, float marbleSpawnSpeed)
    {
        Time.timeScale = 1;
        gameObject.SetActive(true);

        this.savedData = savedData;

        StartCoroutine(FadeInCanvasGroup());
        UpdateScoreText(score);
        UpdateHighScoreText(score, tier, savedData);
        ShowNewIndicator();
        InitializeAdEvents();

        this.score = score;
        this.tier = tier - 1;
        this.marbleSpawnSpeed = marbleSpawnSpeed;

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

        canvasGroup.alpha = 0;
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

        DialogManager.Instance.ShowDialog("Alert", "Couldn't load rewards ad at this time!");
        secondChanceButton.interactable = false;
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
            StartCoroutine(SwayNewIndicator());

            if (!savedData.HasSeenPerksTutorial)
            {
                arrow.SetActive(true);
                StartCoroutine(SwayArrowCoroutine());

                playAgainButton.interactable = false;
                playAgainButton.GetComponentInChildren<ShineEffect>().enabled = false;
            }
        }
    }

    private IEnumerator SwayNewIndicator()
    {
        while (true)
        {
            float angle = swayAngle * Mathf.Sin(Time.time * swaySpeed);
            newIndicator.transform.rotation = Quaternion.Euler(0, 0, angle);
            yield return null;
        }
    }

    private IEnumerator SwayArrowCoroutine()
    {
        while (true)
        {
            float swayPosition = Mathf.Lerp(-96f, -101f, (Mathf.Sin(Time.time * swaySpeed * 2) + 1) / 2);
            arrow.transform.localPosition = new Vector3(swayPosition, arrow.transform.localPosition.y, arrow.transform.localPosition.z);
            yield return null;
        }
    }

    private void OnDestroy()
    {
        DeinitializeAdEvents();
    }
}