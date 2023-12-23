public class InterstitialAd : AdBase
{
    void Start() // Removed the 'protected override'
    {
        // Set default surfacing ID for iOS
        surfacingId = "Interstitial_iOS";

        #if UNITY_ANDROID
                surfacingId = "Interstitial_Android";
        #endif

        base.LoadAd(); // Call LoadAd from the base class
    }

    // The rest of the class remains unchanged
}