using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMusicPlayer : MonoBehaviour
{
    public static GameMusicPlayer Instance = null;
    public AudioSource backgroundMusic;

    private SaveObject savedData;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;

            savedData = SaveManager.Load();
            SetMusic();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Game")
        {
            Destroy(gameObject);
            return;
        }
    }

    public void Play()
    {
        if (!backgroundMusic.isPlaying)
        {
            backgroundMusic.Play();
        }
    }

    public void Pause()
    {
        if (backgroundMusic.isPlaying)
        {
            backgroundMusic.Pause();
        }
    }

    private void SetMusic()
    {
        var bgMusicData = MusicService.GetTrack(savedData);
        backgroundMusic.clip = bgMusicData.clip;
        backgroundMusic.volume = bgMusicData.volume;
        backgroundMusic.Play();
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
