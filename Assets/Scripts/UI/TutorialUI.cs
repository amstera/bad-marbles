using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;
using TMPro;

public class TutorialUI : MonoBehaviour
{
    public CanvasGroup panelCanvasGroup;
    public GameObject popUp;
    public Button marbleButton1, marbleButton2;
    public CanvasGroup page1, page2;
    public GameObject greenMarble;
    public Image greenMarbleImage;
    public GameObject shatteredMarble1, shatteredMarble2;
    public Action ClosePopup;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI closeButtonText;

    public AudioSource plopSound;
    public AudioSource marbleHitSound;
    public AudioSource pointGainedSound;

    private float rotationSpeed = -20f;
    private Vector3 greenMarbleStartPos;
    private float originalScoreTextY;
    private float greenMarbleMoveSpeed = 100f;
    private IEnumerator scoreTextRoutine;

    void Start()
    {
        popUp.transform.localScale = Vector3.zero;

        marbleButton1.onClick.AddListener(() => MarbleButtonPressed());
        marbleButton2.onClick.AddListener(() => MarbleButtonPressed());

        greenMarbleStartPos = greenMarble.transform.localPosition;
        originalScoreTextY = scoreText.transform.localPosition.y;
    }

    void Update()
    {
        if (page1.isActiveAndEnabled)
        {
            marbleButton1.transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
            marbleButton2.transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
        else if (page2.isActiveAndEnabled)
        {
            greenMarble.transform.Rotate(0, 0, rotationSpeed * 2 * Time.deltaTime);
            MoveAndResetGreenMarble();
        }
    }

    public void Show(string closeButtonText = null)
    {
        gameObject.SetActive(true);
        if (closeButtonText != null)
        {
            this.closeButtonText.text = closeButtonText;
        }
        StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        // Fade in the panel
        yield return StartCoroutine(FadeCanvasGroup(panelCanvasGroup, 0.2f, 1));

        // Pop-in animation for the pop-up
        popUp.transform.localScale = Vector3.zero;
        yield return StartCoroutine(PopIn(popUp, 0.2f));

        panelCanvasGroup.blocksRaycasts = true;
    }

    public void Hide()
    {
        StartCoroutine(HideRoutine());
    }

    private IEnumerator HideRoutine()
    {
        // Fade out the entire panel
        yield return StartCoroutine(FadeCanvasGroup(panelCanvasGroup, 0.2f, 0));
        popUp.transform.localScale = Vector3.zero;
        panelCanvasGroup.blocksRaycasts = false;

        ClosePopup?.Invoke();

        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float time, float targetAlpha)
    {
        float startAlpha = cg.alpha;
        for (float t = 0; t < 1; t += Time.deltaTime / time)
        {
            cg.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }
        cg.alpha = targetAlpha;
    }

    private IEnumerator PopIn(GameObject obj, float time)
    {
        Vector3 originalScale = obj.transform.localScale;
        Vector3 targetScale = Vector3.one;
        for (float t = 0; t < 1; t += Time.deltaTime / time)
        {
            obj.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }
        obj.transform.localScale = targetScale;
    }

    private void MarbleButtonPressed()
    {
        plopSound?.Play();
        marbleHitSound?.Play();

        marbleButton1.gameObject.SetActive(false);
        marbleButton2.gameObject.SetActive(false);

        ActivateAndShatterMarble(shatteredMarble1);
        ActivateAndShatterMarble(shatteredMarble2);

        StartCoroutine(SwitchPageRoutine());
    }

    private void ActivateAndShatterMarble(GameObject marbleObject)
    {
        marbleObject.SetActive(true);
        foreach (Transform child in marbleObject.transform)
        {
            StartCoroutine(MoveShard(child));
        }
    }

    private IEnumerator MoveShard(Transform shard)
    {
        float duration = 2f;
        var dir = Random.insideUnitSphere.normalized;
        var speed = 10000;

        Vector3 startPosition = shard.localPosition;
        Vector3 endPosition = startPosition + new Vector3(dir.x, dir.y, 0) * speed;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            shard.localPosition = Vector3.Lerp(startPosition, endPosition, t / duration);
            yield return null;
        }

        shard.gameObject.SetActive(false);
    }

    private void MoveAndResetGreenMarble()
    {
        float fadeStartX = greenMarbleStartPos.x + 150;
        float fadeEndX = greenMarbleStartPos.x + 200;

        if (greenMarble.transform.localPosition.x < fadeEndX)
        {
            // Move the marble
            greenMarble.transform.localPosition += new Vector3(greenMarbleMoveSpeed * Time.deltaTime, 0, 0);

            // Start fading out and move scoreText if it reaches the fade start position
            if (greenMarble.transform.localPosition.x >= fadeStartX)
            {
                float fadeAmount = (greenMarble.transform.localPosition.x - fadeStartX) / (fadeEndX - fadeStartX);
                greenMarbleImage.color = new Color(1f, 1f, 1f, 1f - fadeAmount);

                // Trigger scoreText animation if not already started
                if (scoreTextRoutine == null)
                {
                    scoreTextRoutine = AnimateScoreText();
                    StartCoroutine(scoreTextRoutine);
                }
            }
        }
        else
        {
            // Reset position and color of the marble
            greenMarble.transform.localPosition = greenMarbleStartPos;
            greenMarble.transform.localRotation = Quaternion.identity;
            greenMarbleImage.color = new Color(1f, 1f, 1f, 1f);

            // Reset scoreText
            if (scoreTextRoutine != null)
            {
                StopCoroutine(scoreTextRoutine);
                scoreTextRoutine = null;
                ResetScoreText();
            }
        }
    }

    private IEnumerator AnimateScoreText()
    {
        pointGainedSound?.Play();

        scoreText.alpha = 1;
        Vector3 startPos = scoreText.transform.localPosition;
        Vector3 endPos = startPos + new Vector3(0, 100, 0);

        float duration = 1f;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            scoreText.transform.localPosition = Vector3.Lerp(startPos, endPos, t / duration);
            scoreText.alpha = Mathf.Lerp(1, 0, t / duration);
            yield return null;
        }

        scoreText.transform.localPosition = endPos;
        scoreText.alpha = 0;
    }

    private void ResetScoreText()
    {
        scoreText.transform.localPosition = new Vector3(scoreText.transform.localPosition.x, originalScoreTextY, scoreText.transform.localPosition.z);
        scoreText.alpha = 0;
    }

    private IEnumerator SwitchPageRoutine()
    {
        // Start fading out page1
        StartCoroutine(FadeCanvasGroup(page1, 0.25f, 0));

        // Wait for the fade out to complete
        yield return new WaitForSeconds(0.25f);

        page1.gameObject.SetActive(false);
        page2.gameObject.SetActive(true);

        // Start fading in page2
        StartCoroutine(FadeCanvasGroup(page2, 0.5f, 1));
    }
}