using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{
    private static string saveFilePath = Application.persistentDataPath + "/savefile.json";
    private static SaveObject cachedSaveObject = null;

    public static void Save(SaveObject saveObject)
    {
        string json = JsonUtility.ToJson(saveObject);
        string encryptedJson = CryptoManager.EncryptString(json);
        File.WriteAllText(saveFilePath, encryptedJson);

        cachedSaveObject = saveObject;
    }

    public static SaveObject Load()
    {
        if (cachedSaveObject != null)
        {
            return cachedSaveObject;
        }

        if (File.Exists(saveFilePath))
        {
            string encryptedJson = File.ReadAllText(saveFilePath);
            string decryptedJson = CryptoManager.DecryptString(encryptedJson);
            cachedSaveObject = JsonUtility.FromJson<SaveObject>(decryptedJson);
            return cachedSaveObject;
        }

        cachedSaveObject = new SaveObject();
        return cachedSaveObject;
    }

    public static void Clear()
    {
        cachedSaveObject = new SaveObject();

        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
        }
    }

}

[System.Serializable]
public class SaveObject
{
    public int HighScore;
    public int Points;
    public int HighTier = 1;
    public int GamesPlayed;
    public int HighStreak = 1;
    public int HighStreakMultiplier = 1;
    public int LastNoAdsPopupGameCount;

    public bool CanShowAds = true;
    public bool HasShownRateApp;
    public bool HasSeenTutorial;
    public bool HasSeenPerksTutorial;
    public bool HasSeenPerksPopup;
    public bool LastGameShowedAd;

    public List<PerkEnum> SeenDescription = new List<PerkEnum>();

    public Settings Settings = new Settings();
    public SelectedPerks SelectedPerks = new SelectedPerks();
    public ExtraChance ExtraChance = new ExtraChance();
}

[System.Serializable]
public class Settings
{
    public float Volume = 1;
    public bool SFXEnabled = true;
    public bool VibrationEnabled = true;
}

[System.Serializable]
public class SelectedPerks
{
    public List<PerkEnum> SelectedSpecial = new List<PerkEnum>();
    public int LastSpecialPoints = 0;
    public PerkEnum SelectedMusic = PerkEnum.DefaultMusic;
    public int LastMusicPoints = 0;
    public PerkEnum SelectedBackground = PerkEnum.DefaultBackground;
    public int LastBackgroundPoints = 0;
    public PerkEnum SelectedRamp = PerkEnum.DefaultRamp;
    public int LastRampPoints = 0;
}

[System.Serializable]
public class ExtraChance
{
    public int ActiveCount;
    public int Score;
    public int Tier;
    public float MarbleSpawnSpeed;
    public float ElapsedTime;
}

