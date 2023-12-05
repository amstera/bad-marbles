using UnityEngine;
using System.IO;

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
}
