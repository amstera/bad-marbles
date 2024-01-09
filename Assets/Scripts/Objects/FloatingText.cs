using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    private float floatSpeed = 4f;
    private float fadeDuration = 1.5f;
    public TextMeshPro textMesh;
    private float startTime;

    void Start()
    {
        startTime = Time.time;
    }

    void Update()
    {
        if (textMesh == null) return;

        // Floating effect
        transform.Translate(Vector3.up * floatSpeed * Time.deltaTime);

        // Fading effect
        float t = (Time.time - startTime) / fadeDuration;
        textMesh.alpha = Mathf.Lerp(1.0f, 0, t);

        // Destroy the text object after fading
        if (Time.time > startTime + fadeDuration)
        {
            Destroy(gameObject);
        }
    }
}
