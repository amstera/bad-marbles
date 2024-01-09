using UnityEngine;
using TMPro;

public class PointsText : MonoBehaviour
{
    public float baseLifetime = 0.6f;
    public float riseSpeed = 22f;
    public float moveAwaySpeed = 6f;
    public TextMeshPro textMesh;

    private float lifetime;
    private float timer;
    private Camera mainCamera;
    private Vector3 targetWorldPosition;
    private bool setLives;

    private void Start()
    {
        timer = 0;
        mainCamera = Camera.main;

        float zDepth = 10f;
        Vector3 screenTop = new Vector3(setLives ? Screen.width - 25 : 0, Screen.height, zDepth);
        targetWorldPosition = mainCamera.ScreenToWorldPoint(screenTop);

        AdjustLifetimeBasedOnYPosition();
    }

    private void AdjustLifetimeBasedOnYPosition()
    {
        float defaultYPosition = -9.22f;
        float yPosDifference = transform.position.y - defaultYPosition;
        lifetime = Mathf.Max(baseLifetime - yPosDifference * 0.01f, 0); 
    }

    public void SetPoints(int points)
    {
        textMesh.text = $"+{points}";
    }

    public void SetLives(bool isLoss)
    {
        if (isLoss)
        {
            textMesh.text = $"-♥";
            textMesh.colorGradient = new VertexGradient(Color.red, Color.red, Color.red, Color.red);
        }
        else
        {
            textMesh.text = $"+♥";
        }

        riseSpeed = 18f;
        setLives = true;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        Vector3 direction = targetWorldPosition - transform.position;
        direction.z = 0;
        direction.Normalize();

        transform.position += direction * riseSpeed * Time.deltaTime;
        transform.position += new Vector3(0, 0, moveAwaySpeed * Time.deltaTime);

        float fadeProgress = timer / lifetime;
        float alpha = Mathf.Clamp01(1.0f - fadeProgress);
        textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, alpha);

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}
