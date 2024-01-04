using UnityEngine;
using TMPro;

public class Tier : MonoBehaviour
{
    public Marble marble;
    public TextMeshPro text;
    public AudioSource tierAS;

    private Color gradientStartGreen = new Color(0x34 / 255f, 0xFF / 255f, 0x00 / 255f); // Light Green
    private Color gradientEndGreen = new Color(0x0E / 255f, 0x4D / 255f, 0x00 / 255f); // Dark Green
    private Color gradientStartOrangeRed = new Color(0xFF / 255f, 0x71 / 255f, 0x00 / 255f); // Orange
    private Color gradientEndRed = new Color(0xFF / 255f, 0x12 / 255f, 0x0D / 255f); // Red

    // Update tier and color gradient
    public void UpdateTier(int tierNumber)
    {
        // Update text
        text.text = $"TIER {tierNumber}";

        // Normalize tier number for gradient calculation
        float normalizedTier = Mathf.Clamp((tierNumber - 1) / 11f, 0, 1);

        // Calculate and apply color gradient
        Color gradientStart = Color.Lerp(gradientStartGreen, gradientStartOrangeRed, normalizedTier);
        Color gradientEnd = Color.Lerp(gradientEndGreen, gradientEndRed, normalizedTier);
        VertexGradient gradient = new VertexGradient(gradientStart, gradientStart, gradientEnd, gradientEnd);
        text.colorGradient = gradient;

        tierAS.PlayDelayed(1f);
    }

    // Update marble speed
    public void UpdateSpeed(float speed)
    {
        marble.speed = speed;
    }
}