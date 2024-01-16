using UnityEngine;
using System.Collections;
using UnityEngine.UI.ProceduralImage;

public class SlowTimeButton : MonoBehaviour
{
    public BombButton bombButton;
    public GameObject shine;
    public ProceduralImage image;
    public AudioSource plopSound;

    private float slowDuration = 5.0f;
    private float originalTimeScale = 1.0f;
    private bool isPressed;
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

    private void CheckPerks()
    {
        foreach (var perk in savedData.SelectedPerks.SelectedSpecial)
        {
            if (perk == PerkEnum.SlowTime)
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

        if (bombButton != null && bombButton.gameObject.activeSelf)
        {
            bombButton.MovePosition(transform.position, true);
        }

        Destroy(gameObject);
    }
}