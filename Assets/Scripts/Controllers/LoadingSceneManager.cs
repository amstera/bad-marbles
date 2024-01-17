using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI.ProceduralImage;
using TMPro;

public class LoadingScreenManager : MonoBehaviour
{
    public ProceduralImage progressBar;
    public TextMeshProUGUI footerText;

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;

        SetFooterText();
        StartCoroutine(LoadAndPrepareAssets());
    }

    IEnumerator LoadAndPrepareAssets()
    {
        string folderPath = "Prefabs/Marbles";
        Object[] loadedPrefabs = Resources.LoadAll(folderPath, typeof(GameObject));
        int totalPrefabs = loadedPrefabs.Length;

        for (int i = 0; i < totalPrefabs; i++)
        {
            GameObject prefab = (GameObject)loadedPrefabs[i];

            // Instantiate and then destroy the prefab
            if (prefab == null)
            {
                Debug.Log($"Couldn't load prefab!");
            }
            else
            {
                Destroy(Instantiate(prefab));
            }

            // Update progress bar
            UpdateProgressBar(i + 1, 0, totalPrefabs);
            yield return null;
        }

        Debug.Log("Resource preloading and preparation complete!");
        LoadMenuScene();
    }

    void UpdateProgressBar(int currentIndex, float currentProgress, int totalPrefabs)
    {
        float totalProgress = (currentIndex + currentProgress) / totalPrefabs;
        progressBar.fillAmount = totalProgress;
    }

    void LoadMenuScene()
    {
        SceneManager.LoadScene("Menu");
    }

    void SetFooterText()
    {
        footerText.text = $"Bad Marbles © {System.DateTime.Now.Year} Green Tea Gaming - Version {Application.version}";
    }
}