using System.Collections;
using UnityEngine;

public class ShatteredMarble : MonoBehaviour
{
    public float scatterForce = 5f;
    public float fadeOutTime = 3f;
    private Material marbleMaterial;
    private bool isFading = false;
    private float fadeStartTime;

    public GameObject explosionPrefab;

    void Start()
    {
        Destroy(Instantiate(explosionPrefab, transform.position, Quaternion.identity), 1);

        // Apply force to each child's rigidbody
        foreach (Transform child in transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer("ShatteredMarble");

            Rigidbody childRb = child.GetComponent<Rigidbody>();
            if (childRb != null)
            {
                childRb.AddForce(Random.onUnitSphere * scatterForce, ForceMode.Impulse);
            }

            // Set the material for each child
            Renderer childRenderer = child.GetComponent<Renderer>();
            if (childRenderer != null && marbleMaterial != null)
            {
                childRenderer.material = marbleMaterial;
            }
        }

        fadeStartTime = Time.time;
    }

    void Update()
    {
        // Start fading out after the specified time
        if (!isFading && Time.time - fadeStartTime > fadeOutTime)
        {
            isFading = true;
            StartCoroutine(FadeOutAndDestroy());
        }
    }

    public void SetMaterial(Material material)
    {
        marbleMaterial = material;
    }

    IEnumerator FadeOutAndDestroy()
    {
        float elapsedTime = 0;
        while (elapsedTime < fadeOutTime)
        {
            foreach (Transform child in transform)
            {
                Renderer childRenderer = child.GetComponent<Renderer>();
                if (childRenderer != null)
                {
                    Color color = childRenderer.material.color;
                    color.a = 1 - (elapsedTime / fadeOutTime);
                    childRenderer.material.color = color;
                }
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}