using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI.ProceduralImage;
using TMPro;
using System;

public class LoadingScreenManager : MonoBehaviour
{
    public ProceduralImage progressBar;
    public TextMeshProUGUI footerText;

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;

        SetFooterText();
        StartCoroutine(LoadAssets());
    }

    IEnumerator LoadAssets()
    {
        string folderPath = "Prefabs/Marbles";
        string[] prefabPaths = GetPrefabPaths(folderPath);

        for (int i = 0; i < prefabPaths.Length; i++)
        {
            ResourceRequest request = Resources.LoadAsync<GameObject>(prefabPaths[i]);

            while (!request.isDone)
            {
                var totalProgress = (i + request.progress) / prefabPaths.Length;
                progressBar.fillAmount = totalProgress;
                yield return null;
            }
        }

        Debug.Log("Resource preloading complete!");

        LoadMenuScene();
    }

    void LoadMenuScene()
    {
        SceneManager.LoadScene("Menu");
    }

    string[] GetPrefabPaths(string folderPath)
    {
        List<string> prefabPaths = new List<string>();

        // Load all prefabs in the specified folder
        UnityEngine.Object[] prefabs = Resources.LoadAll(folderPath, typeof(GameObject));

        foreach (var prefab in prefabs)
        {
            prefabPaths.Add(prefab.name);
        }

        return prefabPaths.ToArray();
    }

    void SetFooterText()
    {
        int currentYear = DateTime.Now.Year;
        string gameVersion = Application.version;
        footerText.text = $"Bad Marbles © {currentYear} Green Tea Mobile - Version {gameVersion}";
    }
}