using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.iOS;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public ScoreText scoreText;
    public LivesUI livesUI;
    public GameOverUI gameOverUI;
    public StreakText streakText;
    public StreakSaveText streakSaveText;
    public StressReceiver stressReceiver;
    public MarbleSpawner marbleSpawner;
    public StartText startText, extraChanceText;
    public VignetteAnimator vignetteAnimator;
    public TutorialPanel tutorialPanel;
    public TutorialUI tutorialUI;
    public ComplimentText complimentText;

    public GameObject capsule, bombButton, slowMoButton;

    public AudioSource pointGainedSound;
    public AudioSource lifeLossSound;
    public AudioSource lifeGainedSound;

    private int score;
    public int Score
    {
        get => score;
        private set
        {
            score = value;
            scoreText?.UpdateScore(score);
        }
    }

    private int lives = 3;
    public int Lives
    {
        get => lives;
        private set
        {
            lives = Mathf.Max(value, 0);
            livesUI?.UpdateLives(lives);
            CheckGameOver();
        }
    }

    private int streak;
    public int Streak
    {
        get => streak;
        private set
        {
            streak = value;
            streakText?.UpdateStreak(CalculateStreakMultiplier());
        }
    }

    private int tier;
    public int Tier
    {
        get => tier;
        private set
        {
            tier = value;
            marbleSpawner?.UpdateTier(tier);
        }
    }

    public int StartingLives = 3;

    private float marbleHitRadius = 2.6f;

    private int streakSavers = 0;
    private SaveObject savedData;
    private ExtraChance extraChance;
    private int marblesHit = 0;
    private bool canHitMarbles = true;
    private bool isInvincible;

    private int initialStreakInterval = 5;
    private int subsequentStreakInterval = 10;
    private int initialStreakMaxMultiplier = 5;

    private HashSet<Marble> touchedMarbles = new HashSet<Marble>();
    const int MarbleLayerMask = 1 << 3;
    private Vector3 downBackVector = new Vector3(0, -1, -1);

    void Awake()
    {
        SetQualitySettings();
        savedData = SaveManager.Load();
        InitializeSingleton();
        SetPerks();
        extraChance = savedData.ExtraChance;
        StartingLives = lives;

        if (extraChance.ActiveCount > 0)
        {
            UpdateExtraChanceValues();
        }

        if (DeviceTypeChecker.IsTablet())
        {
            AdjustTabletUI();
        }
    }

    void Start()
    {
        InitializeAdEvents();

        if (extraChance.ActiveCount == 0 && ShouldShowAd())
        {
            GameMusicPlayer.Instance.Pause();
            AdsManager.Instance.interstitialAd.ShowAd();
        }
        else
        {
            StartGame();
            if (!savedData.HasSeenTutorial)
            {
                StartCoroutine(ActivateTutorial());
            }
        }
    }

    void Update()
    {
        if (Lives <= 0 || !canHitMarbles)
        {
            return;
        }

        ProcessTouches();

        touchedMarbles.Clear();
    }

    void ProcessTouches()
    {
        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    ProcessTouchInput(touch.position);
                }
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            ProcessTouchInput(Input.mousePosition);
        }
    }

    void ProcessTouchInput(Vector2 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            TryProcessHitObject(hit.collider, hit.point);
        }
    }

    void TryProcessHitObject(Collider collider, Vector3 hitPoint)
    {
        bool marbleDestroyed = TryDestroyMarble(collider);
        if (!marbleDestroyed)
        {
            marbleDestroyed = CheckAndDestroyNearbyMarble(hitPoint);
            if (!marbleDestroyed)
            {
                Ramp ramp = collider.GetComponent<Ramp>();
                if (ramp != null)
                {
                    ramp.Hit(hitPoint);
                }
            }
        }
    }

    bool TryDestroyMarble(Collider collider)
    {
        Marble marble = collider.GetComponent<Marble>();
        if (marble != null && !touchedMarbles.Contains(marble) && marble.color != MarbleColor.Tier)
        {
            touchedMarbles.Add(marble);
            if (marble.livesLost > 0)
            {
                marblesHit++;
                if (marblesHit > 0 && marblesHit % 10 == 0)
                {
                    complimentText.ShowCompliment();
                }
            }
            else if (marble.color == MarbleColor.Life)
            {
                if (Lives > 0)
                {
                    Lives++;
                    lifeGainedSound?.Play();

                    marble.GetComponentInParent<ExtraLife>().Destroy();
                }
            }

            marble.Destroy();

            return true;
        }

        return false;
    }

    bool CheckAndDestroyNearbyMarble(Vector3 point)
    {
        var startPoint = point + downBackVector;
        var endPoint = point - downBackVector;

        Collider[] hitColliders = Physics.OverlapCapsule(startPoint, endPoint, marbleHitRadius, MarbleLayerMask);
        foreach (var hitCollider in hitColliders)
        {
            if (TryDestroyMarble(hitCollider))
            {
                return true; // Marble found and destroyed
            }
        }

        return false; // No marble found
    }

    private void StartGame()
    {
        DeinitializeAdEvents();

        GameMusicPlayer.Instance.Play();
        startText.gameObject.SetActive(true);
        if (extraChance.ActiveCount > 0)
        {
            StartCoroutine(ShowExtraChanceText());
        }
        if (savedData.HasSeenTutorial)
        {
            StartCoroutine(UpdateTierRoutine());
        }
    }

    private bool ShouldShowAd()
    {
        // Early return if ads can't be shown, no games have been played, or an ad was shown last game
        if (!savedData.CanShowAds || savedData.GamesPlayed <= 0 || savedData.LastGameShowedAd)
        {
            if (savedData.LastGameShowedAd)
            {
                savedData.LastGameShowedAd = false;
                SaveManager.Save(savedData);
            }
            return false;
        }

        int adFrequency = (savedData.Points >= 50000 && savedData.GamesPlayed >= 50) ? 2 : 3;
        bool isAdDue = savedData.GamesPlayed % adFrequency == 0;

        if (isAdDue)
        {
            savedData.LastGameShowedAd = true;
            SaveManager.Save(savedData);
            return true;
        }

        return false;
    }

    private void InitializeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SetPerks()
    {
        foreach (var perk in savedData.SelectedPerks.SelectedSpecial)
        {
            if (perk == PerkEnum.ExtraLife1 || perk == PerkEnum.ExtraLife2 || perk == PerkEnum.ExtraLife3)
            {
                lives++;
            }
            else if (perk == PerkEnum.StreakSaver || perk == PerkEnum.StreakSaver2)
            {
                streakSavers++;
            }
        }
    }

    private void UpdateExtraChanceValues()
    {
        score = extraChance.Score;
        scoreText.SetScoreImmediately(score);
        tier = extraChance.Tier;
        var newSpeed = extraChance.ActiveCount == 1 ? extraChance.MarbleSpawnSpeed * 0.7f : extraChance.MarbleSpawnSpeed * 0.9f;
        marbleSpawner.speed = Mathf.Max(marbleSpawner.speed, newSpeed);
        vignetteAnimator.SetElapsedTime(extraChance.ElapsedTime);
    }

    private IEnumerator ShowExtraChanceText()
    {
        yield return new WaitForSeconds(3);
        extraChanceText.gameObject.SetActive(true);
    }

    private void InitializeAdEvents()
    {
        AdsManager.Instance.interstitialAd.OnAdCompleted += StartGame;
        AdsManager.Instance.interstitialAd.OnAdSkipped += StartGame;
        AdsManager.Instance.interstitialAd.OnAdFailed += StartGame;
    }

    private void DeinitializeAdEvents()
    {
        AdsManager.Instance.interstitialAd.OnAdCompleted -= StartGame;
        AdsManager.Instance.interstitialAd.OnAdSkipped -= StartGame;
        AdsManager.Instance.interstitialAd.OnAdFailed -= StartGame;
    }

    private IEnumerator UpdateTierRoutine()
    {
        while (Lives > 0)
        {
            Tier++;
            int secondsToWait = tier <= 5 ? 15 : 10;
            yield return new WaitForSeconds(secondsToWait);
        }
    }

    public void AddScore(int points)
    {
        Score += points;
        Streak++;

        pointGainedSound?.Play();
    }

    public void LoseLives(int livesLost)
    {
        if (isInvincible)
        {
            return;
        }

        Lives -= livesLost;
        var curStreak = CalculateStreakMultiplier();
        if (curStreak > 1)
        {
            ResetStreak();
        }
        else
        {
            Streak = 0;
        }
        stressReceiver?.InduceStress(0.4f);
        complimentText.Hide();

        lifeLossSound?.Play();
        if (savedData.Settings.VibrationEnabled)
        {
            Handheld.Vibrate();
        }
    }

    public int CalculateStreakMultiplier()
    {
        // Calculate the streak count at which the initial interval ends
        int initialIntervalMaxStreak = (initialStreakMaxMultiplier - 1) * initialStreakInterval;

        // If the streak is within the initial interval range
        if (streak <= initialIntervalMaxStreak)
        {
            return streak / initialStreakInterval + 1;
        }
        // Beyond the initial interval range
        else
        {
            int additionalStreaks = (streak - initialIntervalMaxStreak) / subsequentStreakInterval;
            return initialStreakMaxMultiplier + additionalStreaks;
        }
    }

    private void ResetStreak()
    {
        if (streakSavers > 0 && lives > 0)
        {
            streakSaveText.Show();
            streakSavers--;
            return;
        }

        int streakMultiplier = CalculateStreakMultiplier();
        if (streak > savedData.HighStreak || streakMultiplier > savedData.HighStreakMultiplier)
        {
            savedData.HighStreak = Mathf.Max(streak, savedData.HighStreak);
            savedData.HighStreakMultiplier = Mathf.Max(streakMultiplier, savedData.HighStreakMultiplier);
            SaveManager.Save(savedData);
        }

        streakSaveText.QuickHide();
        Streak = 0;
    }

    private void CheckGameOver()
    {
        if (Lives <= 0)
        {
            marbleSpawner.DestroyAll();
            Destroy(FindAnyObjectByType<Tier>()?.gameObject);
            vignetteAnimator.Pause(true);

            gameOverUI.ShowGameOver(score, tier, savedData, extraChance.ActiveCount, marbleSpawner.speed, vignetteAnimator.elapsedTime);
        }
    }

    private void OnDestroy()
    {
        if (AdsManager.Instance?.interstitialAd != null)
        {
            DeinitializeAdEvents();
        }
    }

    private IEnumerator ActivateTutorial()
    {
        Tier++;
        marbleSpawner.OnlyMarbleColor = MarbleColor.Red;
        marbleSpawner.canUpdateSpeed = false;
        vignetteAnimator.enabled = false;
        marbleSpawner.spawnArrows = true;

        tutorialUI.ClosePopup += FinishTutorial;

        yield return new WaitForSeconds(4f);
        Time.timeScale = 0.35f;

        isInvincible = true;

        tutorialPanel.Show("1.", "Smash <color=red>Red Marbles</color> before they pass the screen!");

        yield return new WaitUntil(() => marblesHit > 0); // wait for marble to be hit

        tutorialPanel.Hide();
        marbleSpawner.spawnArrows = false;
        foreach (var arrow in FindObjectsOfType<ArrowUI>())
        {
            Destroy(arrow.gameObject);
        }

        Time.timeScale = 1;
        yield return new WaitForSeconds(0.5f);

        tutorialPanel.Show("2.", "Miss a <color=red>Red Marble</color> and you'll lose a <color=red>heart</color>!");

        isInvincible = false;
        canHitMarbles = false;

        yield return new WaitUntil(() => Lives < 2); // wait until you lose life

        tutorialPanel.Hide();
        isInvincible = true;

        yield return new WaitForSeconds(0.5f);
        marbleSpawner.DestroyAll();
        marbleSpawner.OnlyMarbleColor = MarbleColor.Green;

        tutorialPanel.Show("3.", "Let <color=green>Green Marbles</color> pass to score <sprite=0>!");

        yield return new WaitUntil(() => Score > 0); // wait until you get a point

        yield return new WaitForSeconds(1.25f);
        tutorialPanel.Hide();
        marbleSpawner.DestroyAll();
        marbleSpawner.enabled = false;

        tutorialUI.Show();
    }

    private void FinishTutorial()
    {
        tutorialUI.ClosePopup -= FinishTutorial;
        savedData.HasSeenTutorial = true;
        SaveManager.Save(savedData);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void SetQualitySettings()
    {
        #if UNITY_IOS
                if (Device.lowPowerModeEnabled || SystemInfo.batteryLevel <= 0.2f)
                {
                    QualitySettings.SetQualityLevel(1, true);
                }
                else
                {
                    QualitySettings.SetQualityLevel(2, true);
                }
        #endif
    }

    private void AdjustTabletUI()
    {
        Vector3 moveAmount = Vector3.up * 35f;
        capsule.transform.localPosition += moveAmount;
        bombButton.transform.localPosition -= moveAmount * 4f;
        slowMoButton.transform.localPosition -= moveAmount * 4f;
        streakText.transform.localPosition += moveAmount;
        streakSaveText.transform.localPosition += moveAmount;
    }
}