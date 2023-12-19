using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuMusicPlayer : MonoBehaviour
{
    public static MenuMusicPlayer Instance = null;

    public AudioSource backgroundMusic;
    public AudioClip backgroundMusicClip;

    private SaveObject savedData;
    private float defaultBackgroundMusicVolume = 0.4f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            Destroy(gameObject);
            return;
        }

        savedData = SaveManager.Load();

        UpdateVolume(savedData.Settings.Volume);

        if (backgroundMusic.clip != backgroundMusicClip)
        {
            backgroundMusic.clip = backgroundMusicClip;
        }
        if (!backgroundMusic.isPlaying)
        {
            backgroundMusic.Play();
        }
    }

    public void UpdateVolume(float volume)
    {
        backgroundMusic.volume = volume * defaultBackgroundMusicVolume;
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
