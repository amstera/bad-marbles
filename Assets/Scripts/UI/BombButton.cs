using System.Collections;
using UnityEngine;

public class BombButton : MonoBehaviour
{
    public MarbleSpawner marbleSpawner;
    public StressReceiver stressReceiver;
    public SlowTimeButton slowTimeButton;
    public GameObject shine;

    private float destructionDelay = 0.1f;
    private SaveObject savedData;
    private bool isPressed = true;

    void Awake()
    {
        savedData = SaveManager.Load();
        if (savedData.SelectedPerks.SelectedSpecial.Contains(PerkEnum.Bomb))
        {
            StartCoroutine(EnableButton());
        }
        else
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

        if (slowTimeButton != null && slowTimeButton.gameObject.activeSelf)
        {
            slowTimeButton.MovePosition(transform.position, true);
        }

        Destroy(gameObject, destructionDelay);
    }

    private IEnumerator EnableButton()
    {
        yield return new WaitForSeconds(3);

        isPressed = false;
    }
}
