using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Audio;
using TMPro;

public class SettingsUI : MonoBehaviour, IPointerDownHandler
{
    #region UI Components
    public Button gearButton;
    public CanvasGroup canvasGroup;
    public Slider musicVolumeSlider;
    public Toggle sfxToggle;
    public TextMeshProUGUI gamesPlayedAmountText, highestStreakAmountText, highestTierAmountText;
    #endregion

    #region Audio Components
    public AudioMixer audioMixer;
    public AudioSource plopSound;
    #endregion

    private SaveObject savedData;
    private float fadeDuration = 0.35f;
    private const float SFX_VOLUME_OFF = -80f;
    private const string SFX_VOLUME_PARAM = "SFXVolume";

    private void Start()
    {
        savedData = SaveManager.Load();
        InitializeUI();
    }

    private void InitializeUI()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;

        UpdateAudioSettings();

        musicVolumeSlider.onValueChanged.AddListener(HandleVolumeChange);
        sfxToggle.onValueChanged.AddListener(HandleSfxChange);

        UpdateStatTexts();
    }

    private void UpdateAudioSettings()
    {
        musicVolumeSlider.value = savedData.Settings.Volume;
        sfxToggle.isOn = savedData.Settings.SFXEnabled;
        SetSFXVolume(sfxToggle.isOn);
    }

    private void UpdateStatTexts()
    {
        gamesPlayedAmountText.text = savedData.GamesPlayed.ToString();
        highestStreakAmountText.text = $"{savedData.HighStreak / 10 + 1}X";
        highestTierAmountText.text = savedData.HighTier.ToString();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject == gameObject)
        {
            StartCoroutine(Fade(false));
            StartCoroutine(RotateGear(0));
        }
    }

    public void ShowPanel()
    {
        StartCoroutine(Fade(true));
        StartCoroutine(RotateGear(45));
    }

    private void SetSFXVolume(bool enabled)
    {
        audioMixer.SetFloat(SFX_VOLUME_PARAM, enabled ? 0f : SFX_VOLUME_OFF);
    }

    private IEnumerator Fade(bool fadeIn)
    {
        float targetAlpha = fadeIn ? 1f : 0f;
        float startTime = Time.time;
        while (Time.time < startTime + fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, (Time.time - startTime) / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = targetAlpha;
        canvasGroup.blocksRaycasts = fadeIn;
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