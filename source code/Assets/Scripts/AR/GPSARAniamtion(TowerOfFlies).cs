using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Vuforia;

public class TowerOfFliesGPSManager : MonoBehaviour
{
    [Header("GPS Target (Acre, Israel)")]
    public float targetLat = 32.918333f;
    public float targetLon = 35.071944f;
    public float triggerRange = 180f;

    [Header("UI Controls")]
    public Button startButton;

    [Header("Linked Scripts")]
    public PlayAnimationScript animationController;
    public TowerOfFliesAudioAndTextScript narrationController;

    [Header("Required References")]
    public GameObject vuforiaModelTarget;
    public GameObject duplicatedVisuals;

    [Header("Height & Scaling")]
    public float seaLevelOffset = -4.0f;
    public Vector3 farScale = new Vector3(0.5f, 0.5f, 0.5f);
    public Vector3 nearScale = new Vector3(1.0f, 1.0f, 1.0f);
    [Range(0.1f, 5.0f)] public float lerpSpeed = 1.5f;

    private bool isNear = false;
    private bool gpsInitialized = false;
    private bool hasClickedStart = false;
    private bool isQuitting = false;

    void Start()
    {
        if (duplicatedVisuals != null)
        {
            duplicatedVisuals.SetActive(false);
            duplicatedVisuals.transform.SetParent(null);
        }

        if (startButton != null)
        {
            startButton.gameObject.SetActive(false);
            startButton.onClick.AddListener(OnStartButtonClicked);
        }

        Input.compass.enabled = true;
        StartCoroutine(SafeStartGPS());
    }

    void OnApplicationQuit()
    {
        isQuitting = true;
    }

    void OnDisable()
    {
        // Only run cleanup if we are switching scenes/views, not closing the whole app
        // This prevents the Vuforia GuideViewRenderer error
        if (!isQuitting)
        {
            StopSequence();
            if (Input.location.status == LocationServiceStatus.Running)
            {
                Input.location.Stop();
            }
        }
    }

    void OnStartButtonClicked()
    {
        if (animationController != null) animationController.PlayAnimation();
        if (narrationController != null) narrationController.Play();

        if (startButton != null)
            startButton.gameObject.SetActive(false);

        hasClickedStart = true;
        SaveProgress();
    }

    void Update()
    {
        if (!gpsInitialized || Input.location.status != LocationServiceStatus.Running)
            return;

        float currentLat = Input.location.lastData.latitude;
        float currentLon = Input.location.lastData.longitude;
        float dist = GetDistance(currentLat, currentLon);

        if (dist <= triggerRange)
        {
            if (!isNear) StartSequence();
            UpdateTowerTransform(currentLat, currentLon, dist);
        }
        else if (isNear)
        {
            StopSequence();
        }
    }

    void StartSequence()
    {
        isNear = true;
        if (vuforiaModelTarget != null)
        {
            var observer = vuforiaModelTarget.GetComponent<ObserverBehaviour>();
            if (observer != null) observer.enabled = false;
            vuforiaModelTarget.SetActive(false);
        }

        if (duplicatedVisuals != null) duplicatedVisuals.SetActive(true);

        if (startButton != null && !hasClickedStart)
            startButton.gameObject.SetActive(true);
    }

    // Public so ARToggleManager can reset everything
    public void StopSequence()
    {
        isNear = false;
        hasClickedStart = false; // This allows the button to reappear next time

        if (vuforiaModelTarget != null)
        {
            vuforiaModelTarget.SetActive(true);
            var observer = vuforiaModelTarget.GetComponent<ObserverBehaviour>();
            if (observer != null) observer.enabled = true;
        }

        if (duplicatedVisuals != null) duplicatedVisuals.SetActive(false);
        if (startButton != null) startButton.gameObject.SetActive(false);

        if (animationController != null) animationController.StopAnimation();
        if (narrationController != null) narrationController.Stop();
    }

    void UpdateTowerTransform(float lat, float lon, float dist)
    {
        float t = Mathf.InverseLerp(triggerRange, 15f, dist);
        duplicatedVisuals.transform.localScale = Vector3.Lerp(farScale, nearScale, t);

        float bearing = GetBearing(lat, lon, targetLat, targetLon);
        float rotationAngle = bearing - Input.compass.trueHeading;
        Vector3 direction = new Vector3(Mathf.Sin(rotationAngle * Mathf.Deg2Rad), 0, Mathf.Cos(rotationAngle * Mathf.Deg2Rad));

        Vector3 targetPosition = Camera.main.transform.position + (direction * Mathf.Max(dist, 5f));
        targetPosition.y = Camera.main.transform.position.y + seaLevelOffset;

        duplicatedVisuals.transform.position = Vector3.Lerp(duplicatedVisuals.transform.position, targetPosition, Time.deltaTime * lerpSpeed);

        Vector3 lookPos = Camera.main.transform.position;
        lookPos.y = duplicatedVisuals.transform.position.y;
        duplicatedVisuals.transform.LookAt(lookPos);
    }

    float GetDistance(float lat1, float lon1)
    {
        float R = 6371000f;
        float dLat = (targetLat - lat1) * Mathf.Deg2Rad;
        float dLon = (targetLon - lon1) * Mathf.Deg2Rad;
        float a = Mathf.Sin(dLat / 2) * Mathf.Sin(dLat / 2) +
                  Mathf.Cos(lat1 * Mathf.Deg2Rad) * Mathf.Cos(targetLat * Mathf.Deg2Rad) *
                  Mathf.Sin(dLon / 2) * Mathf.Sin(dLon / 2);
        return R * (2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a)));
    }

    float GetBearing(float lat1, float lon1, float lat2, float lon2)
    {
        float dLon = (lon2 - lon1) * Mathf.Deg2Rad;
        float y = Mathf.Sin(dLon) * Mathf.Cos(lat2 * Mathf.Deg2Rad);
        float x = Mathf.Cos(lat1 * Mathf.Deg2Rad) * Mathf.Sin(lat2 * Mathf.Deg2Rad) -
                  Mathf.Sin(lat1 * Mathf.Deg2Rad) * Mathf.Cos(lat2 * Mathf.Deg2Rad) * Mathf.Cos(dLon);
        return (Mathf.Atan2(y, x) * Mathf.Rad2Deg + 360) % 360;
    }

    IEnumerator SafeStartGPS()
    {
        yield return new WaitForSeconds(1.0f);
#if UNITY_ANDROID
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.FineLocation))
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.FineLocation);
#endif
        Input.location.Start(1f, 1f);
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }
        if (Input.location.status != LocationServiceStatus.Failed) gpsInitialized = true;
    }

    private void SaveProgress()
    {
        int slot = PlayerPrefs.GetInt("ActiveSlotIndex", 0);
        if (slot > 0)
        {
            PlayerPrefs.SetInt("Slot" + slot + "_BronzeUnlocked_tower_of_flies", 1);
            PlayerPrefs.Save();
        }
    }
}