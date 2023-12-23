using EasyTransition;
using OneManEscapePlan.ModalDialogs.Scripts;
using TMPro;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public TextMeshProUGUI highScoreText;
    public TransitionSettings transition;
    public GameObject noAdsButton;
    public GameObject newIndicator;

    private SaveObject savedData;

    public AudioSource plopSound;

    void Start()
    {
        savedData = SaveManager.Load();
        if (!savedData.CanShowAds)
        {
            noAdsButton.SetActive(false);
        }
        if (savedData.ExtraChance.IsActive)
        {
            savedData.ExtraChance.IsActive = false;
            SaveManager.Save(savedData);
        }

        UpdateHighScoreText();
        ShowNewIndicator();
    }

    public void LoadGame()
    {
        TransitionManager.Instance().Transition("Game", transition, 0);

        plopSound?.Play();
    }

    public void LoadPerks()
    {
        TransitionManager.Instance().Transition("Perks", transition, 0);

        plopSound?.Play();
    }

    private void UpdateHighScoreText()
    {
        highScoreText.text = $"{savedData.HighScore}";
    }

    private void ShowNewIndicator()
    {
        var categories = PerkService.Instance.GetUnlockedPerks().categories;
        if (categories.Count > 0)
        {
            newIndicator.SetActive(true);
        }
    }
}
