using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NextPerkUI : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Image image;

    public void UpdatePerkInfo(Perk perk, int currentPoints)
    {
        if (perk == null)
        {
            text.gameObject.SetActive(false);
            image.gameObject.SetActive(false);
            return;
        }

        int pointsAway = perk.Points - currentPoints;
        string pointsText = FormatPoints(pointsAway) + (pointsAway == 1 ? " point" : " points");
        string newPerkText = $"<color=yellow>{pointsText}</color> from \"{perk.Name}\" perk!";

        text.text = newPerkText;
        image.sprite = perk.Sprite;
    }

    private string FormatPoints(int points)
    {
        if (points < 1000)
        {
            return points.ToString();
        }
        else
        {
            return points >= 100000 ?
                $"{Mathf.Round(points / 1000f)}K" :
                $"{(Mathf.Round(points / 100f) / 10f)}K"; // Rounds to nearest 100 and divides by 10 for a single decimal place
        }
    }
}