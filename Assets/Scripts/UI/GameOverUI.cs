using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using EasyTransition;
using UnityEngine.iOS;
using OneManEscapePlan.ModalDialogs.Scripts;
using UnityEngine.Analytics;
using System.Collections.Generic;

public class GameOverUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText, highScoreText, extraChanceReminderText;
    public Image newIcon;
    public GameObject newIndicator, popUp;
    public CanvasGroup canvasGroup;
    public TransitionSettings transition;
    public ParticleSystem confettiPS;
    public Button playAgainButton;
    public Button noAdsButton;
    public Button secondChanceButton;
    public Image shareIcon;
    public GameObject arrow;
    public Toast toast;

    public AudioSource highScoreSound;
    public AudioSource plopSound;

    private int score, tier;
    private float marbleSpawnSpeed, elapsedTime;
    private SaveObject savedData;
    private float swayAngle = 5f;
    private float swaySpeed = 3f;

    private void Awake()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void ShowGameOver(int score, int tier, SaveObject savedData, int activeCount, float marbleSpawnSpeed, float elapsedTime)
    {
        Time.timeScale = 1;
        popUp.transform.localScale = Vector3.zero;
        gameObject.SetActive(true);

        this.savedData = savedData;

        StartCoroutine(FadeInCanvasGroup());
        UpdateScoreText(score);
        ShowExtraChanceButton(activeCount);
        UpdateHighScoreText(score, tier, savedData);
        ShowNewIndicator();
        InitializeAdEvents();

        this.score = score;
        this.tier = tier - 1;
        this.marbleSpawnSpeed = marbleSpawnSpeed;
        this.elapsedTime = elapsedTime;

        if (savedData.CanShowAds)
        {
            if (!secondChanceButton.gameObject.activeSelf)
            {
                Vector3 noAdsButtonPosition = noAdsButton.transform.localPosition;
                noAdsButton.transform.localPosition = new Vector3(noAdsButtonPosition.x, noAdsButtonPosition.y + 50, noAdsButtonPosition.z);
            }
        }
        else
        {
            noAdsButton.gameObject.SetActive(false);
        }
    }

    public void LoadGame()
    {
        DeinitializeAdEvents();
        if (savedData.ExtraChance.ActiveCount > 0)
        {
            savedData.ExtraChance.ActiveCount = 0;
            SaveManager.Save(savedData);
        }

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

    public void Share()
    {
        StartCoroutine(ShareButtonPressed());
    }

    private IEnumerator ShareButtonPressed()
    {
        shareIcon.color = new Color(0.8f, 0.8f, 0.8f, 1);

        yield return new WaitForSeconds(0.1f);

        shareIcon.color = Color.white;
    }

    private void RewardedAdSkipped()
    {
        GameMusicPlayer.Instance.Play();
    }

    private void RewardedAdCompleted()
    {
        GameMusicPlayer.Instance.Play();

        canvasGroup.alpha = 0;
        savedData.ExtraChance.ActiveCount++;
        savedData.ExtraChance.Score = score;
        savedData.ExtraChance.Tier = tier;
        savedData.ExtraChance.ElapsedTime = elapsedTime;
        savedData.ExtraChance.MarbleSpawnSpeed = marbleSpawnSpeed;
        SaveManager.Save(savedData);

        DeinitializeAdEvents();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

        StartCoroutine(PopIn(popUp, 0.2f));

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
        else
        {
            float highScoreThreshold = savedData.HighScore * 0.85f;
            if (secondChanceButton.gameObject.activeSelf && score >= highScoreThreshold && score < savedData.HighScore)
            {
                int pointsAway = savedData.HighScore - score;
                extraChanceReminderText.gameObject.SetActive(true);
                extraChanceReminderText.text = $"{pointsAway} point{(pointsAway != 1 ? "s" : "")} away from high score!";

                Vector3 popUpPosition = popUp.transform.localPosition;
                popUp.transform.localPosition = new Vector3(popUpPosition.x, 50, popUpPosition.z);

                Vector3 noAdsButtonPosition = noAdsButton.transform.localPosition;
                noAdsButton.transform.localPosition = new Vector3(noAdsButtonPosition.x, noAdsButtonPosition.y - 50, noAdsButtonPosition.z);
            }
        }

        savedData.Points += savedData.ExtraChance.ActiveCount > 0 ? score - savedData.ExtraChance.Score : score;

        if (savedData.ExtraChance.ActiveCount == 0)
        {
            savedData.GamesPlayed++;

            AnalyticsResult analyticsResult = Analytics.CustomEvent("gamesPlayed",
                new Dictionary<string, object>
                {
                    { "gamesPlayedCount", savedData.GamesPlayed },
                    { "score", score },
                }
            );
            Debug.Log("Analytics Event Sent: " + analyticsResult.ToString());
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
        var perks = PerkService.Instance.GetUnlockedPerks().perks;
        if (perks.Count > 0)
        {
            newIndicator.SetActive(true);
            StartCoroutine(SwayNewIndicator());
            toast.gameObject.SetActive(true);
            toast.Show($"You unlocked {perks.Count} new reward{(perks.Count == 1 ? "" : "s")}!");

            if (!savedData.HasSeenPerksTutorial)
            {
                arrow.SetActive(true);
                StartCoroutine(SwayArrowCoroutine());

                playAgainButton.interactable = false;
                playAgainButton.GetComponentInChildren<ShineEffect>().enabled = false;
            }
        }
    }

    private void ShowExtraChanceButton(int activeCount)
    {
        if (activeCount >= 2 || (activeCount == 1 && !savedData.SelectedPerks.SelectedSpecial.Contains(PerkEnum.ExtraChance)))
        {
            secondChanceButton.gameObject.SetActive(false);
            savedData.ExtraChance.ActiveCount = 0;

            SaveManager.Save(savedData);
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

    private IEnumerator PopIn(GameObject obj, float time)
    {
        Vector3 originalScale = obj.transform.localScale;
        Vector3 targetScale = Vector3.one;
        for (float t = 0; t < 1; t += Time.deltaTime / time)
        {
            obj.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }
        obj.transform.localScale = targetScale;
    }

    private void OnDestroy()
    {
        DeinitializeAdEvents();
    }
}