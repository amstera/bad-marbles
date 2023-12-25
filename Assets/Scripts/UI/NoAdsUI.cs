using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using OneManEscapePlan.ModalDialogs.Scripts;

public class NoAdsUI : MonoBehaviour, IPointerDownHandler
{
    public CanvasGroup panelCanvasGroup;
    public GameObject popUp;
    public GameObject noAdsButton;
    public ParticleSystem confettiPS;

    public AudioSource plopSound;
    public AudioSource purchaseSound;

    void Start()
    {
        popUp.transform.localScale = Vector3.zero;
        panelCanvasGroup.blocksRaycasts = false;

        // Subscribe to the purchase events
        IAPManager.instance.OnPurchaseCompletedEvent += HandlePurchaseComplete;
        IAPManager.instance.OnPurchaseFailedEvent += HandlePurchaseFailed;
    }

    public void Show()
    {
        panelCanvasGroup.blocksRaycasts = true;
        StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        // Fade in the panel
        yield return StartCoroutine(FadeCanvasGroup(panelCanvasGroup, 0.2f, 1));

        // Pop-in animation for the pop-up
        popUp.transform.localScale = Vector3.zero;
        yield return StartCoroutine(PopIn(popUp, 0.2f));
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

    public void AttemptPurchaseRemoveAds()
    {
        plopSound?.Play();
        IAPManager.instance.BuyProductID(Product.RemoveAds);
    }

    private void HandlePurchaseComplete(Product product)
    {
        confettiPS.Play();
        noAdsButton.SetActive(false);
        purchaseSound?.Play();
        Hide();
    }

    private void HandlePurchaseFailed(Product product)
    {
        DialogManager.Instance.ShowDialog("Alert", "Your purchase wasn't successful!");
        Debug.Log("Purchase of Remove Ads failed.");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject == gameObject)
        {
            Hide();
        }
    }

    void OnDestroy()
    {
        IAPManager.instance.OnPurchaseCompletedEvent -= HandlePurchaseComplete;
        IAPManager.instance.OnPurchaseFailedEvent -= HandlePurchaseFailed;
    }
}