using System.Collections;
using UnityEngine;

public class BombMarble : Marble
{
    public Material flashingMaterial;
    private Material initialMaterial;
    private Renderer marbleRenderer;
    private bool isFlashing;
    private float flashDuration = 0.15f;
    private Vector3 originalScale;
    private float scaleMultiplier = 1.2f;

    private float currentAlpha = 1f;

    void Awake()
    {
        marbleRenderer = GetComponent<Renderer>();
        initialMaterial = marbleRenderer.material;
        originalScale = transform.localScale;
    }

    void OnEnable()
    {
        StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        while (true)
        {
            yield return StartCoroutine(Flash(flashDuration));
            isFlashing = !isFlashing;
        }
    }

    IEnumerator Flash(float duration)
    {
        Material targetMaterial = isFlashing ? initialMaterial : flashingMaterial;
        Vector3 targetScale = isFlashing ? originalScale : originalScale * scaleMultiplier;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float lerpFactor = elapsedTime / duration;
            AdjustMaterialAlpha(targetMaterial, Mathf.Lerp(0f, currentAlpha, lerpFactor));
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, lerpFactor);
            yield return null;
        }

        marbleRenderer.material = targetMaterial;
        AdjustMaterialAlpha(targetMaterial, currentAlpha);
        transform.localScale = targetScale;
    }

    private void AdjustMaterialAlpha(Material material, float alpha)
    {
        if (material != marbleRenderer.material)
        {
            material = marbleRenderer.material;
        }

        Color color = material.color;
        color.a = alpha;
        material.color = color;
    }
}