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
            buttonText.text = "Turn AR Off";
            if (vuforiaCamera != null) vuforiaCamera.enabled = true;

            modelTarget.SetActive(true);
            videoCanvas.SetActive(false);
            videoPlayer.Stop();

            if (onVideoStop != null) onVideoStop.Invoke();
        }
        else
        {
            buttonText.text = "Turn AR On";
            if (vuforiaCamera != null) vuforiaCamera.enabled = false;

            // This resets the GPS sequence so the tower hides and button resets
            if (gpsManager != null)
            {
                gpsManager.StopSequence();
            }

            modelTarget.SetActive(false);
            videoCanvas.SetActive(true);
            videoPlayer.Play();

            if (onVideoStart != null) onVideoStart.Invoke();
        }
    }
}