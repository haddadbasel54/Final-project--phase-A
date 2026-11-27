
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TowerOfFliesGPSManager : MonoBehaviour
{
    [Header("GPS Target (Acre, Israel)")]
    public float targetLat = 32.918333f;
    public float targetLon = 35.071944f;
    public float triggerRange;

    [Header("UI Controls")]
    public Button startButton;

    [Header("Linked Scripts")]
    public PlayAnimationScript animationController;
    public TowerOfFliesAudioAndTextScript narrationController;

    [Header("Required References")]
    public GameObject vuforiaModelTarget;
    public GameObject duplicatedVisuals;

    [Header("Pinch Zoom")]
    [Range(0.01f, 1f)]
    public float zoomSensitivity = 0.3f;

    public float minZoom = 0.3f;
    public float maxZoom = 3f;

    private Transform zoomTarget;
    private Vector3 originalVisualScale;

    private float zoomFactor = 1f;
    private float previousPinchDistance = 0f;

    private bool isNear = false;
    private bool gpsInitialized = false;
    private bool hasClickedStart = false;
    private bool isQuitting = false;
    private bool arModeEnabled = true;

    void Awake()
    {
        // Save the original scale without changing the object's placement.
        if (duplicatedVisuals != null)
        {
            zoomTarget = duplicatedVisuals.transform;
            originalVisualScale = zoomTarget.localScale;
        }
    }

    void Start()
    {
        if (duplicatedVisuals != null)
            duplicatedVisuals.SetActive(false);

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
        if (!isQuitting)
        {
            StopGPSAnimation();

            if (Input.location.status == LocationServiceStatus.Running)
                Input.location.Stop();
        }
    }

    public void SetARMode(bool enabled)
    {
        arModeEnabled = enabled;

        if (!enabled)
        {
            StopGPSAnimation();
        }
        else
        {
            isNear = false;
            hasClickedStart = false;
        }
    }

    void OnStartButtonClicked()
    {
        if (!arModeEnabled || hasClickedStart)
            return;

        if (duplicatedVisuals == null || zoomTarget == null)
        {
            Debug.LogError("GPS Animation: Duplicated Visuals not assigned!");
            return;
        }

        // Reset pinch zoom to the saved original scale.
        zoomFactor = 1f;
        previousPinchDistance = 0f;

        zoomTarget.localScale = originalVisualScale;

        // Activate objects exactly where they are positioned in Unity.
        // Do not change their parent, position, or rotation.
        duplicatedVisuals.SetActive(true);

        // Start animation.
        if (animationController != null)
            animationController.PlayAnimation();

        // Start narration.
        if (narrationController != null)
            narrationController.Play();

        // Hide GPS start button.
        if (startButton != null)
            startButton.gameObject.SetActive(false);

        hasClickedStart = true;

        SaveProgress();
    }

    void Update()
    {
        if (!arModeEnabled)
            return;

        if (hasClickedStart)
            HandlePinchZoom();

        if (!gpsInitialized ||
            Input.location.status != LocationServiceStatus.Running)
            return;

        float currentLat = Input.location.lastData.latitude;
        float currentLon = Input.location.lastData.longitude;

        float dist = GetDistance(currentLat, currentLon);

        if (dist <= triggerRange)
        {
            if (!isNear)
                StartSequence();
        }
        else if (isNear)
        {
            StopSequence();
        }
    }

    void HandlePinchZoom()
    {
        if (zoomTarget == null ||
            duplicatedVisuals == null ||
            !duplicatedVisuals.activeInHierarchy)
            return;

        if (Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            float currentDistance = Vector2.Distance(
                touch1.position,
                touch2.position
            );

            if (previousPinchDistance <= 0f)
            {
                previousPinchDistance = currentDistance;
                return;
            }

            float difference = currentDistance - previousPinchDistance;

            zoomFactor += difference * zoomSensitivity * 0.001f;

            zoomFactor = Mathf.Clamp(
                zoomFactor,
                minZoom,
                maxZoom
            );

            // Scale the entire animated parent.
            // Original scale is always preserved as the baseline.
            zoomTarget.localScale = originalVisualScale * zoomFactor;

            previousPinchDistance = currentDistance;
        }
        else
        {
            previousPinchDistance = 0f;
        }
    }

    void StartSequence()
    {
        isNear = true;
        hasClickedStart = false;

        if (vuforiaModelTarget != null)
            vuforiaModelTarget.SetActive(false);

        if (duplicatedVisuals != null)
            duplicatedVisuals.SetActive(false);

        if (startButton != null)
            startButton.gameObject.SetActive(true);
    }

    void StopGPSAnimation()
    {
        hasClickedStart = false;
        previousPinchDistance = 0f;
        zoomFactor = 1f;

        // Restore original scale.
        if (zoomTarget != null)
            zoomTarget.localScale = originalVisualScale;

        if (duplicatedVisuals != null)
            duplicatedVisuals.SetActive(false);

        if (startButton != null)
            startButton.gameObject.SetActive(false);

        if (animationController != null)
            animationController.StopAnimation();

        if (narrationController != null)
            narrationController.Stop();
    }

    public void StopSequence()
    {
        isNear = false;

        StopGPSAnimation();

        if (vuforiaModelTarget != null && arModeEnabled)
            vuforiaModelTarget.SetActive(true);
    }

    float GetDistance(float lat1, float lon1)
    {
        float R = 6371000f;

        float dLat = (targetLat - lat1) * Mathf.Deg2Rad;
        float dLon = (targetLon - lon1) * Mathf.Deg2Rad;

        float a =
            Mathf.Sin(dLat / 2) * Mathf.Sin(dLat / 2) +
            Mathf.Cos(lat1 * Mathf.Deg2Rad) *
            Mathf.Cos(targetLat * Mathf.Deg2Rad) *
            Mathf.Sin(dLon / 2) * Mathf.Sin(dLon / 2);

        return R * (2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a)));
    }

    IEnumerator SafeStartGPS()
    {
        yield return new WaitForSeconds(1f);

#if UNITY_ANDROID
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(
            UnityEngine.Android.Permission.FineLocation))
        {
            UnityEngine.Android.Permission.RequestUserPermission(
                UnityEngine.Android.Permission.FineLocation);
        }
#endif

        Input.location.Start(1f, 1f);

        int maxWait = 20;

        while (Input.location.status == LocationServiceStatus.Initializing
               && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (Input.location.status != LocationServiceStatus.Failed)
            gpsInitialized = true;
    }

    private void SaveProgress()
    {
        int slot = PlayerPrefs.GetInt("ActiveSlotIndex", 0);

        if (slot > 0)
        {
            PlayerPrefs.SetInt(
                "Slot" + slot + "_BronzeUnlocked_tower_of_flies", 1);

            PlayerPrefs.Save();
        }
    }
}
