using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public ScoreText scoreText;
    public LivesUI livesUI;
    public GameOverUI gameOverUI;
    public StreakText streakText;
    public StressReceiver stressReceiver;
    public MarbleSpawner marbleSpawner;

    public AudioSource backgroundMusic;
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

    private float marbleHitRadius = 1.85f;
    private SaveObject savedData;

    private void Awake()
    {
        savedData = SaveManager.Load();
        InitializeSingleton();
        StartCoroutine(UpdateTierRoutine());

        backgroundMusic.volume *= savedData.Settings.Volume;
    }

    void Update()
    {
        if (Lives <= 0)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Collider[] hitColliders = Physics.OverlapSphere(hit.point, marbleHitRadius);
                foreach (var hitCollider in hitColliders)
                {
                    Marble marble = hitCollider.GetComponent<Marble>();
                    if (marble != null && marble.color != MarbleColor.Tier)
                    {
                        marble.Destroy();
                        break;
                    }
                }
            }
        }
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

    private IEnumerator UpdateTierRoutine()
    {
        while (Lives > 0)
        {
            Tier++;
            if (Tier > savedData.HighTier)
            {
                savedData.HighTier = Tier;
                SaveManager.Save(savedData);
            }

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
        ResetStreak();
        Lives -= livesLost;
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
        if (Streak > savedData.HighStreak)
        {
            savedData.HighStreak = Streak;
            SaveManager.Save(savedData);
        }

        Streak = 0;
    }

    private void CheckGameOver()
    {
        if (Lives <= 0)
        {
            marbleSpawner.DestroyAll();
            Destroy(FindAnyObjectByType<Tier>()?.gameObject);

            gameOverUI.ShowGameOver(score, savedData);
        }
    }
}
