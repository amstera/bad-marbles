using System.Collections;
using UnityEngine;

public class Marble : MonoBehaviour
{
    public float speed = 10f;
    private Rigidbody rb;
    public ShatteredMarble shatteredMarblePrefab;
    public MarbleColor color;
    public int points;
    public int livesLost;
    public float fadeInDuration = 0.5f;

    // Sound variables
    public AudioSource rollingSound;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        Renderer marbleRenderer = GetComponent<Renderer>();
        Color originalColor = marbleRenderer.material.color;
        float elapsedTime = 0;

        while (elapsedTime < fadeInDuration)
        {
            float alpha = elapsedTime / fadeInDuration;
            marbleRenderer.material.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        marbleRenderer.material.color = originalColor;
    }

    void FixedUpdate()
    {
        if (rb.velocity.magnitude < speed)
        {
            // Apply force in the direction of the current movement to reach the desired speed
            rb.AddForce(rb.velocity.normalized * speed, ForceMode.Acceleration);
        }
    }

    void Update()
    {
        if (GameManager.Instance.Lives <= 0)
        {
            return;
        }

        // Check for mouse click or touch input
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
            {
                DestroyMarble();
            }
        }
    }

    void DestroyMarble()
    {
        if (shatteredMarblePrefab != null)
        {
            ShatteredMarble shatteredMarble = Instantiate(shatteredMarblePrefab, transform.position, transform.rotation);
            shatteredMarble.SetMaterial(GetComponent<Renderer>().material);
        }

        Destroy(gameObject);
    }
}

public enum MarbleColor
{
    Unknown = 0,
    Green = 1,
    Red = 2
}
