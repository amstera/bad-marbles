using EasyTransition;
using TMPro;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public TextMeshProUGUI highScoreText;
    public TransitionSettings transition;

    public GameObject newIndicator;

    public AudioSource plopSound;

    void Start()
    {
        UpdateHighScoreText();
        ShowNewIndicator();
    }

    public void LoadGame()
    {
        TransitionManager.Instance().Transition("Game", transition, 0);

        plopSound?.Play();
        MenuMusicPlayer.Instance.Destroy();
    }

    public void LoadPerks()
    {
        TransitionManager.Instance().Transition("Perks", transition, 0);

        plopSound?.Play();
    }

    private void UpdateHighScoreText()
    {
        SaveObject savedData = SaveManager.Load();
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
