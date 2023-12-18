using UnityEngine;
using System.Collections.Generic;

public class Ramp : MonoBehaviour
{
    public GameObject hitPrefab;
    public AudioClip boardHitSound;

    private Dictionary<PerkEnum, string> rampMaterials;
    private SaveObject savedData;
    private MeshRenderer meshRenderer;
    private string fileLocation = "Materials/Ramp";

    private void Awake()
    {
        rampMaterials = new Dictionary<PerkEnum, string>
        {
            { PerkEnum.DefaultRamp, $"{fileLocation}/Main" },
            { PerkEnum.GoldRamp, $"{fileLocation}/Gold" },
            { PerkEnum.ChocolateRamp, $"{fileLocation}/Chocolate" },
            { PerkEnum.IceRamp, $"{fileLocation}/Ice" },
            { PerkEnum.WoodRamp, $"{fileLocation}/Wood" },
            { PerkEnum.SciFiRamp, $"{fileLocation}/SciFi" },
            { PerkEnum.RoadRamp, $"{fileLocation}/Road" }
        };

        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        savedData = SaveManager.Load();
        SetRampMaterial(savedData.SelectedPerks.SelectedRamp);
    }

    public void Hit(Vector3 hitPoint)
    {
        Destroy(Instantiate(hitPrefab, hitPoint, Quaternion.identity), 1);

        if (savedData.Settings.SFXEnabled)
        {
            AudioSource.PlayClipAtPoint(boardHitSound, hitPoint);
        }
    }

    private void SetRampMaterial(PerkEnum selectedRamp)
    {
        if (rampMaterials.TryGetValue(selectedRamp, out var materialPath))
        {
            var material = Resources.Load<Material>(materialPath);
            if (material != null)
            {
                meshRenderer.material = material;
            }
            else
            {
                Debug.LogError($"Material not found for path: {materialPath}");
            }
        }
        else
        {
            Debug.LogError($"No material set for ramp perk: {selectedRamp}");
        }
    }
}