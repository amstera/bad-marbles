using UnityEngine;

public enum PerkCategory
{
    Special, Background, Music, Ramp
}

public enum PerkEnum
{
    Unknown = 0,
    DefaultMusic = 1,
    DefaultBackground = 2,
    DefaultRamp = 3,
    ExtraLife1 = 4,
    RockinMarbles = 5,
    MarblesAnthem = 6,
    AngelMarble = 7,
    ExtraLife2 = 8,
    StreamBackground = 9,
    CandyBackground = 10,
    GoldRamp = 11,
    ToysBackground = 12,
    Bomb = 13,
    ChocolateRamp = 14,
    IceRamp = 15,
    RetroBackground = 16,
    WoodRamp = 17,
    SnowBackground = 18,
    CloudsBackground = 19,
    StreakSaver = 20,
    MarblesSong4 = 21,
    StreakSaver2 = 22,
    GoldMarble = 23,
    SciFiRamp = 24,
    TechLabBackground = 25,
    SimpleTune = 26,
    SlowTime = 27,
    ExtraLife3 = 28,
    RoadRamp = 29,
    GoldTrophy = 30,
    RainbowRamp = 31,
    SpaceBackground = 32,
    CaveBackground = 33,
    MarblesRefrain = 34,
    GrassRamp = 35,
    DarkRamp = 36,
    UnderwaterBackground = 37,
    MarbleBackground = 38,
    NoBombs = 39,
    ExtraChance = 40,
    GlassRamp = 41,
    BrickRamp = 42,
    ChillJazz = 43,
    AcousticRambles = 44
}

[System.Serializable]
public class Perk
{
    public PerkEnum Id;
    public string Name;
    public string Description;
    public Sprite Sprite;
    public int Points;
    public PerkCategory Category;
    public bool IsSelected;
    public bool IsUnlocked;
}