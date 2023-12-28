using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UI.ProceduralImage;
using UnityEngine.EventSystems;
using System;
using JoshH.UI;

public class PerkUI : MonoBehaviour, IPointerClickHandler
{
    public PerkEnum id;
    public string perkName;
    public string description;
    public Sprite perkSprite;
    public int pointsRequired;
    public bool isSelected;
    public bool isUnlocked;
    public bool isNewIndicatorEnabled;
    public PerkCategory category;

    public TextMeshProUGUI nameText;
    public ProceduralImage perkImage;
    public Image pillCapsuleImage;
    public TextMeshProUGUI pillText;
    public ProceduralImage perkOutline;
    public UIGradient ForegroundGradient;
    public GameObject newIndicator;

    public event Action<PerkUI> OnPerkClicked;

    private float unlockedAlpha = 1.0f;
    private float lockedAlpha = 0.4f;
    private int selectedBorderWidth = 6;
    private int defaultBorderWidth = 3;

    private readonly Color selectedBorderColor = new Color32(249, 255, 0, 255);
    private readonly Color defaultBorderColor = Color.gray;

    private readonly Color unlockedColor = new Color32(73, 190, 71, 255);
    private readonly Color lockedColor = new Color32(203, 152, 7, 255);

    private readonly Color gradientUnlockedColor1 = new Color32(7, 235, 197, 255);
    private readonly Color gradientUnlockedColor2 = new Color32(0, 157, 228, 255);

    private readonly Color gradientLockedColor1 = new Color32(202, 202, 202, 255);
    private readonly Color gradientLockedColor2 = new Color32(99, 99, 99, 255);

    private readonly Color gradientSelectedColor1 = new Color32(225, 231, 41, 255);
    private readonly Color gradientSelectedColor2 = new Color32(178, 139, 4, 255);

    void Start()
    {
        UpdatePerkDisplay();
    }

    public void InitializePerk(PerkEnum id, string name, string description, Sprite sprite, int points, PerkCategory category, bool isSelected, bool isUnlocked, bool isNewIndicatorEnabled)
    {
        this.id = id;
        perkName = name;
        perkSprite = sprite;
        pointsRequired = points;
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
        }
    }

    private void SetPerkSelected(bool selected)
    {
        if (selected)
        {
            perkOutline.BorderWidth = selectedBorderWidth;
            perkOutline.color = selectedBorderColor;
            SetGradientColors(gradientSelectedColor1, gradientSelectedColor2);
        }
        else
        {
            perkOutline.BorderWidth = defaultBorderWidth;
            perkOutline.color = defaultBorderColor;
            if (isUnlocked)
            {
                SetGradientColors(gradientUnlockedColor1, gradientUnlockedColor2);
            }
        }
    }

    private void SetGradientColors(Color color1, Color color2)
    {
        ForegroundGradient.LinearColor1 = color1;
        ForegroundGradient.LinearColor2 = color2;
    }

    private string FormatPoints(int points)
    {
        if (points < 100000)
        {
            return $"{points} PTS";
        }
        else
        {
            int roundedPoints = (int)Math.Round(points / 1000.0) * 1000;
            return $"{roundedPoints / 1000}K PTS";
        }
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
        if (isUnlocked)
        {
            this.isSelected = isSelected;
            UpdatePerkDisplay();
        }
    }

    public void UnlockPerk()
    {
        isUnlocked = true;
        UpdatePerkDisplay();
    }
}