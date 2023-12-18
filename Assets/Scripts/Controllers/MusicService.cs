using UnityEngine;
using System.Collections.Generic;

public static class MusicService
{
    private static readonly Dictionary<PerkEnum, (AudioClip clip, float volume)> _musicTracks;

    static MusicService()
    {
        _musicTracks = new Dictionary<PerkEnum, (AudioClip, float)>
        {
            { PerkEnum.DefaultMusic, (Resources.Load<AudioClip>("Sounds/Music/MainTheme"), 0.4f) },
            { PerkEnum.RockinMarbles, (Resources.Load<AudioClip>("Sounds/Music/RockinMarbles"), 0.35f) },
            { PerkEnum.MarblesAnthem, (Resources.Load<AudioClip>("Sounds/Music/MarblesAnthem"), 0.25f) },
            { PerkEnum.MarblesSong4, (Resources.Load<AudioClip>("Sounds/Music/LostMarbles"), 0.25f) }
        };
    }

    public static (AudioClip clip, float volume) GetTrack(SaveObject savedData)
    {
        var selectedMusic = savedData.SelectedPerks.SelectedMusic;

        // Retrieve the selected music track and its default volume
        if (_musicTracks.TryGetValue(selectedMusic, out var trackInfo))
        {
            // Adjust volume based on user settings
            float adjustedVolume = trackInfo.volume * savedData.Settings.Volume;
            return (trackInfo.clip, adjustedVolume);
        }

        return ( null, 0f); // Return null if no matching track is found
    }
}