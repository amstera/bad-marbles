using UnityEngine;
using TMPro;
using System.Collections;

public class TierText : MonoBehaviour
{
    public Transform sphereTransform;

    private float originalHeightDiff;
    private TextMeshPro textMesh;
    private float fadeDuration = 1f;
    private float waitTime = 2f;

    void Start()
    {
        originalHeightDiff = transform.position.y - sphereTransform.position.y;

        textMesh = GetComponent<TextMeshPro>();
        if (textMesh == null)
        {
            Debug.LogError("TextMeshPro component missing! Please add one to the 3D text object.");
            return;
        }

        StartCoroutine(FadeTextRoutine());
    }

    IEnumerator FadeTextRoutine()
    {
        // Fade in
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeDuration;
            SetAlpha(normalizedTime);
            yield return null;
        }
        SetAlpha(1);

        // Wait
        yield return new WaitForSeconds(waitTime);

        // Fade out
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeDuration;
            SetAlpha(1 - normalizedTime);
            yield return null;
        }
        SetAlpha(0);
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x, sphereTransform.position.y + originalHeightDiff, sphereTransform.position.z);
    }

    private void SetAlpha(float alpha)
    {
        Color color = textMesh.color;
        color.a = alpha;
        textMesh.color = color;
    }
}