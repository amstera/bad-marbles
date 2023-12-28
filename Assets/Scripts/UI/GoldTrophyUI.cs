using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class GoldTrophyUI : MonoBehaviour, IPointerDownHandler
{
    public CanvasGroup panelCanvasGroup;
    public GameObject popUp;

    public AudioSource plopSound;
    public AudioSource goldTrophyMusic;

    private float defaultBackgroundMusicVolume = 0.5f;
    private SaveObject savedData;

    void Start()
    {
        savedData = SaveManager.Load();
        goldTrophyMusic.volume = defaultBackgroundMusicVolume * savedData.Settings.Volume;

        popUp.transform.localScale = Vector3.zero;
        panelCanvasGroup.blocksRaycasts = false;
    }

    public void Show()
    {
        gameObject.SetActive(true);

        plopSound?.Play();
        StartCoroutine(ShowRoutine());

        MenuMusicPlayer.Instance.backgroundMusic.Stop();
        goldTrophyMusic.Play();
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

        gameObject.SetActive(false);

        MenuMusicPlayer.Instance.backgroundMusic.Play();
        goldTrophyMusic.Stop();
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

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject == gameObject)
        {
            Hide();
        }
    }
}