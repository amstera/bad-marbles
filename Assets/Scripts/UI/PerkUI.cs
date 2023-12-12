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
    public Sprite perkSprite;
    public int pointsRequired;
    public bool isSelected;
    public bool isUnlocked;
    public PerkCategory category;

    public TextMeshProUGUI nameText;
    public ProceduralImage perkImage;
    public Image pillCapsuleImage;
    public TextMeshProUGUI pillText;
    public ProceduralImage perkOutline;
    public ProceduralImage ForegroundImage;
    public UIGradient ForegroundGradient;

    public event Action<PerkUI> OnPerkClicked;

    private const float unlockedAlpha = 1.0f;
    private const float lockedAlpha = 0.4f;
    private const int selectedBorderWidth = 6;
    private const int defaultBorderWidth = 3;
    private Color selectedBorderColor = new Color32(249, 255, 0, 255); // Yellow
    private Color defaultBorderColor = Color.gray;
    private Color unlockedColor = new Color32(73, 190, 71, 255); // Custom green
    public Color lockedGray = new Color32(140, 140, 140, 255); // Inactive gray
    public Color inactiveGray = new Color32(176, 176, 176, 255); // Inactive gray

    void Start()
    {
        UpdatePerkDisplay();
    }

    public void InitializePerk(PerkEnum id, string name, Sprite sprite, int points, PerkCategory category, bool isSelected, bool isUnlocked)
    {
        this.id = id;
        perkName = name;
        perkSprite = sprite;
        pointsRequired = points;
        this.category = category;
        this.isSelected = isSelected;
        this.isUnlocked = isUnlocked;
        UpdatePerkDisplay();
    }

    void UpdatePerkDisplay()
    {
        nameText.text = perkName.ToUpper();
        perkImage.sprite = perkSprite;

        if (isUnlocked)
        {
            pillCapsuleImage.color = unlockedColor;
            pillText.text = "UNLOCKED";
            SetAlpha(unlockedAlpha);
        }
        else
        {
            pillCapsuleImage.color = lockedGray;
            pillText.text = $"{pointsRequired} PTS";
            SetAlpha(lockedAlpha);
            ForegroundGradient.enabled = false;
            ForegroundImage.color = inactiveGray;
        }

        if (isSelected)
        {
            perkOutline.BorderWidth = selectedBorderWidth;
            perkOutline.color = selectedBorderColor;
            ForegroundGradient.enabled = false;
        }
        else
        {
            perkOutline.BorderWidth = defaultBorderWidth;
            perkOutline.color = defaultBorderColor;
            ForegroundGradient.enabled = isUnlocked;
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