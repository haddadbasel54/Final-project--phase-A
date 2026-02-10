using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Events;
using TMPro;
using Vuforia; // Add this to control the camera

public class ARToggleManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text buttonText;
    public GameObject videoCanvas;

    [Header("AR Elements")]
    public GameObject modelTarget;
    public VuforiaBehaviour vuforiaCamera; // Drag your ARCamera here

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
            // --- BACK TO AR MODE ---
            buttonText.text = "Turn AR Off";

            // Re-enable Vuforia Camera
            if (vuforiaCamera != null) vuforiaCamera.enabled = true;

            modelTarget.SetActive(true);
            videoCanvas.SetActive(false);
            videoPlayer.Stop();

            if (onVideoStop != null)
            {
                onVideoStop.Invoke();
            }
        }
        else
        {
            // --- SWITCH TO VIDEO MODE ---
            buttonText.text = "Turn AR On";

            // Disable Vuforia Camera to save battery/performance
            if (vuforiaCamera != null) vuforiaCamera.enabled = false;

            modelTarget.SetActive(false);
            videoCanvas.SetActive(true);
            videoPlayer.Play();

            if (onVideoStart != null)
            {
                onVideoStart.Invoke();
            }
        }
    }
}