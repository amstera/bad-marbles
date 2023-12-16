using UnityEngine;
using System.Collections;

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

    public AudioSource pointGainedSound;
    public AudioSource lifeLossSound;

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
            streakText?.UpdateStreak(streak);
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

    private float marbleHitRadius = 2f;
    private int streakSavers = 0;
    private SaveObject savedData;

    private void Awake()
    {
        savedData = SaveManager.Load();
        InitializeSingleton();
        StartCoroutine(UpdateTierRoutine());
        SetPerks();
    }

    void Update()
    {
        if (Lives <= 0)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            ProcessMarbleHit();
        }
    }

    void ProcessMarbleHit()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            bool marbleDestroyed = TryDestroyMarble(hit.collider);
            if (!marbleDestroyed)
            {
                // If no marble was directly hit, check for nearby marbles
                marbleDestroyed = CheckAndDestroyNearbyMarble(hit.point);
                if (!marbleDestroyed)
                {
                    Ramp ramp = hit.collider.GetComponent<Ramp>();
                    if (ramp != null)
                    {
                        ramp.Hit(hit.point);
                    }
                }
            }
        }
    }

    bool TryDestroyMarble(Collider collider)
    {
        Marble marble = collider.GetComponent<Marble>();
        if (marble != null && marble.color != MarbleColor.Tier)
        {
            marble.Destroy();
            return true;
        }

        return false;
    }

    bool CheckAndDestroyNearbyMarble(Vector3 point)
    {
        int layerMask = 1 << 3; // Layer mask for marbles
        Collider[] hitColliders = Physics.OverlapSphere(point, marbleHitRadius, layerMask);
        foreach (var hitCollider in hitColliders)
        {
            if (TryDestroyMarble(hitCollider))
            {
                return true; // Marble found and destroyed
            }
        }

        return false; // No marble found
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
            if (perk == PerkEnum.ExtraLife1 || perk == PerkEnum.ExtraLife2)
            {
                lives++;
            }
            if (perk == PerkEnum.StreakSaver || perk == PerkEnum.StreakSaver2)
            {
                streakSavers++;
            }
        }
    }

    private IEnumerator UpdateTierRoutine()
    {
        while (Lives > 0)
        {
            Tier++;
            yield return new WaitForSeconds(15);
        }
    }

    public void AddScore(int points)
    {
        points *= CalculateMultiplier();
        Score += points;
        Streak++;

        pointGainedSound?.Play();
    }

    public void LoseLives(int livesLost)
    {
        Lives -= livesLost;
        var curStreak = CalculateMultiplier();
        if (curStreak > 1)
        {
            ResetStreak();
        }
        stressReceiver?.InduceStress(0.5f);

        lifeLossSound?.Play();
        Handheld.Vibrate();
    }

    public int CalculateMultiplier()
    {
        return Streak / 10 + 1;
    }

    private void ResetStreak()
    {
        if (streakSavers > 0 && lives > 0)
        {
            streakSaveText.Show();
            streakSavers--;
            return;
        }

        if (Streak > savedData.HighStreak)
        {
            savedData.HighStreak = Streak;
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

            gameOverUI.ShowGameOver(score, tier, savedData);
        }
    }
}
