using UnityEngine;

public class SceneOrientationController : MonoBehaviour
{
    public enum OrientationMode
    {
        Portrait,
        Landscape
    }

    [Header("Scene Orientation")]
    public OrientationMode orientation = OrientationMode.Portrait;

    private void Awake()
    {
        ApplyOrientation();
    }

    private void ApplyOrientation()
    {
        switch (orientation)
        {
            case OrientationMode.Portrait:
                Screen.autorotateToPortrait = true;
                Screen.autorotateToPortraitUpsideDown = true;
                Screen.autorotateToLandscapeLeft = false;
                Screen.autorotateToLandscapeRight = false;
                Screen.orientation = ScreenOrientation.Portrait;
                break;

            case OrientationMode.Landscape:
                // Choose one landscape direction as your “main” one.
                Screen.autorotateToPortrait = false;
                Screen.autorotateToPortraitUpsideDown = false;
                Screen.autorotateToLandscapeLeft = true;
                Screen.autorotateToLandscapeRight = true;
                Screen.orientation = ScreenOrientation.LandscapeLeft;
                break;
        }
    }
}
