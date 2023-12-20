using System;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener, IUnityAdsInitializationListener
{
    public event Action OnAdClosedOrFailed;

    private string gameId = "5504399";
    private string surfacingId = "Interstitial_iOS";
    private bool testMode = true;
    private bool isAdReady = false;
    private bool showRequested = false;

    public static AdsManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            #if UNITY_ANDROID
                gameId = "5504398";
                surfacingId = "Interstitial_Android";
            #endif

            Advertisement.Initialize(gameId, testMode, this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadAd()
    {
        isAdReady = false;
        Advertisement.Load(surfacingId, this);
    }

    public void ShowAd()
    {
        if (isAdReady)
        {
            Advertisement.Show(surfacingId, this);
            showRequested = false;
        }
        else
        {
            Debug.Log("Ad is not ready yet, waiting for load.");
            showRequested = true;
        }
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log("Ad loaded and ready to show!");
        isAdReady = true;

        if (showRequested)
        {
            Advertisement.Show(surfacingId, this);
            showRequested = false;
        }
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogError($"Error loading Ad on {placementId}: {error} - {message}");
        isAdReady = false;

        OnAdClosedOrFailed?.Invoke();
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.LogError($"Error showing Ad on {placementId}: {error} - {message}");

        OnAdClosedOrFailed?.Invoke();
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log($"Ad on {placementId} completed with result {showCompletionState}");
        isAdReady = false;
        LoadAd();

        OnAdClosedOrFailed?.Invoke();
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log($"Ad on {placementId} started showing.");
        // Optionally pause your game or mute audio
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log($"Ad on {placementId} clicked.");
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
        LoadAd();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.LogError($"Unity Ads Initialization Failed: {error} - {message}");
    }
}