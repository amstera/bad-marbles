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
    public GameObject popUp;
    public Slider musicVolumeSlider;
    public Toggle sfxToggle, vibrationsToggle;
    public TextMeshProUGUI gamesPlayedAmountText, highestStreakAmountText, highestTierAmountText;
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

    private void Start()
    {
        savedData = SaveManager.Load();
        InitializeUI();
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
        highestStreakAmountText.text = $"{savedData.HighStreak / 10 + 1}X";
        highestTierAmountText.text = savedData.HighTier.ToString();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject == gameObject)
        {
            StartCoroutine(Fade(false));
            popUp.transform.localScale = Vector3.zero;
            StartCoroutine(RotateGear(0));
        }
    }

    public void ShowPanel()
    {
        StartCoroutine(Fade(true));
        StartCoroutine(PopIn(popUp, popUpDuration));
        StartCoroutine(RotateGear(45));
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