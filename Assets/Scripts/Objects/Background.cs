using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    private Dictionary<PerkEnum, (string path, Vector3 position)> backgrounds;

    public SpriteRenderer spriteRenderer;

    private SaveObject savedData;
    private string fileLocation = "Images/Backgrounds/Game";

    private void Awake()
    {
        backgrounds = new Dictionary<PerkEnum, (string, Vector3)>
        {
            { PerkEnum.DefaultBackground, ($"{fileLocation}/Sunset", new Vector3(-0.3f, 31f, 67f)) },
            { PerkEnum.StreamBackground, ($"{fileLocation}/Stream", new Vector3(0f, 18f, 51.5f)) },
            { PerkEnum.CandyBackground, ($"{fileLocation}/Candy", new Vector3(-5.2f, 30.5f, 101.4f)) },
            { PerkEnum.ToysBackground, ($"{fileLocation}/Toys", new Vector3(0, 29.5f, 80)) },
            { PerkEnum.RetroBackground, ($"{fileLocation}/Retro", new Vector3(0f, 30.9f, 94.6f)) },
            { PerkEnum.SnowBackground, ($"{fileLocation}/Snow", new Vector3(-35.1f, 18.1f, 87f)) },
            { PerkEnum.CloudsBackground, ($"{fileLocation}/Clouds", new Vector3(5.9f, 25.5f, 88.8f)) },
            { PerkEnum.TechLabBackground, ($"{fileLocation}/TechLab", new Vector3(-0.3f, 34.3f, 111.6f)) },
            { PerkEnum.CaveBackground, ($"{fileLocation}/Cave", new Vector3(9f, 35f, 169f)) },
            { PerkEnum.UnderwaterBackground, ($"{fileLocation}/Underwater", new Vector3(-0.3f, 33.3f, 116.3f)) },
            { PerkEnum.SpaceBackground, ($"{fileLocation}/Space", new Vector3(0f, 26f, 91f)) },
            { PerkEnum.TheaterBackground, ($"{fileLocation}/Theater", new Vector3(-1.2f, 34.2f, 99.1f)) },
            { PerkEnum.MarbleBackground, ($"{fileLocation}/Marbleville", new Vector3(10.2f, 25.5f, 85f)) },
            { PerkEnum.StadiumBackground, ($"{fileLocation}/Stadium", new Vector3(0f, 28f, 111f)) }
        };
    }

    private void Start()
    {
        savedData = SaveManager.Load();
        SetBackground(savedData.SelectedPerks.SelectedBackground);
    }

    private void SetBackground(PerkEnum selectedBackground)
    {
        if (backgrounds.TryGetValue(selectedBackground, out var backgroundInfo))
        {
            var sprite = Resources.Load<Sprite>(backgroundInfo.path);
            if (sprite != null)
            {
                spriteRenderer.sprite = sprite;
                spriteRenderer.transform.position = backgroundInfo.position;
            }
            else
            {
                Debug.LogError($"Sprite not found for path: {backgroundInfo.path}");
            }
        }
        else
        {
            Debug.LogError($"No background set for perk: {selectedBackground}");
        }
    }
}
