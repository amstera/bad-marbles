using UnityEngine;
using System.Collections;

public class Heart : MonoBehaviour
{
    public Transform sphereTransform;
    private float fadeDuration = 0.5f;
    private float originalHeightDiff;

    void Start()
    {
        originalHeightDiff = transform.position.y - sphereTransform.position.y;
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        Renderer marbleRenderer = GetComponent<Renderer>();
        Color originalColor = marbleRenderer.material.color;
        float elapsedTime = 0;

        while (elapsedTime < fadeDuration)
        {
            float alpha = elapsedTime / fadeDuration;
            marbleRenderer.material.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        marbleRenderer.material.color = originalColor;
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x, sphereTransform.position.y + originalHeightDiff, sphereTransform.position.z);
    }
}