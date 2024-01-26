using JoshH.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NextPerkUI : MonoBehaviour, IPointerClickHandler
{
    public TextMeshProUGUI text;
    public Image image, lockIcon;
    public UIGradient gradient;
    public GameObject newIndicator, shine;

    public Color linearColor1Unlocked;
    public Color linearColor2Unlocked;
    public Color linearColor1Next;
    public Color linearColor2Next;

    private Perk currentPerk;
    private PerksManager perksManager;

    public void UpdatePerkInfo(Perk perk, PerksManager manager, int currentPoints, bool isUnlocked)
    {
        if (perk == null)
        {
            text.gameObject.SetActive(false);
            image.gameObject.SetActive(false);
            return;
        }

        currentPerk = perk;
        perksManager = manager;

        gradient.LinearColor1 = isUnlocked ? linearColor1Unlocked : linearColor1Next;
        gradient.LinearColor2 = isUnlocked ? linearColor2Unlocked : linearColor2Next;

        string perkText;
        if (isUnlocked)
        {
            perkText = $"You unlocked\n<color=yellow>{perk.Name}</color>!";
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
            lockIcon.gameObject.SetActive(false);
            newIndicator.SetActive(true);
            shine.SetActive(true);
        }
        else
        {
            int pointsAway = perk.Points - currentPoints;
            string pointsText = FormatPoints(pointsAway) + (pointsAway == 1 ? " point" : " points");
            perkText = $"<color=#41EF3E>{pointsText}</color>\nuntil <color=yellow>{perk.Name}</color>!";
        }

        text.text = perkText;
        image.sprite = perk.Sprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentPerk != null && perksManager != null)
        {
            perksManager.ScrollTo(currentPerk.Id);
        }
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