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
    public AudioSource pageFlipSound;

    public Button backButton;
    public Button perksButton, musicButton, backgroundButton, rampButton;
    public GameObject perksViewedIndicator, musicViewedIndicator, backgroundViewedIndicator, rampViewedIndicator;
    public GameObject perksShineIndicator, musicShineIndicator, backgroundShineIndicator, rampShineIndicator;
    public GameObject arrow;
    public RectTransform selectedIndicator;
    public CanvasGroup scrollViewCanvasGroup;

    public PerkUI perkPrefab;
    public NextPerkUI nextPerkPrefab;
    public ScrollRect scrollView;
    public RectTransform scrollViewRect;
    public RectTransform scrollViewContent;
    public PerkPopUpUI perkPopUp;

    private float indicatorMoveSpeed = 0.1f;
    private SaveObject savedData;
    private Button lastPressedButton;
    private List<PerkUI> currentPerks = new List<PerkUI>();
    private List<Perk> unlockedPerks;
    private Dictionary<PerkCategory, Perk> nextPerks;
    private NextPerkUI nextPerkInstance;

    private Coroutine updateScrollViewCoroutine;
    private float fadeDuration = 0.075f;

    // Grid layout settings
    private Vector2 initialPosition = new Vector2(-105, -140);
    private float xOffset = 209f;
    private float yOffset = -244f;
    private float nextPerkHeight = 80f;
    private int itemsPerRow = 2;

    void Start()
    {
        savedData = SaveManager.Load();
        totalPointsText.text = savedData.Points == 1 ? "1 POINT" : $"{savedData.Points} POINTS";

        var unlockedData = PerkService.Instance.GetUnlockedPerks();
        var unlockedCategories = unlockedData.categories;
        unlockedPerks = unlockedData.perks;
        nextPerks = PerkService.Instance.GetNextPerksForAllCategories();

        SetUpButton(perksButton, PerkCategory.Special, perksViewedIndicator, perksShineIndicator, unlockedCategories);
        SetUpButton(musicButton, PerkCategory.Music, musicViewedIndicator, musicShineIndicator, unlockedCategories);
        SetUpButton(backgroundButton, PerkCategory.Background, backgroundViewedIndicator, backgroundShineIndicator, unlockedCategories);
        SetUpButton(rampButton, PerkCategory.Ramp, rampViewedIndicator, rampShineIndicator, unlockedCategories);

        lastPressedButton = perksButton;

        if (!savedData.HasSeenPerksPopup)
        {
            perkPopUp.Show("Rewards", "Every game played earns points to unlock new rewards!", perksButton.image.sprite);
            savedData.HasSeenPerksPopup = true;

            SaveManager.Save(savedData);
        }

        if (!savedData.HasSeenPerksTutorial && unlockedCategories.Contains(PerkCategory.Ramp))
        {
            OnButtonClicked(rampButton, rampViewedIndicator, rampShineIndicator);
            arrow.SetActive(true);
            StartCoroutine(SwayArrowCoroutine());
        }
        else
        {
            PopulateScrollViewContent(PerkCategory.Special);
            UpdateLastViewedIndicator(PerkCategory.Special, null, null);
        }
    }

    public void ScrollTo(PerkEnum perkId)
    {
        PerkUI targetPerk = currentPerks.Find(perk => perk.id == perkId);

        if (targetPerk == null)
        {
            Debug.LogWarning($"Perk with ID {perkId} could not be found.");
            return;
        }

        // Get the Y position of the targetPerk in the scroll view content
        float perkPositionY = targetPerk.GetComponent<RectTransform>().anchoredPosition.y;

        // Calculate the normalized position to scroll to
        float contentHeight = scrollViewContent.rect.height;
        float viewportHeight = scrollView.GetComponent<RectTransform>().rect.height;
        float scrollHeight = contentHeight - viewportHeight;
        float padding = 125f;
        float normalizedPosition = (scrollHeight + perkPositionY + padding) / scrollHeight;

        // Clamp the value between 0 and 1
        normalizedPosition = Mathf.Clamp01(normalizedPosition);

        // Start the coroutine to scroll
        StartCoroutine(ScrollToPosition(normalizedPosition, 0.2f));
    }

    private IEnumerator ScrollToPosition(float targetPosition, float duration)
    {
        float time = 0;
        float startPosition = scrollView.verticalNormalizedPosition;

        while (time < duration)
        {
            time += Time.deltaTime;
            scrollView.verticalNormalizedPosition = Mathf.Lerp(startPosition, targetPosition, time / duration);
            yield return null;
        }

        // Ensure the final position is set
        scrollView.verticalNormalizedPosition = targetPosition;
    }

    private void SetUpButton(Button button, PerkCategory category, GameObject indicator, GameObject shine, List<PerkCategory> unlockedCategories)
    {
        button.onClick.AddListener(() => OnButtonClicked(button, indicator, shine));

        if (indicator != null)
        {
            var hasNewPerks = unlockedCategories.Contains(category);
            indicator.SetActive(hasNewPerks);
            if (shine != null)
            {
                shine.SetActive(hasNewPerks);
            }
        }
    }

    private void PopulateScrollViewContent(PerkCategory category)
    {
        ClearScrollViewContent();

        var perks = PerkService.Instance.GetPerksByCategory(category);
        Vector2 currentPosition = initialPosition;

        // Find the newly unlocked perk for this category
        Perk justUnlockedPerk = unlockedPerks.Find(u => u.Category == category);

        // Determine whether to show the next perk or the unlocked perk
        Perk perkToShow = justUnlockedPerk ?? nextPerks.GetValueOrDefault(category);
        if (perkToShow != null)
        {
            bool isJustUnlocked = justUnlockedPerk != null;

            nextPerkInstance = Instantiate(nextPerkPrefab, scrollViewContent);
            nextPerkInstance.UpdatePerkInfo(perkToShow, this, savedData.Points, isJustUnlocked);
            RectTransform nextPerkRectTransform = nextPerkInstance.GetComponent<RectTransform>();
            nextPerkRectTransform.anchoredPosition = new Vector3(0, -nextPerkHeight / 2 - 10, 0);

            currentPosition.y -= nextPerkHeight;
        }

        int counter = 0;
        foreach (Perk perk in perks)
        {
            PerkUI perkObject = Instantiate(perkPrefab, scrollViewContent);
            RectTransform perkRectTransform = perkObject.GetComponent<RectTransform>();
            perkRectTransform.anchoredPosition = currentPosition;

            perkObject.transform.SetSiblingIndex(0); // Position perks below the next perk

            // Initialize perk data
            perkObject.InitializePerk(perk.Id, perk.Name, perk.Description, perk.Sprite, perk.Points, savedData.Points, perk.Category, perk.IsSelected, perk.IsUnlocked, unlockedPerks.Contains(perk));

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

        AdjustScrollViewHeight(counter);

        scrollView.verticalNormalizedPosition = 1;
    }

    private void AdjustScrollViewHeight(int itemCount)
    {
        int rows = Mathf.CeilToInt((float)itemCount / itemsPerRow);

        float topPadding = 50;
        if (nextPerkInstance != null)
        {
            topPadding += nextPerkHeight;
        }

        // Calculated height = height of all items + spacing between them + top padding
        float calculatedHeight = (rows * -yOffset) + (rows - 1) + topPadding;

        float scrollViewHeight = scrollViewRect.rect.height;

        float additionalHeight = calculatedHeight > scrollViewHeight ? calculatedHeight - scrollViewHeight : 0;
        scrollViewContent.sizeDelta = new Vector2(scrollViewContent.sizeDelta.x, additionalHeight);
    }

    private void ClearScrollViewContent()
    {
        if (nextPerkInstance != null)
        {
            Destroy(nextPerkInstance.gameObject);
            nextPerkInstance = null;
        }

        foreach (var perk in currentPerks)
        {
            if (perk != null)
            {
                perk.OnPerkClicked -= HandlePerkClick;
                Destroy(perk.gameObject);
            }
        }
        currentPerks.Clear();
    }

    private void OnButtonClicked(Button clickedButton, GameObject indicator, GameObject shine)
    {
        if (lastPressedButton == clickedButton)
        {
            return;
        }

        pageFlipSound?.Play();

        lastPressedButton.interactable = true;
        lastPressedButton = clickedButton;
        lastPressedButton.interactable = false;

        MoveIndicatorToButton(clickedButton);

        PerkCategory category = DetermineCategoryFromButton(clickedButton);
        UpdateLastViewedIndicator(DetermineCategoryFromButton(clickedButton), indicator, shine);

        if (updateScrollViewCoroutine != null)
        {
            StopCoroutine(updateScrollViewCoroutine);
        }
        updateScrollViewCoroutine = StartCoroutine(UpdateScrollViewContent(category));
    }


    private void MoveIndicatorToButton(Button button)
    {
        // Move the indicator to the button's position
        Vector2 buttonPosition = button.GetComponent<RectTransform>().anchoredPosition;
        Vector2 newPosition = new Vector2(buttonPosition.x, selectedIndicator.anchoredPosition.y);
        StartCoroutine(SmoothMove(selectedIndicator, newPosition, indicatorMoveSpeed));
    }

    private void UpdateLastViewedIndicator(PerkCategory category, GameObject indicator, GameObject shine)
    {
        if (indicator != null)
        {
            indicator.SetActive(false);
        }
        if (shine != null)
        {
            shine.SetActive(false);
        }

        bool shouldSave = false;

        switch (category)
        {
            case PerkCategory.Special:
                shouldSave = UpdateCategoryPoints(ref savedData.SelectedPerks.LastSpecialPoints, savedData.Points);
                break;
            case PerkCategory.Background:
                shouldSave = UpdateCategoryPoints(ref savedData.SelectedPerks.LastBackgroundPoints, savedData.Points);
                break;
            case PerkCategory.Music:
                shouldSave = UpdateCategoryPoints(ref savedData.SelectedPerks.LastMusicPoints, savedData.Points);
                break;
            case PerkCategory.Ramp:
                shouldSave = UpdateCategoryPoints(ref savedData.SelectedPerks.LastRampPoints, savedData.Points);
                break;
        }

        if (shouldSave)
        {
            SaveManager.Save(savedData);
        }
    }

    private bool UpdateCategoryPoints(ref int lastPoints, int currentPoints)
    {
        if (lastPoints != currentPoints)
        {
            lastPoints = currentPoints;
            return true;
        }
        return false;
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
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeDuration)
        {
            scrollViewCanvasGroup.alpha = 1.0f - t;
            yield return null;
        }

        PopulateScrollViewContent(category);

        // Fade in
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeDuration)
        {
            scrollViewCanvasGroup.alpha = t;
            yield return null;
        }
        scrollViewCanvasGroup.alpha = 1.0f;
    }

    private void HandlePerkClick(PerkUI clickedPerk)
    {
        if (clickedPerk.category == PerkCategory.Special)
        {
            clickedPerk.SetSelected(!clickedPerk.isSelected);
            if (clickedPerk.isSelected)
            {
                savedData.SelectedPerks.SelectedSpecial.Add(clickedPerk.id);
                UpdateLastViewedIndicator(PerkCategory.Special, perksViewedIndicator, perksShineIndicator);
                if (!string.IsNullOrEmpty(clickedPerk.description) && !savedData.SeenDescription.Contains(clickedPerk.id))
                {
                    perkPopUp.Show(clickedPerk.perkName, clickedPerk.description, clickedPerk.perkImage.sprite);
                    savedData.SeenDescription.Add(clickedPerk.id);
                }
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
                PlayTrack();
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

        if (!savedData.HasSeenPerksTutorial && clickedPerk.isSelected)
        {
            arrow.SetActive(false);
            savedData.HasSeenPerksTutorial = true;
        }

        SaveManager.Save(savedData);
        PerkService.Instance.savedData = savedData;

        if (clickedPerk.isSelected)
        {
            plopSound?.Play();
        }
    }

    public void BackButtonAreaPressed()
    {
        StartCoroutine(BackButtonPressed());
    }

    private IEnumerator BackButtonPressed()
    {
        backButton.image.color = new Color(0.8f, 0.8f, 0.8f, 1);

        yield return new WaitForSeconds(0.1f);

        backButton.image.color = Color.white;

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

    private void PlayTrack()
    {
        var bgMusicInfo = MusicService.GetTrack(savedData);
        MenuMusicPlayer.Instance.backgroundMusic.clip = bgMusicInfo.clip;
        MenuMusicPlayer.Instance.backgroundMusic.volume = bgMusicInfo.volume;
        MenuMusicPlayer.Instance.backgroundMusic.Play();
    }

    private IEnumerator SwayArrowCoroutine()
    {
        float swaySpeed = 2;

        while (arrow.activeSelf)
        {
            float swayPosition = Mathf.Lerp(-32f, -23f, (Mathf.Sin(Time.time * swaySpeed * 2) + 1) / 2);
            arrow.transform.localPosition = new Vector3(swayPosition, arrow.transform.localPosition.y, arrow.transform.localPosition.z);
            yield return null;
        }
    }
}