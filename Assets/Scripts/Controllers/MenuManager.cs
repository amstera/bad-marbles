using EasyTransition;
using TMPro;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public TextMeshProUGUI highScoreText;
    public TransitionSettings transition;

    public AudioSource plopSound;

    void Start()
    {
        UpdateHighScoreText();
    }

    public void LoadGame()
    {
        TransitionManager.Instance().Transition("Game", transition, 0);

        plopSound?.Play();
    }

    private void UpdateHighScoreText()
    {
        SaveObject savedData = SaveManager.Load();
        highScoreText.text = $"{savedData.HighScore}";
    }
}
