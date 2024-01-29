using UnityEngine;

public static class DeviceTypeChecker
{
    public static bool IsTablet()
    {
        float aspectRatio = (float)Screen.height / Screen.width;
        return aspectRatio < 1.5f; // Common tablet aspect ratios are less than 1.5
    }
}