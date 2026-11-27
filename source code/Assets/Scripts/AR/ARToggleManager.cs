
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Events;
using TMPro;
using Vuforia;

public class ARToggleManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text buttonText;
    public GameObject videoCanvas;

    [Header("AR Elements")]
    public GameObject modelTarget;
    public VuforiaBehaviour vuforiaCamera;

    [Header("GPS Integration")]
    public TowerOfFliesGPSManager gpsManager;

    [Header("Media & Audio")]
    public VideoPlayer videoPlayer;
    public UnityEvent onVideoStart;
    public UnityEvent onVideoStop;

    private bool arIsOn = true;

    public void ToggleARView()
    {
        arIsOn = !arIsOn;

        if (arIsOn)
        {
            // Switch to AR
            if (buttonText != null)
                buttonText.text = "Turn AR Off";

            if (vuforiaCamera != null)
                vuforiaCamera.enabled = true;

            if (modelTarget != null)
                modelTarget.SetActive(true);

            if (videoPlayer != null)
                videoPlayer.Stop();

            if (videoCanvas != null)
                videoCanvas.SetActive(false);

            if (onVideoStop != null)
                onVideoStop.Invoke();

            if (gpsManager != null)
                gpsManager.SetARMode(true);
        }
        else
        {
            // Switch to non-AR video
            if (buttonText != null)
                buttonText.text = "Turn AR On";

            if (gpsManager != null)
                gpsManager.SetARMode(false);

            if (vuforiaCamera != null)
                vuforiaCamera.enabled = false;

            if (modelTarget != null)
                modelTarget.SetActive(false);

            if (videoCanvas != null)
                videoCanvas.SetActive(true);

            if (videoPlayer != null)
                videoPlayer.Play();

            if (onVideoStart != null)
                onVideoStart.Invoke();
        }
    }
}
