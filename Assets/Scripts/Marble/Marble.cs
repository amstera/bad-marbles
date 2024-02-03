using System;
using System.Collections;
using UnityEngine;

public class Marble : MonoBehaviour
{
    public float speed = 10f;
    public Rigidbody rb;
    public ShatteredMarble shatteredMarblePrefab;
    public MarbleColor color;
    public int points;
    public int livesLost;
    public float fadeInDuration = 0.5f;

    public event Action OnDestroyed;

    protected void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void FadeIn()
    {
        StartCoroutine(FadeInAnimation());
    }

    IEnumerator FadeInAnimation()
    {
        Renderer marbleRenderer = GetComponent<Renderer>();
        MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
        marbleRenderer.GetPropertyBlock(propBlock); // Retrieve the current block to avoid overwriting other properties

        float elapsedTime = 0;
        while (elapsedTime < fadeInDuration)
        {
            float alpha = elapsedTime / fadeInDuration;
            Color colorWithAlpha = new Color(1f, 1f, 1f, Mathf.Clamp01(alpha));

            propBlock.SetColor("_Color", colorWithAlpha);
            marbleRenderer.SetPropertyBlock(propBlock);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
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
        if (!gameObject.activeSelf)
        {
            return;
        }

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
    Life = -2,
    Tier = -1,
    Unknown = 0,
    Green = 1,
    Red = 2,
    Fire = 3,
    Angel = 4,
    Gold = 5,
    BigRed = 6,
    Bomb = 7,
    BlueFire = 8
}
