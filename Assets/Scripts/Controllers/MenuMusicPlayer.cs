using UnityEngine;

public class MenuMusicPlayer : MonoBehaviour
{
    public static MenuMusicPlayer Instance = null;

    public AudioSource backgroundMusic;

    private SaveObject savedData;
    private float defaultBackgroundMusicVolume;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            savedData = SaveManager.Load();
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        defaultBackgroundMusicVolume = backgroundMusic.volume;
        UpdateVolume(savedData.Settings.Volume);

        if (!backgroundMusic.isPlaying)
        {
            backgroundMusic.Play();
        }
    }

    public void UpdateVolume(float volume)
    {
        backgroundMusic.volume = volume * defaultBackgroundMusicVolume;
    }

    public void Destroy()
    {
        Destroy(gameObject, 1);
    }
}

