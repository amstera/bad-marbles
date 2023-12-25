using UnityEngine;

public class BombButton : MonoBehaviour
{
    public MarbleSpawner marbleSpawner;
    public StressReceiver stressReceiver;
    public SlowTimeButton slowTimeButton;
    public GameObject shine;

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
        shine.SetActive(false);
        marbleSpawner?.DestroyAll(onlyBad: true);
        stressReceiver?.InduceStress(0.8f, true);

        if (slowTimeButton != null)
        {
            slowTimeButton.MovePosition(transform.position);
        }

        Destroy(gameObject, destructionDelay);
    }
}
