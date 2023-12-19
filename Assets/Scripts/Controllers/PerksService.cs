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
            Points = 2500,
            Category = PerkCategory.Special
        });
        perks.Add(new Perk
        {
            Id = PerkEnum.AngelMarble,
            Name = "2X Marble",
            Sprite = Resources.Load<Sprite>("Images/UI/Special/AngelMarble"),
            Points = 4000,
            Category = PerkCategory.Special
        });
        perks.Add(new Perk
        {
            Id = PerkEnum.Bomb,
            Name = "Red Bomb",
            Sprite = Resources.Load<Sprite>("Images/UI/Special/Bomb"),
            Points = 5000,
            Category = PerkCategory.Special
        });
        perks.Add(new Perk
        {
            Id = PerkEnum.StreakSaver,
            Name = "Streak Saver #1",
            Sprite = Resources.Load<Sprite>("Images/UI/Special/StreakSaver"),
            Points = 15000,
            Category = PerkCategory.Special
        });
        perks.Add(new Perk
        {
            Id = PerkEnum.ExtraLife2,
            Name = "Extra Life #2",
            Sprite = Resources.Load<Sprite>("Images/UI/Special/ExtraLife"),
            Points = 25000,
            Category = PerkCategory.Special
        });
        perks.Add(new Perk
        {
            Id = PerkEnum.GoldMarble,
            Name = "5X Marble",
            Sprite = Resources.Load<Sprite>("Images/UI/Special/GoldMarble"),
            Points = 40000,
            Category = PerkCategory.Special
        });
        perks.Add(new Perk
        {
            Id = PerkEnum.NoBigMarbles,
            Name = "No Big Red",
            Sprite = Resources.Load<Sprite>("Images/UI/Special/NoBigRed"),
            Points = 50000,
            Category = PerkCategory.Special
        });
        perks.Add(new Perk
        {
            Id = PerkEnum.StreakSaver2,
            Name = "Streak Saver #2",
            Sprite = Resources.Load<Sprite>("Images/UI/Special/StreakSaver"),
            Points = 75000,
            Category = PerkCategory.Special
        });
        perks.Add(new Perk
        {
            Id = PerkEnum.ExtraLife3,
            Name = "Extra Life #3",
            Sprite = Resources.Load<Sprite>("Images/UI/Special/ExtraLife"),
            Points = 100000,
            Category = PerkCategory.Special
        });
        perks.Add(new Perk
        {
            Id = PerkEnum.GoldTrophy,
            Name = "Gold Trophy",
            Sprite = Resources.Load<Sprite>("Images/UI/Special/GoldTrophy"),
            Points = 200000,
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
            Points = 1500,
            Category = PerkCategory.Music
        });
        perks.Add(new Perk
        {
            Id = PerkEnum.SimpleTune,
            Name = "Simple Tune",
            Sprite = Resources.Load<Sprite>("Images/UI/Music/SimpleTune"),
            Points = 10000,
            Category = PerkCategory.Music
        });
        perks.Add(new Perk
        {
            Id = PerkEnum.MarblesAnthem,
            Name = "Marbles Anthem",
            Sprite = Resources.Load<Sprite>("Images/UI/Music/MarblesAnthem"),
            Points = 20000,
            Category = PerkCategory.Music
        });
        perks.Add(new Perk
        {
            Id = PerkEnum.MarblesSong4,
            Name = "Lost Marbles",
            Sprite = Resources.Load<Sprite>("Images/UI/Music/LostMarbles"),
            Points = 45000,
            Category = PerkCategory.Music
        });
        perks.Add(new Perk
        {
            Id = PerkEnum.MarblesRefrain,
            Name = "Marbles Refrain",
            Sprite = Resources.Load<Sprite>("Images/UI/Music/MarblesRefrain"),
            Points = 60000,
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
        perks.Add(new Perk
        {
            Id = PerkEnum.StreamBackground,
            Name = "Magic Realm",
            Sprite = Resources.Load<Sprite>("Images/UI/Backgrounds/Stream"),
            Points = 250,
            Category = PerkCategory.Background
        });
        perks.Add(new Perk
        {
            Id = PerkEnum.CandyBackground,
            Name = "Candy Land",
            Sprite = Resources.Load<Sprite>("Images/UI/Backgrounds/Candy"),
            Points = 1000,
            Category = PerkCategory.Background
        });
        perks.Add(new Perk
        {
            Id = PerkEnum.SnowBackground,
            Name = "Snowy Inn",
            Sprite = Resources.Load<Sprite>("Images/UI/Backgrounds/Snow"),
            Points = 2000,
            Category = PerkCategory.Background
        });
        perks.Add(new Perk
        {
            Id = PerkEnum.RetroBackground,
            Name = "Retro Forest",
            Sprite = Resources.Load<Sprite>("Images/UI/Backgrounds/Retro"),
            Points = 11000,
            Category = PerkCategory.Background
        });
        perks.Add(new Perk
        {
            Id = PerkEnum.ToysBackground,
            Name = "Toy Room",
            Sprite = Resources.Load<Sprite>("Images/UI/Backgrounds/Toys"),
            Points = 17500,
            Category = PerkCategory.Background
        });
        perks.Add(new Perk
        {
            Id = PerkEnum.MedievalBackground,
            Name = "Medieval Town",
            Sprite = Resources.Load<Sprite>("Images/UI/Backgrounds/Medieval"),
            Points = 27500,
            Category = PerkCategory.Background
        });
        perks.Add(new Perk
        {
            Id = PerkEnum.TechLabBackground,
            Name = "Tech Lab",
            Sprite = Resources.Load<Sprite>("Images/UI/Backgrounds/TechLab"),
            Points = 35000,
            Category = PerkCategory.Background
        });
        perks.Add(new Perk
        {
            Id = PerkEnum.CaveBackground,
            Name = "Deep Cave",
            Sprite = Resources.Load<Sprite>("Images/UI/Backgrounds/Cave"),
            Points = 55000,
            Category = PerkCategory.Background
        });
        perks.Add(new Perk
        {
            Id = PerkEnum.SpaceBackground,
            Name = "Outer Space",
            Sprite = Resources.Load<Sprite>("Images/UI/Backgrounds/Space"),
            Points = 80000,
            Category = PerkCategory.Background
        });

        // Ramp
        perks.Add(new Perk
        {
            Id = PerkEnum.DefaultRamp,
            Name = "Main Ramp",
            Sprite = Resources.Load<Sprite>("Images/UI/Ramps/MainRamp"),
            Points = 0,
            Category = PerkCategory.Ramp
        });
        perks.Add(new Perk
        {
            Id = PerkEnum.GoldRamp,
            Name = "Gold Ramp",
            Sprite = Resources.Load<Sprite>("Images/UI/Ramps/GoldRamp"),
            Points = 500,
            Category = PerkCategory.Ramp
        });
        perks.Add(new Perk
        {
            Id = PerkEnum.IceRamp,
            Name = "Ice Ramp",
            Sprite = Resources.Load<Sprite>("Images/UI/Ramps/IceRamp"),
            Points = 3000,
            Category = PerkCategory.Ramp
        });
        perks.Add(new Perk
        {
            Id = PerkEnum.WoodRamp,
            Name = "Wood Ramp",
            Sprite = Resources.Load<Sprite>("Images/UI/Ramps/WoodRamp"),
            Points = 7500,
            Category = PerkCategory.Ramp
        });
        perks.Add(new Perk
        {
            Id = PerkEnum.ChocolateRamp,
            Name = "Chocolate Ramp",
            Sprite = Resources.Load<Sprite>("Images/UI/Ramps/ChocolateRamp"),
            Points = 12500,
            Category = PerkCategory.Ramp
        });
        perks.Add(new Perk
        {
            Id = PerkEnum.SciFiRamp,
            Name = "Sci-Fi Ramp",
            Sprite = Resources.Load<Sprite>("Images/UI/Ramps/SciFiRamp"),
            Points = 30000,
            Category = PerkCategory.Ramp
        });
        perks.Add(new Perk
        {
            Id = PerkEnum.RoadRamp,
            Name = "Road Ramp",
            Sprite = Resources.Load<Sprite>("Images/UI/Ramps/RoadRamp"),
            Points = 42500,
            Category = PerkCategory.Ramp
        });
        perks.Add(new Perk
        {
            Id = PerkEnum.GrassRamp,
            Name = "Grass Ramp",
            Sprite = Resources.Load<Sprite>("Images/UI/Ramps/GrassRamp"),
            Points = 70000,
            Category = PerkCategory.Ramp
        });
        perks.Add(new Perk
        {
            Id = PerkEnum.DarkRamp,
            Name = "Dark Ramp",
            Sprite = Resources.Load<Sprite>("Images/UI/Ramps/DarkRamp"),
            Points = 90000,
            Category = PerkCategory.Ramp
        });
        perks.Add(new Perk
        {
            Id = PerkEnum.RainbowRamp,
            Name = "Rainbow Ramp",
            Sprite = Resources.Load<Sprite>("Images/UI/Ramps/RainbowRamp"),
            Points = 110000,
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

    public (List<PerkCategory> categories, List<Perk> perks) GetUnlockedPerks()
    {
        var lastPointsByCategory = new Dictionary<PerkCategory, int>
        {
            { PerkCategory.Special, savedData.SelectedPerks.LastSpecialPoints },
            { PerkCategory.Music, savedData.SelectedPerks.LastMusicPoints },
            { PerkCategory.Background, savedData.SelectedPerks.LastBackgroundPoints },
            { PerkCategory.Ramp, savedData.SelectedPerks.LastRampPoints }
        };

        var unlockedCategories = new HashSet<PerkCategory>();
        var unlockedPerks = new List<Perk>();

        foreach (var perk in perks)
        {
            // If the perk's points are within the new range, add its category and the perk itself
            if (perk.Points > 0 && perk.Points > lastPointsByCategory[perk.Category] && perk.Points <= savedData.Points)
            {
                unlockedCategories.Add(perk.Category);
                unlockedPerks.Add(perk);
            }
        }

        return (categories: unlockedCategories.ToList(), perks: unlockedPerks);
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