using System;
using System.Collections.Generic;
using OneManEscapePlan.ModalDialogs.Scripts;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

public class IAPManager : MonoBehaviour, IStoreListener, IDetailedStoreListener
{
    public static IAPManager instance;

    public event Action<Product> OnPurchaseCompletedEvent;
    public event Action<Product> OnPurchaseFailedEvent;

    private IStoreController storeController;
    private IExtensionProvider extensionProvider;
    private SaveObject savedData;

    private const int MaxInitializationRetries = 3;
    private int initializationRetryCount = 0;

    private static readonly Dictionary<Product, string> ProductIdMap = new Dictionary<Product, string>
    {
        {Product.RemoveAds, "com.badmarbles.removeads"},
        // Add other product mappings here
    };

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            savedData = SaveManager.Load();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializePurchasing();
    }

    public void BuyProductID(Product product)
    {
        string productId = GetProductId(product);
        bool isInitialized = IsInitialized();
        if (isInitialized && storeController.products.WithID(productId) != null)
        {
            Debug.Log($"Product is initialized and there exists ID: {productId}");
            storeController.InitiatePurchase(productId);
        }
        else
        {
            int productCount = 0;
            string firstProductName = "";
            if (storeController != null)
            {
                productCount = storeController.products.all.Length;
                firstProductName = productCount > 0 ? storeController.products.all[0].definition.storeSpecificId : "None";
            }

            Debug.Log($"BuyProductID: FAIL. Initialized: {isInitialized}. Products: Count = {productCount}, First Product = {firstProductName}");
            OnPurchaseFailedEvent?.Invoke(product);
        }
    }

    public void RestorePurchases()
    {
        if (!IsInitialized())
        {
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        if (Application.platform == RuntimePlatform.IPhonePlayer
            || Application.platform == RuntimePlatform.OSXEditor
            || Application.platform == RuntimePlatform.OSXPlayer)
        {
            Debug.Log("RestorePurchases started ...");
            var apple = extensionProvider.GetExtension<IAppleExtensions>();
            apple.RestoreTransactions((success, message) =>
            {
                Debug.Log("RestorePurchases result: " + success + " Message: " + message);
                if (success)
                {
                    DialogManager.Instance.ShowDialog("Alert", "Purchases were restored!");
                }
                else
                {
                    DialogManager.Instance.ShowDialog("Alert", "Couldn't restore purchases!");
                }
            });
        }
        else
        {
            Debug.Log("RestorePurchases not supported on this platform. Current = " + Application.platform);
        }
    }

    private bool IsInitialized()
    {
        return storeController != null && storeController.products != null;
    }

    private void InitializePurchasing()
    {
        if (IsInitialized())
            return;

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        foreach (var pair in ProductIdMap)
        {
            builder.AddProduct(pair.Value, ProductType.NonConsumable);
        }
        Debug.Log("Attempting to initialize...");
        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;
        extensionProvider = extensions;

        Debug.Log("IAP initialized");
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError($"IAP initialization failed: {error}. Retry attempt: {initializationRetryCount}");

        if (initializationRetryCount < MaxInitializationRetries)
        {
            initializationRetryCount++;
            Debug.Log("Retrying initialization...");
            InitializePurchasing();
        }
        else
        {
            Debug.LogError("Maximum retry attempts reached. Initialization failed.");
        }
    }


    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);

        if (initializationRetryCount < MaxInitializationRetries)
        {
            initializationRetryCount++;
            Debug.Log("Retrying initialization...");
            InitializePurchasing();
        }
        else
        {
            Debug.LogError("Maximum retry attempts reached. Initialization failed.");
        }
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        Product product = GetProductFromId(args.purchasedProduct.definition.id);
        if (product != Product.None)
        {
            Debug.Log("Purchase successful or restored: " + args.purchasedProduct.definition.id);

            if (UnlockProduct(product))
            {
                OnPurchaseCompletedEvent?.Invoke(product);
            }
        }
        else
        {
            Debug.Log("Purchase failed or unknown product: " + args.purchasedProduct.definition.id);
            OnPurchaseFailedEvent?.Invoke(product);
        }

        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(UnityEngine.Purchasing.Product product, PurchaseFailureDescription failureDescription)
    {
        Product enumProduct = GetProductFromId(product.definition.id);

        Debug.Log($"OnPurchaseFailed: FAIL. Product: {product.definition.id}, PurchaseFailureReason: {failureDescription}");

        OnPurchaseFailedEvent?.Invoke(enumProduct);
    }

    public void OnPurchaseFailed(UnityEngine.Purchasing.Product product, PurchaseFailureReason failureReason)
    {
        Product enumProduct = GetProductFromId(product.definition.id);

        Debug.Log($"OnPurchaseFailed: FAIL. Product: {product.definition.id}, PurchaseFailureReason: {failureReason}");

        OnPurchaseFailedEvent?.Invoke(enumProduct);
    }

    private static Product GetProductFromId(string productId)
    {
        foreach (var pair in ProductIdMap)
        {
            if (pair.Value == productId)
            {
                return pair.Key;
            }
        }
        return Product.None;
    }

    private static string GetProductId(Product product)
    {
        if (ProductIdMap.TryGetValue(product, out var productId))
        {
            return productId;
        }
        throw new ArgumentOutOfRangeException(nameof(product), product, "Product not found in map.");
    }

    private bool UnlockProduct(Product product)
    {
        switch (product)
        {
            case Product.RemoveAds:
                if (!savedData.CanShowAds) return false;
                savedData.CanShowAds = false;
                break;
            case Product.None:
                return false;
        }

        SaveManager.Save(savedData);
        return true;
    }
}

public enum Product
{
    None,
    RemoveAds
}