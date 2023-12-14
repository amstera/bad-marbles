using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    private Dictionary<PerkEnum, (string path, Vector3 position)> backgrounds;

    public SpriteRenderer spriteRenderer;

    private SaveObject savedData;
    private float zCoord = 51.5f;

    private void Awake()
    {
        backgrounds = new Dictionary<PerkEnum, (string, Vector3)>
        {
            { PerkEnum.DefaultBackground, ("Images/Backgrounds/Sunset", new Vector3(-0.3f, 26.8f, zCoord)) },
            { PerkEnum.StreamBackground, ("Images/Backgrounds/Stream", new Vector3(0f, 18f, zCoord)) },
            { PerkEnum.ForestBackground, ("Images/Backgrounds/Forest", new Vector3(0f, 18.5f, zCoord)) },
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
