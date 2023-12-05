using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public Image newIcon;
    public CanvasGroup canvasGroup;

    public AudioSource highScoreSound;

    private void Awake()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void ShowGameOver(int score)
    {
        StartCoroutine(FadeInCanvasGroup());
        UpdateScoreText(score);
        UpdateHighScoreText(score);
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

    private void UpdateHighScoreText(int score)
    {
        SaveObject savedData = SaveManager.Load();
        if (score > savedData.HighScore)
        {
            newIcon.enabled = true;
            savedData.HighScore = score;

            highScoreSound?.Play();
        }

        savedData.Points += score;
        SaveManager.Save(savedData);

        highScoreText.text = $"{savedData.HighScore}";
    }
}