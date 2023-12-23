using JoshH.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NextPerkUI : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Image image;
    public UIGradient gradient;

    public Color linearColor1Unlocked;
    public Color linearColor2Unlocked;
    public Color linearColor1Next;
    public Color linearColor2Next;

    public void UpdatePerkInfo(Perk perk, int currentPoints, bool isUnlocked)
    {
        if (perk == null)
        {
            text.gameObject.SetActive(false);
            image.gameObject.SetActive(false);
            return;
        }

        gradient.LinearColor1 = isUnlocked ? linearColor1Unlocked : linearColor1Next;
        gradient.LinearColor2 = isUnlocked ? linearColor2Unlocked : linearColor2Next;

        string perkText;
        if (isUnlocked)
        {
            perkText = $"<color=yellow>Awesome!</color> You unlocked the \"{perk.Name}\" perk!";
        }
        else
        {
            int pointsAway = perk.Points - currentPoints;
            string pointsText = FormatPoints(pointsAway) + (pointsAway == 1 ? " point" : " points");
            perkText = $"<color=yellow>{pointsText}</color> until \"{perk.Name}\" perk!";
        }

        text.text = perkText;
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