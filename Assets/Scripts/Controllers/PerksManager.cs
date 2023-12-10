using System.Collections;
using System.Collections.Generic;
using EasyTransition;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PerksManager : MonoBehaviour
{
    public TextMeshProUGUI totalPointsText;
    public TransitionSettings transition;

    public AudioSource plopSound;

    public Button perksButton, musicButton, backgroundButton, rampButton;
    public RectTransform selectedIndicator;
    public CanvasGroup scrollViewCanvasGroup;

    public PerkUI perkPrefab;
    public Transform scrollViewContent;

    private float indicatorMoveSpeed = 0.15f;
    private SaveObject savedData;
    private Button lastPressedButton;
    private List<PerkUI> currentPerks = new List<PerkUI>();

    // Grid layout settings
    private Vector2 initialPosition = new Vector2(-105, -155);
    private float xOffset = 209;
    private float yOffset = -244;
    private int itemsPerRow = 2;

    void Start()
    {
        savedData = SaveManager.Load();
        totalPointsText.text = savedData.Points == 1 ? "1 POINT" : $"{savedData.Points} POINTS";

        // Add button click listeners
        perksButton.onClick.AddListener(() => OnButtonClicked(perksButton));
        musicButton.onClick.AddListener(() => OnButtonClicked(musicButton));
        backgroundButton.onClick.AddListener(() => OnButtonClicked(backgroundButton));
        rampButton.onClick.AddListener(() => OnButtonClicked(rampButton));

        lastPressedButton = perksButton;

        PopulateScrollViewContent(PerkCategory.Special);
    }

    private void PopulateScrollViewContent(PerkCategory category)
    {
        ClearScrollViewContent();

        var perks = PerkService.Instance.GetPerksByCategory(category);
        Vector2 currentPosition = initialPosition;
        int counter = 0;

        foreach (Perk perk in perks)
        {
            PerkUI perkObject = Instantiate(perkPrefab, scrollViewContent);
            RectTransform perkRectTransform = perkObject.GetComponent<RectTransform>();
            perkRectTransform.anchoredPosition = currentPosition;

            // Initialize perk data
            perkObject.InitializePerk(perk.Id, perk.Name, perk.Sprite, perk.Points, perk.Category, perk.IsSelected, perk.IsUnlocked);

            perkObject.OnPerkClicked += HandlePerkClick;
            currentPerks.Add(perkObject);

            // Update position for next perk
            if (++counter % itemsPerRow == 0)
            {
                currentPosition.x = initialPosition.x;
                currentPosition.y += yOffset;
            }
            else
            {
                currentPosition.x += xOffset;
            }
        }
    }

    private void ClearScrollViewContent()
    {
        foreach (var perk in currentPerks)
        {
            if (perk != null)
            {
                perk.OnPerkClicked -= HandlePerkClick;
            }
            Destroy(perk.gameObject);
        }
        currentPerks.Clear();
    }

    private void OnButtonClicked(Button clickedButton)
    {
        if (lastPressedButton == clickedButton)
        {
            return;
        }

        lastPressedButton.interactable = true;
        lastPressedButton = clickedButton;
        lastPressedButton.interactable = false;

        MoveIndicatorToButton(clickedButton);
        PerkCategory category = DetermineCategoryFromButton(clickedButton);
        StartCoroutine(UpdateScrollViewContent(category));
    }


    private void MoveIndicatorToButton(Button button)
    {
        // Move the indicator to the button's position
        Vector2 buttonPosition = button.GetComponent<RectTransform>().anchoredPosition;
        Vector2 newPosition = new Vector2(buttonPosition.x, selectedIndicator.anchoredPosition.y);
        StartCoroutine(SmoothMove(selectedIndicator, newPosition, indicatorMoveSpeed));
    }

    private IEnumerator SmoothMove(RectTransform rectTransform, Vector2 targetPosition, float speed)
    {
        float time = 0;
        Vector2 startPosition = rectTransform.anchoredPosition;

        while (time < 1)
        {
            time += Time.deltaTime / speed;
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, time);
            yield return null;
        }
    }

    private IEnumerator UpdateScrollViewContent(PerkCategory category)
    {
        // Fade out
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 0.1f)
        {
            scrollViewCanvasGroup.alpha = 1.0f - t;
            yield return null;
        }

        PopulateScrollViewContent(category);

        // Fade in
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 0.1f)
        {
            scrollViewCanvasGroup.alpha = t;
            yield return null;
        }
    }

    private void HandlePerkClick(PerkUI clickedPerk)
    {
        if (clickedPerk.category == PerkCategory.Special)
        {
            clickedPerk.SetSelected(!clickedPerk.isSelected);
            if (clickedPerk.isSelected)
            {
                savedData.SelectedPerks.SelectedSpecial.Add(clickedPerk.id);
            }
            else
            {
                savedData.SelectedPerks.SelectedSpecial.Remove(clickedPerk.id);
            }
        }
        else
        {
            if (savedData.SelectedPerks.SelectedMusic == clickedPerk.id ||
                savedData.SelectedPerks.SelectedBackground == clickedPerk.id ||
                savedData.SelectedPerks.SelectedRamp == clickedPerk.id)
            {
                return;
            }

            foreach (var perk in currentPerks)
            {
                if (perk != clickedPerk && perk.category == clickedPerk.category)
                {
                    perk.SetSelected(false);
                }
            }

            clickedPerk.SetSelected(true);

            if (clickedPerk.category == PerkCategory.Music)
            {
                savedData.SelectedPerks.SelectedMusic = clickedPerk.id;
            }
            else if (clickedPerk.category == PerkCategory.Background)
            {
                savedData.SelectedPerks.SelectedBackground = clickedPerk.id;
            }
            else if (clickedPerk.category == PerkCategory.Ramp)
            {
                savedData.SelectedPerks.SelectedRamp = clickedPerk.id;
            }
        }

        SaveManager.Save(savedData);
        PerkService.Instance.savedData = savedData;

        if (clickedPerk.isSelected)
        {
            plopSound?.Play();
        }
    }

    public void LoadMenu()
    {
        TransitionManager.Instance().Transition("Menu", transition, 0);
        plopSound?.Play();
    }

    private PerkCategory DetermineCategoryFromButton(Button button)
    {
        if (button == musicButton)
            return PerkCategory.Music;
        if (button == backgroundButton)
            return PerkCategory.Background;
        if (button == rampButton)
            return PerkCategory.Ramp;

        return PerkCategory.Special;
    }
}