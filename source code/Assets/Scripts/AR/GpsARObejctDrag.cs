
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class ARObjectDrag : MonoBehaviour
{
    [Header("AR Camera")]
    public Camera arCamera;

    [Header("Movement")]
    public float dragDepth = 3f;

    [Header("Pinch Zoom")]
    public float zoomSensitivity = 0.005f;
    public float minZoom = 0.3f;
    public float maxZoom = 3f;

    private Vector3 originalScale;
    private Vector3 dragOffset;
    private bool dragging;

    void Awake()
    {
        originalScale = transform.localScale;
    }

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    void Start()
    {
        if (arCamera == null)
            arCamera = Camera.main;
    }

    void Update()
    {
        if (arCamera == null)
            return;

        var touches = Touch.activeTouches;

        if (touches.Count == 1)
        {
            var touch = touches[0];

            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                Vector3 screenPoint = new Vector3(
                    touch.screenPosition.x,
                    touch.screenPosition.y,
                    dragDepth
                );

                Vector3 worldPoint =
                    arCamera.ScreenToWorldPoint(screenPoint);

                dragOffset = transform.position - worldPoint;
                dragging = true;
            }

            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved
                && dragging)
            {
                Vector3 screenPoint = new Vector3(
                    touch.screenPosition.x,
                    touch.screenPosition.y,
                    dragDepth
                );

                transform.position =
                    arCamera.ScreenToWorldPoint(screenPoint) + dragOffset;
            }
        }
        else
        {
            dragging = false;
        }

        if (touches.Count == 2)
        {
            var touch1 = touches[0];
            var touch2 = touches[1];

            Vector2 previous1 =
                touch1.screenPosition - touch1.delta;

            Vector2 previous2 =
                touch2.screenPosition - touch2.delta;

            float previousDistance =
                Vector2.Distance(previous1, previous2);

            float currentDistance =
                Vector2.Distance(
                    touch1.screenPosition,
                    touch2.screenPosition
                );

            float difference = currentDistance - previousDistance;

            float zoomFactor = transform.localScale.x /
                               originalScale.x;

            zoomFactor += difference * zoomSensitivity;
            zoomFactor = Mathf.Clamp(zoomFactor, minZoom, maxZoom);

            transform.localScale = originalScale * zoomFactor;
        }
    }
}
