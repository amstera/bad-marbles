using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class VignetteAnimator : MonoBehaviour
{
    public PostProcessVolume postProcessVolume;
    private Vignette vignette;

    public float initialIntensity = 0.15f;

    private float transitionDuration = 105;
    private float targetIntensity = 0.55f;
    private float elapsedTime = 0f;

    void Start()
    {
        if (postProcessVolume == null || !postProcessVolume.profile.TryGetSettings(out vignette))
        {
            Debug.LogError("PostProcessVolume not assigned or doesn't contain a Vignette.");
            return;
        }

        vignette.intensity.value = Mathf.Min(initialIntensity, targetIntensity);
    }

    void Update()
    {
        if (GameManager.Instance.Lives > 0 && elapsedTime < transitionDuration && vignette.intensity.value < targetIntensity)
        {
            elapsedTime += Time.deltaTime;
            float newIntensity = Mathf.Lerp(initialIntensity, targetIntensity, elapsedTime / transitionDuration);
            vignette.intensity.value = newIntensity;
        }
    }
}