using UnityEngine;
using Vuforia;

public class ModelTargetSmoother : MonoBehaviour
{
    public Transform content;               // Your AR content
    public float positionSmooth = 8f;       // Higher = smoother
    public float rotationSmooth = 8f;

    private ObserverBehaviour observer;
    private bool isTracked = false;

    private Vector3 targetPosition;
    private Quaternion targetRotation;

    void Awake()
    {
        observer = GetComponent<ObserverBehaviour>();
        observer.OnTargetStatusChanged += OnStatusChanged;
    }

    void OnDestroy()
    {
        if (observer != null)
            observer.OnTargetStatusChanged -= OnStatusChanged;
    }

    void Update()
    {
        if (!isTracked || content == null)
            return;

        // Smooth position
        content.position = Vector3.Lerp(
            content.position,
            targetPosition,
            Time.deltaTime * positionSmooth
        );

        // Smooth rotation
        content.rotation = Quaternion.Slerp(
            content.rotation,
            targetRotation,
            Time.deltaTime * rotationSmooth
        );
    }

    void LateUpdate()
    {
        if (!isTracked || content == null)
            return;

        // Read latest tracking pose
        targetPosition = transform.position;
        targetRotation = transform.rotation;
    }

    void OnStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        isTracked =
            status.Status == Status.TRACKED ||
            status.Status == Status.EXTENDED_TRACKED;
    }
}
