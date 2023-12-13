using UnityEngine;
using System.Collections.Generic;

public class Ramp : MonoBehaviour
{
    public GameObject hitPrefab;
    public AudioClip boardHitSound;

    private Dictionary<PerkEnum, string> rampMaterials;
    private SaveObject savedData;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        rampMaterials = new Dictionary<PerkEnum, string>
        {
            { PerkEnum.DefaultRamp, "Materials/Ramp/Main" },
            { PerkEnum.GoldRamp, "Materials/Ramp/Gold" }
        };

        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        savedData = SaveManager.Load();
        SetRampMaterial(savedData.SelectedPerks.SelectedRamp);
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

    void Update()
    {
        if (GameManager.Instance.Lives <= 0)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
            {
                Destroy(Instantiate(hitPrefab, hit.point, Quaternion.identity), 1);

                if (savedData.Settings.SFXEnabled)
                {
                    AudioSource.PlayClipAtPoint(boardHitSound, hit.point);
                }
            }
        }
    }
}