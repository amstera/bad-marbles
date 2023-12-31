using UnityEngine;
using System.Collections.Generic;

public class Ramp : MonoBehaviour
{
    public GameObject hitPrefab;
    public AudioClip boardHitSound;
    public float scrollSpeed = 0.1f;

    private Dictionary<PerkEnum, string> rampMaterials;
    private SaveObject savedData;
    private MeshRenderer meshRenderer;
    private string fileLocation = "Materials/Ramp";
    private float yOffset = 0f;

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
            { PerkEnum.RoadRamp, $"{fileLocation}/Road" },
            { PerkEnum.GrassRamp, $"{fileLocation}/Grass" },
            { PerkEnum.DarkRamp, $"{fileLocation}/Dark" },
            { PerkEnum.RainbowRamp, $"{fileLocation}/Rainbow" }
        };

        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        savedData = SaveManager.Load();
        SetRampMaterial(savedData.SelectedPerks.SelectedRamp);
    }

    private void Update()
    {
        UpdateMaterialOffset();
    }

    private void UpdateMaterialOffset()
    {
        yOffset += Time.deltaTime * scrollSpeed;
        yOffset = yOffset % 1; // Ensures yOffset stays between 0 and 1

        if (meshRenderer.material != null)
        {
            Vector2 currentOffset = meshRenderer.material.GetTextureOffset("_MainTex");
            meshRenderer.material.SetTextureOffset("_MainTex", new Vector2(currentOffset.x, yOffset));

            if (meshRenderer.material.HasProperty("_BumpMap"))
            {
                Vector2 currentBumpMapOffset = meshRenderer.material.GetTextureOffset("_BumpMap");
                meshRenderer.material.SetTextureOffset("_BumpMap", new Vector2(currentBumpMapOffset.x, yOffset));
            }
        }
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