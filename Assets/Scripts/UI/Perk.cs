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
    ForestBackground = 10
}

[System.Serializable]
public class Perk
{
    public PerkEnum Id;
    public string Name;
    public Sprite Sprite;
    public int Points;
    public PerkCategory Category;
    public bool IsSelected;
    public bool IsUnlocked;
}