using UnityEngine;
using TMPro;

public class PointsText : MonoBehaviour
{
    public float lifetime = 0.6f;
    public TextMeshPro textMesh;

    private float timer;
    private float riseSpeed = 22f;
    private float moveAwaySpeed = 6f;
    private float zDepth = 10;
    private static Camera mainCamera;
    private Vector3 targetWorldPosition;
    private Vector3 direction;
    private Color textColor;

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        textColor = textMesh.color;
    }

    public void SetPoints(int points)
    {
        textMesh.text = $"+{points}";
        InitializeText(new Vector3(0, Screen.height, zDepth));
    }

    public void SetLives(int lifeChange)
    {
        string heartSymbol = "♥";
        string sign = lifeChange >= 0 ? "+" : "-";
        int numberOfHearts = Mathf.Abs(lifeChange);
        string hearts = new string(heartSymbol[0], numberOfHearts);

        textMesh.text = $"{sign}{hearts}";

        if (lifeChange < 0)
        {
            textMesh.colorGradient = new VertexGradient(Color.red);
        }
        riseSpeed = 18f;
        InitializeText(new Vector3(Screen.width - 25, Screen.height, zDepth));
    }

    private void InitializeText(Vector3 screenTop)
    {
        timer = 0;
        targetWorldPosition = mainCamera.ScreenToWorldPoint(screenTop);
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (direction == Vector3.zero)
        {
            direction = targetWorldPosition - transform.position;
            direction.z = 0;
            direction.Normalize();
        }

        transform.position += direction * riseSpeed * Time.deltaTime;
        transform.position += new Vector3(0, 0, moveAwaySpeed * Time.deltaTime);

        UpdateTextAlpha();

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void UpdateTextAlpha()
    {
        float fadeProgress = timer / lifetime;
        textColor.a = Mathf.Clamp01(1.0f - fadeProgress);
        textMesh.color = textColor;
    }
}