using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class VignetteAnimator : MonoBehaviour
{
    public PostProcessVolume postProcessVolume;
    public SpriteRenderer background;
    private Vignette vignette;

    public float elapsedTime = 0f;
    private bool isPaused = false;
    private Coroutine animationCoroutine;

    private const float InitialIntensity = 0.15f;
    private const float TransitionDuration = 140;
    private const float TargetIntensity = 0.375f;
    private Color startColor = new Color32(191, 255, 250, 255);
    private Color targetColor = Color.white;

    void Start()
    {
        if (postProcessVolume == null || !postProcessVolume.profile.TryGetSettings(out vignette))
        {
            Debug.LogError("PostProcessVolume not assigned or doesn't contain a Vignette.");
            return;
        }

        vignette.intensity.value = InitialIntensity;
        animationCoroutine = StartCoroutine(AnimateVignetteAndBackground());
    }

    private IEnumerator AnimateVignetteAndBackground()
    {
        while (elapsedTime < TransitionDuration)
        {
            if (!isPaused)
            {
                UpdateVignetteAndBackground();
                elapsedTime += Time.deltaTime;
            }

            yield return null;
        }

        // Ensure final values are set
        if (vignette != null)
        {
            vignette.intensity.value = TargetIntensity;
        }
        if (background != null)
        {
            background.color = targetColor;
        }
    }

    private void UpdateVignetteAndBackground()
    {
        float progress = Mathf.Clamp(elapsedTime / TransitionDuration, 0f, 1f);
        if (vignette != null)
        {
            vignette.intensity.value = Mathf.Lerp(InitialIntensity, TargetIntensity, progress);
        }
        if (background != null)
        {
            background.color = Color.Lerp(startColor, targetColor, progress);
        }
    }

    public void SetElapsedTime(float newElapsedTime)
    {
        elapsedTime = newElapsedTime;
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        animationCoroutine = StartCoroutine(AnimateVignetteAndBackground());
    }

    public void Pause(bool pause)
    {
        isPaused = pause;
    }
}