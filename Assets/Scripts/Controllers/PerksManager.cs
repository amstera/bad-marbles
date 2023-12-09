using EasyTransition;
using TMPro;
using UnityEngine;

public class PerksManager : MonoBehaviour
{
    public TextMeshProUGUI totalPointsText;
    public TransitionSettings transition;

    public AudioSource plopSound;

    private SaveObject savedData;

    void Start()
    {
        savedData = SaveManager.Load();
        totalPointsText.text = savedData.Points == 1 ? "1 POINT" : $"{savedData.Points} POINTS";
    }

    public void LoadMenu()
    {
        TransitionManager.Instance().Transition("Menu", transition, 0);

        plopSound?.Play();
    }
}
