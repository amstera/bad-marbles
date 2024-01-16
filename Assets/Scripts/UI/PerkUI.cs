using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UI.ProceduralImage;
using UnityEngine.EventSystems;
using JoshH.UI;
using System;

public class PerkUI : MonoBehaviour, IPointerClickHandler
{
    public PerkEnum id;
    public string perkName;
    public string description;
    public Sprite perkSprite;
    public int pointsRequired;
    public int totalPoints;
    public bool isSelected;
    public bool isUnlocked;
    public bool isNewIndicatorEnabled;
    public PerkCategory category;

    public TextMeshProUGUI nameText;
    public ProceduralImage perkImage;
    public Image pillCapsuleImage;
    public TextMeshProUGUI pillText;
    public ProceduralImage perkOutline, lockProgress;
    public UIGradient ForegroundGradient;
    public GameObject newIndicator, lockIcon;
    public Shadow outlineShadow;

    public event Action<PerkUI> OnPerkClicked;

    private float unlockedAlpha = 1.0f;
    private float lockedAlpha = 0.4f;
    private int selectedBorderWidth = 6;
    private int defaultBorderWidth = 3;

    private readonly Color selectedBorderColor = new Color32(253, 255, 93, 255);
    private readonly Color defaultBorderColor = Color.gray;
    private readonly Color unlockedColor = new Color32(73, 190, 71, 255);
    private readonly Color lockedColor = new Color32(210, 70, 50, 255);
    private readonly Color gradientUnlockedColor1 = new Color32(7, 235, 197, 255);
    private readonly Color gradientUnlockedColor2 = new Color32(0, 157, 228, 255);
    private readonly Color gradientLockedColor1 = new Color32(202, 202, 202, 255);
    private readonly Color gradientLockedColor2 = new Color32(99, 99, 99, 255);
    private readonly Color gradientSelectedColor1 = new Color32(225, 231, 41, 255);
    private readonly Color gradientSelectedColor2 = new Color32(178, 139, 4, 255);

    public void InitializePerk(PerkEnum id, string name, string description, Sprite sprite, int points, int totalPoints, PerkCategory category, bool isSelected, bool isUnlocked, bool isNewIndicatorEnabled)
    {
        this.id = id;
        perkName = name;
        perkSprite = sprite;
        pointsRequired = points;
        this.totalPoints = totalPoints;
        this.description = description;
        this.category = category;
        this.isSelected = isSelected;
        this.isUnlocked = isUnlocked;
        this.isNewIndicatorEnabled = isNewIndicatorEnabled;
        UpdatePerkDisplay();
    }

    void UpdatePerkDisplay()
    {
        nameText.text = perkName.ToUpper();
        perkImage.sprite = perkSprite;
        newIndicator.SetActive(isNewIndicatorEnabled);

        SetPerkState();
        SetPerkSelected(isSelected);
    }

    private void SetPerkState()
    {
        if (isUnlocked)
        {
            pillCapsuleImage.color = unlockedColor;
            pillText.text = "UNLOCKED";
            SetAlpha(unlockedAlpha);
        }
        else
        {
            pillCapsuleImage.color = lockedColor;
            pillText.text = FormatPoints(pointsRequired);
            SetAlpha(lockedAlpha);
            SetGradientColors(gradientLockedColor1, gradientLockedColor2);
            lockIcon.SetActive(true);
            lockProgress.gameObject.SetActive(true);
            lockProgress.fillAmount = (float)totalPoints / pointsRequired;
        }
    }

    private void SetPerkSelected(bool selected)
    {
        if (isUnlocked)
        {
            ApplyPerkSelectionState(selected);
            outlineShadow.enabled = selected;
        }
    }

    private void ApplyPerkSelectionState(bool selected)
    {
        perkOutline.BorderWidth = selected ? selectedBorderWidth : defaultBorderWidth;
        perkOutline.color = selected ? selectedBorderColor : defaultBorderColor;
        SetGradientColors(selected ? gradientSelectedColor1 : gradientUnlockedColor1,
                          selected ? gradientSelectedColor2 : gradientUnlockedColor2);
    }

    private void SetGradientColors(Color color1, Color color2)
    {
        ForegroundGradient.LinearColor1 = color1;
        ForegroundGradient.LinearColor2 = color2;
    }

    private string FormatPoints(int points)
    {
        return points < 100000 ? $"{points} PTS" : $"{(int)Math.Round(points / 1000.0) * 1000 / 1000}K PTS";
    }

    void SetAlpha(float alpha)
    {
        Color color = perkImage.color;
        color.a = alpha;
        perkImage.color = color;

        color = nameText.color;
        color.a = alpha;
        nameText.color = color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isUnlocked)
        {
            OnPerkClicked?.Invoke(this);
        }
    }

    public void SetSelected(bool isSelected)
    {
        this.isSelected = isSelected;
        SetPerkSelected(isSelected);
    }

    public void UnlockPerk()
    {
        isUnlocked = true;
        UpdatePerkDisplay();
    }
}