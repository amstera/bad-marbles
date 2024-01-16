using System.Collections;
using UnityEngine;

public class BombButton : MonoBehaviour
{
    public MarbleSpawner marbleSpawner;
    public StressReceiver stressReceiver;
    public SlowTimeButton slowTimeButton;
    public GameObject shine;
    public AudioSource plopSound;

    private float destructionDelay = 0.1f;
    private float moveDuration = 0.25f;
    private SaveObject savedData;
    private bool isPressed = true;
    private bool bombActive, slowTimeActive;

    void Awake()
    {
        savedData = SaveManager.Load();
        CheckPerks();

        if (bombActive)
        {
            StartCoroutine(EnableButton());
        }
        else
        {
            gameObject.SetActive(false);
            return;
        }

        if (!slowTimeActive)
        {
            MovePosition(slowTimeButton.transform.position, false);
        }
    }

    public void OnButtonPressed()
    {
        if (isPressed)
        {
            return;
        }

        plopSound?.Play();
        isPressed = true;
        shine.SetActive(false);
        marbleSpawner?.DestroyAll(onlyBad: true);
        stressReceiver?.InduceStress(0.8f, true);

        Destroy(gameObject, destructionDelay);
    }

    private IEnumerator EnableButton()
    {
        yield return new WaitForSeconds(3);

        isPressed = false;
    }

    private void CheckPerks()
    {
        foreach (var perk in savedData.SelectedPerks.SelectedSpecial)
        {
            if (perk == PerkEnum.Bomb)
            {
                bombActive = true;
            }
            else if (perk == PerkEnum.SlowTime)
            {
                slowTimeActive = true;
            }
        }
    }

    public void MovePosition(Vector3 newPosition, bool isAnimated)
    {
        if (isAnimated)
        {
            StartCoroutine(MoveToPosition(newPosition, moveDuration));
        }
        else
        {
            transform.position = newPosition;
        }
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition, float duration)
    {
        float time = 0;
        Vector3 startPosition = transform.position;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition; // Ensure the final position is set accurately
    }
}
