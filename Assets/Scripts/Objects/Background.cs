using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    private Dictionary<PerkEnum, (string path, Vector3 position)> backgrounds;

    public SpriteRenderer spriteRenderer;

    private SaveObject savedData;
    private float zCoord = 51.5f;
    private string fileLocation = "Images/Backgrounds/Game";

    private void Awake()
    {
        backgrounds = new Dictionary<PerkEnum, (string, Vector3)>
        {
            { PerkEnum.DefaultBackground, ($"{fileLocation}/Sunset", new Vector3(-0.3f, 26.8f, zCoord)) },
            { PerkEnum.StreamBackground, ($"{fileLocation}/Stream", new Vector3(0f, 18f, zCoord)) },
            { PerkEnum.CandyBackground, ($"{fileLocation}/Candy", new Vector3(-0.7f, 15.2f, zCoord)) },
            { PerkEnum.ToysBackground, ($"{fileLocation}/Toys", new Vector3(-0.6f, 19.3f, zCoord)) },
            { PerkEnum.RetroBackground, ($"{fileLocation}/Retro", new Vector3(0, 30.9f, 94.6f)) },
            { PerkEnum.SnowBackground, ($"{fileLocation}/Snow", new Vector3(-35.1f, 18.1f, 87)) },
            { PerkEnum.MedievalBackground, ($"{fileLocation}/Medieval", new Vector3(14.2f, 23f, 82)) },
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
