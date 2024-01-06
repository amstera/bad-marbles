using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class VignetteAnimator : MonoBehaviour
{
    public PostProcessVolume postProcessVolume;
    public SpriteRenderer background;
    private Vignette vignette;

    public float elapsedTime = 0f;

    private float initialIntensity = 0.15f;
    private float transitionDuration = 105;
    private float targetIntensity = 0.55f;

    private Color startColor = Color.white; // White color
    private Color targetColor = new Color32(55, 125, 210, 255); // Specific blue color

    void Start()
    {
        if (postProcessVolume == null || !postProcessVolume.profile.TryGetSettings(out vignette))
        {
            Debug.LogError("PostProcessVolume not assigned or doesn't contain a Vignette.");
            return;
        }

        vignette.intensity.value = Mathf.Min(initialIntensity, targetIntensity);
        background.color = startColor;
    }

    void Update()
    {
        if (GameManager.Instance.Lives > 0 && elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;

            if (vignette.intensity.value < targetIntensity)
            {
                float newIntensity = Mathf.Lerp(initialIntensity, targetIntensity, elapsedTime / transitionDuration);
                vignette.intensity.value = newIntensity;
            }

            background.color = Color.Lerp(startColor, targetColor, elapsedTime / transitionDuration);
        }
    }
}