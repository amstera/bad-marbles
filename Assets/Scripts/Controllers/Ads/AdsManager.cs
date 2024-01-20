using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour, IUnityAdsInitializationListener
{
    public static AdsManager Instance { get; private set; }

    public InterstitialAd interstitialAd;
    public RewardedAd rewardedAd;

    private string gameId;
    private bool testMode = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            #if UNITY_ANDROID
                gameId = "5504398"; // Android Game ID
            #else
                gameId = "5504399"; // Default to iOS Game ID
            #endif

            InitializeAds();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAds()
    {
        Advertisement.Initialize(gameId, testMode, this);
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
        interstitialAd.LoadAd();
        rewardedAd.LoadAd();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.LogError($"Unity Ads Initialization Failed: {error} - {message}");
    }
}