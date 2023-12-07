using TMPro;
using UnityEngine;

public class PerksManager : MonoBehaviour
{
    public TextMeshProUGUI totalPointsText;

    private SaveObject savedData;

    void Start()
    {
        savedData = SaveManager.Load();
        totalPointsText.text = savedData.Points == 1 ? "1 POINT" : $"{savedData.Points} POINTS";
    }
}
