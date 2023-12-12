using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using EasyTransition;

public class GameOverUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public Image newIcon;
    public GameObject newIndicator;
    public CanvasGroup canvasGroup;
    public TransitionSettings transition;

    public AudioSource highScoreSound;
    public AudioSource plopSound;

    private void Awake()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void ShowGameOver(int score, SaveObject savedData)
    {
        StartCoroutine(FadeInCanvasGroup());
        UpdateScoreText(score);
        UpdateHighScoreText(score, savedData);
        ShowNewIndicator();
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadPerks()
    {
        Destroy(GameMusicPlayer.Instance.gameObject, 1);
        TransitionManager.Instance().Transition("Perks", transition, 0);

        plopSound?.Play();
    }

    private IEnumerator FadeInCanvasGroup()
    {
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

    private void UpdateHighScoreText(int score, SaveObject savedData)
    {
        if (score > savedData.HighScore)
        {
            newIcon.enabled = true;
            savedData.HighScore = score;

            highScoreSound?.Play();
        }

        savedData.Points += score;
        savedData.GamesPlayed++;
        SaveManager.Save(savedData);

        highScoreText.text = $"{savedData.HighScore}";
    }

    private void ShowNewIndicator()
    {
        var categories = PerkService.Instance.GetUnlockedPerkCategories();
        if (categories.Count > 0)
        {
            newIndicator.SetActive(true);
        }
    }
}