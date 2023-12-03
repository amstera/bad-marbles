using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public ScoreText scoreText;
    public LivesUI livesUI;
    public StreakText streakText;
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

    private void Awake()
    {
        InitializeSingleton();
        StartCoroutine(UpdateTierRoutine());
    }

    private void InitializeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
        stressReceiver?.InduceStress(0.45f);
        ResetStreak();

        lifeLossSound?.Play();
    }

    public int CalculateMultiplier()
    {
        return Streak / 10 + 1;
    }

    private void ResetStreak()
    {
        Streak = 0;
    }

    private void CheckGameOver()
    {
        if (Lives <= 0)
        {
            marbleSpawner.DestroyAll();
            // TODO: Handle Game Over Logic
        }
    }
}
