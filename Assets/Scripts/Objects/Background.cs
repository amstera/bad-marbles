using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    private Dictionary<PerkEnum, string> backgroundPaths;
    private Dictionary<PerkEnum, Vector3> backgroundPositions;
    private Dictionary<PerkEnum, Vector3> tabletBackgroundPositions;

    public SpriteRenderer spriteRenderer;

    private SaveObject savedData;
    private string fileLocation = "Images/Backgrounds/Game";

    private void Awake()
    {
        backgroundPaths = new Dictionary<PerkEnum, string>
        {
            { PerkEnum.DefaultBackground, $"{fileLocation}/Sunset" },
            { PerkEnum.StreamBackground, $"{fileLocation}/Stream" },
            { PerkEnum.CandyBackground, $"{fileLocation}/Candy" },
            { PerkEnum.ToysBackground, $"{fileLocation}/Toys" },
            { PerkEnum.RetroBackground, $"{fileLocation}/Retro" },
            { PerkEnum.SnowBackground, $"{fileLocation}/Snow" },
            { PerkEnum.CloudsBackground, $"{fileLocation}/Clouds" },
            { PerkEnum.TechLabBackground, $"{fileLocation}/TechLab" },
            { PerkEnum.CaveBackground, $"{fileLocation}/Cave" },
            { PerkEnum.UnderwaterBackground, $"{fileLocation}/Underwater" },
            { PerkEnum.SpaceBackground, $"{fileLocation}/Space" },
            { PerkEnum.TheaterBackground, $"{fileLocation}/Theater" },
            { PerkEnum.MarbleBackground, $"{fileLocation}/Marbleville" },
            { PerkEnum.StadiumBackground, $"{fileLocation}/Stadium" }
        };

        backgroundPositions = new Dictionary<PerkEnum, Vector3>
        {
            { PerkEnum.DefaultBackground, new Vector3(-0.3f, 31f, 67f) },
            { PerkEnum.StreamBackground, new Vector3(0f, 18f, 51.5f) },
            { PerkEnum.CandyBackground, new Vector3(-5.2f, 30.5f, 101.4f) },
            { PerkEnum.ToysBackground, new Vector3(0, 29.5f, 80) },
            { PerkEnum.RetroBackground, new Vector3(0f, 26f, 94.6f) },
            { PerkEnum.SnowBackground, new Vector3(-35.1f, 18.1f, 87f) },
            { PerkEnum.CloudsBackground, new Vector3(5.9f, 25.5f, 88.8f) },
            { PerkEnum.TechLabBackground, new Vector3(-0.3f, 34.3f, 111.6f) },
            { PerkEnum.CaveBackground, new Vector3(9f, 35f, 169f) },
            { PerkEnum.UnderwaterBackground, new Vector3(-0.3f, 33.3f, 116.3f) },
            { PerkEnum.SpaceBackground, new Vector3(0f, 26f, 91f) },
            { PerkEnum.TheaterBackground, new Vector3(-1.2f, 34.2f, 99.1f) },
            { PerkEnum.MarbleBackground, new Vector3(10.2f, 25.5f, 85f) },
            { PerkEnum.StadiumBackground, new Vector3(0f, 28f, 111f) }
        };

        tabletBackgroundPositions = new Dictionary<PerkEnum, Vector3>
        {
            { PerkEnum.DefaultBackground, new Vector3(-0.3f, 18.2f, 38.5f) },
            { PerkEnum.StreamBackground, new Vector3(15.7f, 7.8f, 49.7f) },
            { PerkEnum.CandyBackground, new Vector3(-2.3f, 8.3f, 69.4f) },
            { PerkEnum.ToysBackground, new Vector3(0.4f, 3.5f, 70.6f) },
            { PerkEnum.RetroBackground, new Vector3(4f, 3.8f, 60.3f) },
            { PerkEnum.SnowBackground, new Vector3(-33.1f, 4.2f, 70f) },
            { PerkEnum.CloudsBackground, new Vector3(0f, 4.2f, 72.4f) },
            { PerkEnum.TechLabBackground, new Vector3(0f, 4.2f, 71.1f) },
            { PerkEnum.CaveBackground, new Vector3(9.4f, 18.2f, 117f) },
            { PerkEnum.UnderwaterBackground, new Vector3(-2.1f, 6.1f, 70.3f) },
            { PerkEnum.SpaceBackground, new Vector3(3f, 6f, 73f) },
            { PerkEnum.TheaterBackground, new Vector3(-1.9f, 5.2f, 65.8f) },
            { PerkEnum.MarbleBackground, new Vector3(-4.8f, 6.6f, 74.2f) },
            { PerkEnum.StadiumBackground, new Vector3(-0.5f, 10.1f, 74.2f) }
        };
    }

    private void Start()
    {
        savedData = SaveManager.Load();
        SetBackground(savedData.SelectedPerks.SelectedBackground);
    }

    private void SetBackground(PerkEnum selectedBackground)
    {
        if (backgroundPaths.TryGetValue(selectedBackground, out var backgroundPath))
        {
            var sprite = Resources.Load<Sprite>(backgroundPath);
            if (sprite != null)
            {
                spriteRenderer.sprite = sprite;
                spriteRenderer.transform.position = GetPositionForDevice(selectedBackground);
            }
            else
            {
                Debug.LogError($"Sprite not found for path: {backgroundPath}");
            }
        }
        else
        {
            Debug.LogError($"No background set for perk: {selectedBackground}");
        }
    }

    private Vector3 GetPositionForDevice(PerkEnum selectedBackground)
    {
        var useTabletCoordinates = DeviceTypeChecker.IsTablet();
        var positionDict = useTabletCoordinates ? tabletBackgroundPositions : backgroundPositions;

        if (positionDict.TryGetValue(selectedBackground, out var position))
        {
            return position;
        }
        else
        {
            Debug.LogError($"No position set for perk: {selectedBackground}");
            return Vector3.zero;
        }
    }
}