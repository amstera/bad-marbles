using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PerkService
{
    private static PerkService _instance;
    public static PerkService Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PerkService();
            }
            return _instance;
        }
    }

    public SaveObject savedData;

    private List<Perk> perks = new List<Perk>();

    public PerkService()
    {
        InitializePerks();
        savedData = SaveManager.Load();
    }

    private void InitializePerks()
    {
        // Special
        perks.Add(new Perk
        {
            Id = PerkEnum.ExtraLife1,
            Name = "Extra Life #1",
            Sprite = Resources.Load<Sprite>("Images/UI/Special/ExtraLife"),
            Points = 5000,
            Category = PerkCategory.Special
        });

        // Music
        perks.Add(new Perk
        {
            Id = PerkEnum.DefaultMusic,
            Name = "Main Theme",
            Sprite = Resources.Load<Sprite>("Images/UI/Music/MainTheme"),
            Points = 0,
            Category = PerkCategory.Music
        });
        perks.Add(new Perk
        {
            Id = PerkEnum.RockinMarbles,
            Name = "Rockin' Marbles",
            Sprite = Resources.Load<Sprite>("Images/UI/Music/RockinMarbles"),
            Points = 1000,
            Category = PerkCategory.Music
        });
        perks.Add(new Perk
        {
            Id = PerkEnum.MarblesAnthem,
            Name = "Marbles Anthem",
            Sprite = Resources.Load<Sprite>("Images/UI/Music/MarblesAnthem"),
            Points = 10000,
            Category = PerkCategory.Music
        });

        // Background
        perks.Add(new Perk
        {
            Id = PerkEnum.DefaultBackground,
            Name = "Main BG",
            Sprite = Resources.Load<Sprite>("Images/UI/Backgrounds/Sunset"),
            Points = 0,
            Category = PerkCategory.Background
        });

        // Ramp
        perks.Add(new Perk
        {
            Id = PerkEnum.DefaultRamp,
            Name = "Main Ramp",
            Sprite = null,
            Points = 0,
            Category = PerkCategory.Ramp
        });
    }

    public List<Perk> GetPerksByCategory(PerkCategory category)
    {
        List<Perk> filteredPerks = new List<Perk>();
        foreach (var perk in perks)
        {
            if (perk.Category == category)
            {
                perk.IsUnlocked = savedData.Points >= perk.Points;
                perk.IsSelected = DetermineIfPerkIsSelected(perk);
                filteredPerks.Add(perk);
            }
        }
        return filteredPerks.OrderBy(f => f.Points).ToList();
    }

    private bool DetermineIfPerkIsSelected(Perk perk)
    {
        if (perk.Category == PerkCategory.Special)
        {
            return savedData.SelectedPerks.SelectedSpecial.Contains(perk.Id);
        }

        if (perk.Category == PerkCategory.Music)
        {
            return savedData.SelectedPerks.SelectedMusic == perk.Id;
        }

        if (perk.Category == PerkCategory.Background)
        {
            return savedData.SelectedPerks.SelectedBackground == perk.Id;
        }

        if (perk.Category == PerkCategory.Ramp)
        {
            return savedData.SelectedPerks.SelectedRamp == perk.Id;
        }

        return false;
    }
}