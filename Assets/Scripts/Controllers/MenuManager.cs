using System.Collections;
using EasyTransition;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public TextMeshProUGUI highScoreTitleText, highScoreText;
    public TransitionSettings transition;
    public Button playButton, perksButton, noAdsButton;
    public GameObject newIndicator;
    public GameObject logo, goldTrophy;

    public AudioSource plopSound;

    private SaveObject savedData;
    private float swayAngle = 5f;
    private float swaySpeed = 3f;

    void Start()
    {
        savedData = SaveManager.Load();

        ConfigureSavedData();
        UpdateHighScoreText();
        ShowNewIndicator();
        ShowNoAdsPopup();
        if (DeviceTypeChecker.IsTablet())
        {
            AdjustTabletSpacing();
        }
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
            StartCoroutine(SwayNewIndicator());
        }
    }

    private void ConfigureSavedData()
    {
        if (!savedData.CanShowAds)
        {
            noAdsButton.gameObject.SetActive(false);
        }

        if (savedData.ExtraChance.ActiveCount > 0)
        {
            savedData.ExtraChance.ActiveCount = 0;
            SaveManager.Save(savedData);
        }

        if (savedData.SelectedPerks.SelectedSpecial.Contains(PerkEnum.GoldTrophy))
        {
            goldTrophy.SetActive(true);
            StartCoroutine(SwayTrophy());
        }
    }

    private void ShowNoAdsPopup()
    {
        const int popupFrequency = 25;

        // Check if ads can be shown
        if (!savedData.CanShowAds)
        {
            return;
        }

        if (savedData.GamesPlayed - savedData.LastNoAdsPopupGameCount >= popupFrequency)
        {
            savedData.LastNoAdsPopupGameCount = savedData.GamesPlayed;
            SaveManager.Save(savedData);

            noAdsButton.onClick.Invoke();
        }
    }

    private IEnumerator SwayTrophy()
    {
        while (true)
        {
            float angle = swayAngle * Mathf.Sin(Time.time * swaySpeed);
            goldTrophy.transform.rotation = Quaternion.Euler(0, 0, angle);
            yield return null;
        }
    }

    private IEnumerator SwayNewIndicator()
    {
        while (true)
        {
            float angle = swayAngle * Mathf.Sin(Time.time * swaySpeed);
            newIndicator.transform.rotation = Quaternion.Euler(0, 0, angle);
            yield return null;
        }
    }

    private void AdjustTabletSpacing()
    {
        Vector3 amountToRaise = Vector3.up * 30;

        logo.transform.localPosition += amountToRaise;
        highScoreTitleText.transform.localPosition += amountToRaise;
        highScoreText.transform.localPosition += amountToRaise;
        goldTrophy.transform.localPosition -= amountToRaise;

        playButton.transform.localScale *= 0.75f;
        perksButton.transform.localScale *= 0.75f;
        perksButton.transform.localPosition += amountToRaise;

    }
}
