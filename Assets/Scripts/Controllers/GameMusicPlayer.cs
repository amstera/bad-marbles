using UnityEngine;

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
            savedData = SaveManager.Load();
            SetMusic();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void SetMusic()
    {
        var bgMusicData = MusicService.GetTrack(savedData);
        backgroundMusic.clip = bgMusicData.clip;
        backgroundMusic.volume = bgMusicData.volume;
        backgroundMusic.Play();
    }
}
