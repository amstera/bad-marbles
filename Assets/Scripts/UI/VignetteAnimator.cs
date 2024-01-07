using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class VignetteAnimator : MonoBehaviour
{
    public PostProcessVolume postProcessVolume;
    public SpriteRenderer background;
    private Vignette vignette;

    private float elapsedTime = 0f;
    private Coroutine animationCoroutine;

    private const float InitialIntensity = 0.15f;
    private const float TransitionDuration = 110;
    private const float TargetIntensity = 0.55f;
    private readonly Color StartColor = Color.white;
    private readonly Color TargetColor = new Color32(250, 200, 130, 255);

    void Start()
    {
        if (postProcessVolume == null || !postProcessVolume.profile.TryGetSettings(out vignette))
        {
            Debug.LogError("PostProcessVolume not assigned or doesn't contain a Vignette.");
            return;
        }

        vignette.intensity.value = InitialIntensity;
        background.color = StartColor;
        animationCoroutine = StartCoroutine(AnimateVignetteAndBackground());
    }

    private IEnumerator AnimateVignetteAndBackground()
    {
        while (elapsedTime < TransitionDuration)
        {
            UpdateVignetteAndBackground();

            yield return null;

            elapsedTime += Time.deltaTime;
        }

        // Ensure final values are set
        if (vignette != null)
        {
            vignette.intensity.value = TargetIntensity;
        }
        if (background != null)
        {
            background.color = TargetColor;
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
            background.color = Color.Lerp(StartColor, TargetColor, progress);
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
}