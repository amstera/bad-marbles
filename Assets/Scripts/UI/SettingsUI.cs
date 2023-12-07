using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Audio;

public class SettingsUI : MonoBehaviour, IPointerDownHandler
{
    public CanvasGroup canvasGroup;

    public Slider musicVolumeSlider;
    public Toggle sfxToggle;

    public AudioMixer audioMixer;
    public AudioSource backgroundMusic;
    public AudioSource plopSound;

    private SaveObject savedData;
    private float fadeDuration = 0.25f;
    private float defaultBackgroundMusicVolume;

    private void Start()
    {
        savedData = SaveManager.Load();
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;

        defaultBackgroundMusicVolume = backgroundMusic.volume;
        backgroundMusic.volume = savedData.Settings.Volume * defaultBackgroundMusicVolume;

        musicVolumeSlider.value = savedData.Settings.Volume;
        sfxToggle.isOn = savedData.Settings.SFXEnabled;
        SetSFXVolume(sfxToggle.isOn);

        musicVolumeSlider.onValueChanged.AddListener(HandleVolumeChange);
        sfxToggle.onValueChanged.AddListener(HandleSfxChange);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Check if the touch is on the panel itself and not on the popup
        if (eventData.pointerCurrentRaycast.gameObject == gameObject)
        {
            StartCoroutine(FadeOut());
        }
    }

    public void ShowPanel()
    {
        StartCoroutine(FadeIn());
    }

    private void SetSFXVolume(bool enabled)
    {
        float volume = enabled ? 0f : -80f;
        audioMixer.SetFloat("SFXVolume", volume);
    }

    private IEnumerator FadeIn()
    {
        float startTime = Time.time;
        while (Time.time < startTime + fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, (Time.time - startTime) / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
    }

    private IEnumerator FadeOut()
    {
        float startTime = Time.time;
        while (Time.time < startTime + fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, (Time.time - startTime) / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }

    private void HandleVolumeChange(float value)
    {
        backgroundMusic.volume = value * defaultBackgroundMusicVolume;

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
}