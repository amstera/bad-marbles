using UnityEngine;
using TMPro;

public class PointsText : MonoBehaviour
{
    public float lifetime = 0.75f;
    public float riseSpeed = 18f;
    public float moveAwaySpeed = 5f;
    public TextMeshPro textMesh;

    private float timer;
    private Camera mainCamera;
    private Vector3 targetWorldPosition;


    private void Start()
    {
        timer = 0;
        mainCamera = Camera.main;

        float zDepth = 10f;
        Vector3 screenTopLeft = new Vector3(0, Screen.height, zDepth);
        targetWorldPosition = mainCamera.ScreenToWorldPoint(screenTopLeft);
    }

    public void SetPoints(int points)
    {
        textMesh.text = $"+{points}";
    }

    private void Update()
    {
        timer += Time.deltaTime;

        Vector3 direction = targetWorldPosition - transform.position;
        direction.z = 0; // Ignore z-axis for direction calculation
        direction.Normalize();

        transform.position += direction * riseSpeed * Time.deltaTime; // Move towards target in X and Y
        transform.position += new Vector3(0, 0, moveAwaySpeed * Time.deltaTime); // Move away in Z

        float fadeProgress = timer / lifetime;
        float alpha = Mathf.Clamp01(1.0f - fadeProgress);
        textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, alpha);

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}
