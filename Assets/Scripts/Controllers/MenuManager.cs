using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public TextMeshProUGUI highScoreText;

    void Start()
    {
        UpdateHighScoreText();
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("Game");
    }

    private void UpdateHighScoreText()
    {
        SaveObject savedData = SaveManager.Load();
        highScoreText.text = $"{savedData.HighScore}";
    }
}
