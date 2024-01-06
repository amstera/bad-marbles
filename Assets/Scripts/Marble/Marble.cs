using System;
using System.Collections;
using UnityEngine;

public class Marble : MonoBehaviour
{
    public float speed = 10f;
    protected Rigidbody rb;
    public ShatteredMarble shatteredMarblePrefab;
    public MarbleColor color;
    public int points;
    public int livesLost;
    public float fadeInDuration = 0.5f;

    public event Action OnDestroyed;

    protected void Start()
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

    protected void FixedUpdate()
    {
        if (rb.velocity.magnitude < speed)
        {
            rb.AddForce(rb.velocity.normalized * speed, ForceMode.Acceleration);
        }
    }

    public void Destroy()
    {
        if (shatteredMarblePrefab != null)
        {
            ShatteredMarble shatteredMarble = Instantiate(shatteredMarblePrefab, transform.position, transform.rotation);
            if (color == MarbleColor.BigRed)
            {
                shatteredMarble.transform.localScale *= 2;
            }

            shatteredMarble.SetMaterial(GetComponent<Renderer>().material);
        }

        OnDestroyed?.Invoke();
        Destroy(gameObject);
    }

}

public enum MarbleColor
{
    Tier = -1,
    Unknown = 0,
    Green = 1,
    Red = 2,
    Fire = 3,
    Angel = 4,
    Gold = 5,
    BigRed = 6,
    Bomb = 7
}
