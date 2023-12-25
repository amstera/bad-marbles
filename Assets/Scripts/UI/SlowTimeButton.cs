using UnityEngine;
using System.Collections;
using UnityEngine.UI.ProceduralImage;

public class SlowTimeButton : MonoBehaviour
{
    public GameObject bombButton;
    public GameObject shine;
    public ProceduralImage image;

    private float slowDuration = 5.0f;
    private float originalTimeScale = 1.0f;
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
            MovePosition(bombButton.transform.position);
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
        StartCoroutine(SlowDownTime());
    }

    public void MovePosition(Vector3 position)
    {
        transform.position = position;
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