using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{
    private static string saveFilePath = Application.persistentDataPath + "/savefile.json";

    public static void Save(SaveObject saveObject)
    {
        string json = JsonUtility.ToJson(saveObject);
        string encryptedJson = CryptoManager.EncryptString(json);
        File.WriteAllText(saveFilePath, encryptedJson);
    }

    public static SaveObject Load()
    {
        if (File.Exists(saveFilePath))
        {
            string encryptedJson = File.ReadAllText(saveFilePath);
            string decryptedJson = CryptoManager.DecryptString(encryptedJson);
            return JsonUtility.FromJson<SaveObject>(decryptedJson);
        }
        return new SaveObject();
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

    public Settings Settings;
    public SelectedPerks SelectedPerks;
}

[System.Serializable]
public class Settings
{
    public float Volume = 1;
    public bool SFXEnabled = true;
}

[System.Serializable]
public class SelectedPerks
{
    public List<PerkEnum> SelectedSpecial = new List<PerkEnum>();
    public PerkEnum SelectedMusic = PerkEnum.DefaultMusic;
    public PerkEnum SelectedBackground = PerkEnum.DefaultBackground;
    public PerkEnum SelectedRamp = PerkEnum.DefaultRamp;
}
