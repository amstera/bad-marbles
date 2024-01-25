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

    private Coroutine flashCoroutine;

    void Awake()
    {
        marbleRenderer = GetComponent<Renderer>();
        initialMaterial = marbleRenderer.material;
        originalScale = transform.localScale;
    }

    void OnEnable()
    {
        ResetBombState();
        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    void OnDisable()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
    }

    void ResetBombState()
    {
        marbleRenderer.material = initialMaterial;
        transform.localScale = originalScale;
        isFlashing = false;
    }

    IEnumerator FlashRoutine()
    {
        while (true)
        {
            yield return Flash(flashDuration, isFlashing ? initialMaterial : flashingMaterial);
            isFlashing = !isFlashing;
        }
    }

    IEnumerator Flash(float duration, Material targetMaterial)
    {
        float elapsedTime = 0;
        Material startMaterial = marbleRenderer.material;
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = isFlashing ? originalScale : originalScale * scaleMultiplier;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            marbleRenderer.material.Lerp(startMaterial, targetMaterial, elapsedTime / duration);
            transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / duration);
            yield return null;
        }

        marbleRenderer.material = targetMaterial;
        transform.localScale = targetScale;
    }
}
