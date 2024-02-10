using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI.ProceduralImage;
using TMPro;
using Unity.Services.Core;
using Unity.Services.Analytics;

public class LoadingScreenManager : MonoBehaviour
{
    public ProceduralImage progressBar;
    public TextMeshProUGUI footerText;

    async void Awake()
    {
        await UnityServices.InitializeAsync();

        AnalyticsService.Instance.StartDataCollection();
    }

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;

        SetFooterText();
        StartCoroutine(LoadAndPrepareAssets());
    }

    IEnumerator LoadAndPrepareAssets()
    {
        string folderPath = "Prefabs";
        Object[] loadedPrefabs = Resources.LoadAll(folderPath, typeof(GameObject));
        int totalPrefabs = loadedPrefabs.Length;
        int batchSize = 10;

        for (int i = 0; i < totalPrefabs; i++)
        {
            GameObject prefab = (GameObject)loadedPrefabs[i];
            if (prefab != null)
            {
                GameObject temp = Instantiate(prefab);
                AudioSource[] audioSources = temp.GetComponentsInChildren<AudioSource>();
                foreach (var audioSource in audioSources)
                {
                    audioSource.enabled = false;
                }

                yield return null; // Spread out instantiation
                Destroy(temp);
            }

            UpdateProgressBar(i + 1, totalPrefabs);

            if ((i + 1) % batchSize == 0)
            {
                yield return null;
                System.GC.Collect();
            }
        }

        Debug.Log("Resource preloading and preparation complete!");
        LoadMenuScene();
    }

    void UpdateProgressBar(int currentIndex, int totalPrefabs)
    {
        float totalProgress = (float)currentIndex / totalPrefabs;
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