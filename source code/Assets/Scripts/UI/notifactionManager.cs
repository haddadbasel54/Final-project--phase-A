
using UnityEngine;
using UnityEngine.Android;

public class SiteGeofenceManager : MonoBehaviour
{
    private const string FineLocation =
        "android.permission.ACCESS_FINE_LOCATION";

    private const string BackgroundLocation =
        "android.permission.ACCESS_BACKGROUND_LOCATION";

    private const string Notifications =
        "android.permission.POST_NOTIFICATIONS";

    private bool registrationAttempted;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        CheckPermissionsAndRegister();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && !registrationAttempted)
        {
            CheckPermissionsAndRegister();
        }
    }

    private void CheckPermissionsAndRegister()
    {
#if UNITY_ANDROID && !UNITY_EDITOR

        if (!Permission.HasUserAuthorizedPermission(FineLocation))
        {
            Permission.RequestUserPermission(FineLocation);
            return;
        }

        if (GetAndroidVersion() >= 33 &&
            !Permission.HasUserAuthorizedPermission(Notifications))
        {
            Permission.RequestUserPermission(Notifications);
            return;
        }

        if (GetAndroidVersion() >= 29 &&
            !Permission.HasUserAuthorizedPermission(BackgroundLocation))
        {
            Debug.LogWarning(
                "[SiteGeofenceManager] Please grant Location: Allow all the time in Android Settings."
            );
            return;
        }

        RegisterGeofences();

#endif
    }

    private int GetAndroidVersion()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        using (AndroidJavaClass version =
            new AndroidJavaClass("android.os.Build$VERSION"))
        {
            return version.GetStatic<int>("SDK_INT");
        }
#else
        return 0;
#endif
    }

    private void RegisterGeofences()
    {
#if UNITY_ANDROID && !UNITY_EDITOR

        if (registrationAttempted)
            return;

        registrationAttempted = true;

        try
        {
            using (AndroidJavaClass plugin =
                new AndroidJavaClass(
                    "com.example.sitegeofence.GeofencePlugin"))
            using (AndroidJavaClass unityPlayer =
                new AndroidJavaClass(
                    "com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject activity =
                unityPlayer.GetStatic<AndroidJavaObject>(
                    "currentActivity"))
            {
                plugin.CallStatic("registerGeofences", activity);
            }

            Debug.Log(
                "[SiteGeofenceManager] Geofence registration requested."
            );
        }
        catch (System.Exception e)
        {
            registrationAttempted = false;
            Debug.LogError(
                "[SiteGeofenceManager] Registration failed: " + e
            );
        }

#endif
    }
}
