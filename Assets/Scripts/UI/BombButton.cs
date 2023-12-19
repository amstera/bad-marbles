using UnityEngine;
using System.Collections;

public class BombButton : MonoBehaviour
{
    public MarbleSpawner marbleSpawner;
    public StressReceiver stressReceiver;

    private float destructionDelay = 0.1f;
    private SaveObject savedData;
    private bool isPressed;

    void Awake()
    {
        savedData = SaveManager.Load();
        if (!savedData.SelectedPerks.SelectedSpecial.Contains(PerkEnum.Bomb))
        {
            gameObject.SetActive(false);
        }
    }

    public void OnButtonPressed()
    {
        if (isPressed)
        {
            return;
        }

        isPressed = true;
        marbleSpawner?.DestroyAll(onlyBad: true);
        stressReceiver?.InduceStress(0.8f, true);

        Destroy(gameObject, destructionDelay);
    }
}
