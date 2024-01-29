using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Audio;
using TMPro;
using System;

public class SettingsUI : MonoBehaviour, IPointerDownHandler
{
    #region UI Components
    public Button gearButton;
    public CanvasGroup canvasGroup;
    public GameObject popUp;
    public Slider musicVolumeSlider;
    public Toggle sfxToggle, vibrationsToggle;
    public TextMeshProUGUI gamesPlayedAmountText, highestStreakAmountText, highestTierAmountText, avgScoreAmountText, footerText;
    public TutorialUI tutorialUIPrefab;
    public SupportUI supportUI;
    #endregion

    #region Page Components
    public GameObject page1;
    public GameObject page2;
    #endregion

    #region Audio Components
    public AudioMixer audioMixer;
    public AudioSource plopSound;
    #endregion

    private SaveObject savedData;
    private float fadeDuration = 0.25f;
    private float popUpDuration = 0.25f;
    private const float SFX_VOLUME_OFF = -80f;
    private const string SFX_VOLUME_PARAM = "SFXVolume";
    private float pageTransitionDuration = 0.1f;

    private void Awake()
    {
        savedData = SaveManager.Load();
        InitializeUI();
        InitializePages();
    }

    private void InitializeUI()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        popUp.transform.localScale = Vector3.zero;

        UpdateAudioSettings();

        musicVolumeSlider.onValueChanged.AddListener(HandleVolumeChange);
        sfxToggle.onValueChanged.AddListener(HandleSfxChange);
        vibrationsToggle.onValueChanged.AddListener(HandleVibrationChange);

        UpdateStatTexts();
        SetFooterText();
    }

    private void InitializePages()
    {
        page1.SetActive(true);
        page2.SetActive(false);
    }

    public void ForwardButtonPressed()
    {
        plopSound?.Play();
        StartCoroutine(TransitionPage(page1, page2));
    }

    public void BackwardButtonPressed()
    {
        plopSound?.Play();
        StartCoroutine(TransitionPage(page2, page1));
    }

    public void RestorePurchases()
    {
        IAPManager.instance.RestorePurchases();
    }

    private IEnumerator TransitionPage(GameObject fromPage, GameObject toPage)
    {
        CanvasGroup fromCanvas = fromPage.GetComponent<CanvasGroup>();
        CanvasGroup toCanvas = toPage.GetComponent<CanvasGroup>();

        // Start fading out the current page
        for (float t = 0; t < pageTransitionDuration; t += Time.deltaTime)
        {
            if (fromCanvas != null) fromCanvas.alpha = 1 - t / pageTransitionDuration;
            yield return null;
        }

        fromPage.SetActive(false);
        toPage.SetActive(true);

        // Start fading in the next page
        for (float t = 0; t < pageTransitionDuration; t += Time.deltaTime)
        {
            if (toCanvas != null) toCanvas.alpha = t / pageTransitionDuration;
            yield return null;
        }

        if (toCanvas != null) toCanvas.alpha = 1;
    }

    private void UpdateAudioSettings()
    {
        musicVolumeSlider.value = savedData.Settings.Volume;
        sfxToggle.isOn = savedData.Settings.SFXEnabled;
        vibrationsToggle.isOn = savedData.Settings.VibrationEnabled;
        SetSFXVolume(sfxToggle.isOn);
    }

    private void UpdateStatTexts()
    {
        gamesPlayedAmountText.text = savedData.GamesPlayed.ToString();
        int oldStreakCalculation = savedData.HighStreak / 10 + 1;
        highestStreakAmountText.text = savedData.HighStreakMultiplier > oldStreakCalculation ? $"{savedData.HighStreakMultiplier}X" : $"{oldStreakCalculation}X";
        highestTierAmountText.text = savedData.HighTier.ToString();
        avgScoreAmountText.text = savedData.GamesPlayed == 0 ? "N/A" : $"{Math.Round((double)savedData.Points / savedData.GamesPlayed)}";
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject == gameObject)
        {
            Hide();
        }
    }

    private void Hide()
    {
        StartCoroutine(Fade(false));
        popUp.transform.localScale = Vector3.zero;
        StartCoroutine(RotateGear(0));

        page1.SetActive(true);
        page1.GetComponent<CanvasGroup>().alpha = 1;
        page2.SetActive(false);
    }


    public void ShowPanel()
    {
        gameObject.SetActive(true);

        StartCoroutine(Fade(true));
        StartCoroutine(PopIn(popUp, popUpDuration));
        StartCoroutine(RotateGear(45));
    }

    public void ShowTutorial()
    {
        var tutorialUI = Instantiate(tutorialUIPrefab, transform.parent);
        tutorialUI.Show("Close");
    }

    public void ShowSupport()
    {
        supportUI.ShowPanel();
    }

    private void SetSFXVolume(bool enabled)
    {
        audioMixer.SetFloat(SFX_VOLUME_PARAM, enabled ? 0f : SFX_VOLUME_OFF);
    }

    private IEnumerator Fade(bool fadeIn)
    {
        float targetAlpha = fadeIn ? 1f : 0f;
        float startAlpha = canvasGroup.alpha;
        float startTime = Time.time;
        while (Time.time < startTime + fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, (Time.time - startTime) / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = targetAlpha;
        canvasGroup.blocksRaycasts = fadeIn;

        if (!fadeIn)
        {
            gameObject.SetActive(false);
        }
    }

    private IEnumerator PopIn(GameObject obj, float time)
    {
        Vector3 targetScale = Vector3.one;
        for (float t = 0; t < 1; t += Time.deltaTime / time)
        {
            obj.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);
            yield return null;
        }
        obj.transform.localScale = targetScale;
    }

    private void HandleVolumeChange(float value)
    {
        MenuMusicPlayer.Instance.UpdateVolume(value);
        savedData.Settings.Volume = value;
        SaveManager.Save(savedData);
    }

    private void HandleSfxChange(bool isEnabled)
    {
        savedData.Settings.SFXEnabled = isEnabled;
        SaveManager.Save(savedData);

        SetSFXVolume(isEnabled);
        if (isEnabled)
        {
            plopSound?.Play();
        }
    }

    private void HandleVibrationChange(bool isEnabled)
    {
        savedData.Settings.VibrationEnabled = isEnabled;
        SaveManager.Save(savedData);

        if (isEnabled)
        {
            Handheld.Vibrate();
        }
    }

    private void SetFooterText()
    {
        int currentYear = DateTime.Now.Year;
        string gameVersion = Application.version;
        footerText.text = $"Bad Marbles © {currentYear} Green Tea Gaming - Version {gameVersion}";

        if (DeviceTypeChecker.IsTablet())
        {
            footerText.transform.localPosition += Vector3.down * 50;
        }
    }

    private IEnumerator RotateGear(float targetAngle)
    {
        float duration = 0.25f;
        float time = 0;
        Quaternion startRotation = gearButton.transform.rotation;
        Quaternion endRotation = Quaternion.Euler(0, 0, targetAngle);

        while (time < duration)
        {
            gearButton.transform.rotation = Quaternion.Lerp(startRotation, endRotation, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        gearButton.transform.rotation = endRotation;
    }

    private void OnDestroy()
    {
        musicVolumeSlider.onValueChanged.RemoveListener(HandleVolumeChange);
        sfxToggle.onValueChanged.RemoveListener(HandleSfxChange);
    }
}