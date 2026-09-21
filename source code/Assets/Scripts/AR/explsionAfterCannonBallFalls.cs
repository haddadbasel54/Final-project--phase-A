using System.Collections;
using UnityEngine;

public class DelayedExplosionWithCameraShake : MonoBehaviour
{
    [Header("Explosion")]
    public ParticleSystem explosion;

    [Header("Delay Settings")]
    public float delaySeconds = 15f;

    [Header("Loop Settings")]
    public bool loopExplosion = false;      // Checkbox for re-looping
    public float reWaitSeconds = 5f;        // Seconds to wait before next cycle

    [Header("Camera Shake")]
    public Transform arCamera;              // Vuforia AR Camera
    public float shakeMagnitude = 2f;

    private Coroutine delayCoroutine;
    private bool hasExploded = false;

    private Vector3 cameraOriginalPos;

    void Start()
    {
        if (arCamera != null)
            cameraOriginalPos = arCamera.localPosition;

        ResetExplosion();
    }

    // Call this when the model target is detected
    public void OnTargetDetected()
    {
        // FIX: If loopExplosion is true, we allow the code to proceed even if hasExploded is true
        if (hasExploded && !loopExplosion) return;

        if (delayCoroutine != null)
            StopCoroutine(delayCoroutine);

        delayCoroutine = StartCoroutine(DelayThenExplode());
    }

    // Call this when the model target is lost
    public void OnTargetLost()
    {
        if (delayCoroutine != null)
        {
            StopCoroutine(delayCoroutine);
            delayCoroutine = null;
        }

        hasExploded = false;
        ResetExplosion();
    }

    IEnumerator DelayThenExplode()
    {
        yield return new WaitForSeconds(delaySeconds);

        if (explosion != null)
        {
            explosion.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            explosion.Play();

            float explosionDuration = explosion.main.duration;
            // Wait for shake to finish before moving to the re-wait timer
            yield return StartCoroutine(CameraShake(explosionDuration));
        }

        hasExploded = true;

        while (loopExplosion)
        {
            yield return new WaitForSeconds(reWaitSeconds);
            if (explosion != null)
            {
                explosion.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                explosion.Play();

                float explosionDuration = explosion.main.duration;
                // Wait for shake to finish before moving to the re-wait timer
                yield return StartCoroutine(CameraShake(explosionDuration));
            }
        }
        delayCoroutine = null;
    }

    IEnumerator CameraShake(float duration)
    {
        if (arCamera == null)
            yield break;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            arCamera.localPosition =
                cameraOriginalPos + Random.insideUnitSphere * shakeMagnitude;

            elapsed += Time.deltaTime;
            yield return null;
        }

        arCamera.localPosition = cameraOriginalPos;
    }

    private void ResetExplosion()
    {
        if (explosion != null)
        {
            explosion.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        if (arCamera != null)
            arCamera.localPosition = cameraOriginalPos;
    }
}