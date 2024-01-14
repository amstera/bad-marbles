using UnityEngine;
using System.Collections;
using UnityEngine.UI.ProceduralImage;

public class SlowTimeButton : MonoBehaviour
{
    public GameObject bombButton;
    public GameObject shine;
    public ProceduralImage image;
    public AudioSource plopSound;

    private float slowDuration = 5.0f;
    private float originalTimeScale = 1.0f;
    private float moveDuration = 0.25f;
    private bool isPressed;
    private bool bombActive;
    private bool slowTimeActive;
    private SaveObject savedData;

    void Awake()
    {
        savedData = SaveManager.Load();
        CheckPerks();

        if (!slowTimeActive)
        {
            gameObject.SetActive(false);
            return;
        }

        if (!bombActive)
        {
            MovePosition(bombButton.transform.position, false);
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
        StartCoroutine(SlowDownTime());
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

    private IEnumerator SlowDownTime()
    {
        Time.timeScale = 0.5f;
        float startTime = Time.realtimeSinceStartup;

        while (Time.realtimeSinceStartup - startTime < slowDuration)
        {
            float elapsed = Time.realtimeSinceStartup - startTime;
            image.fillAmount = elapsed / slowDuration;
            yield return null;
        }

        Time.timeScale = originalTimeScale; // Reset time scale to normal

        Destroy(gameObject);
    }
}