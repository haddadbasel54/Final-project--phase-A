using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayAnimationScript : MonoBehaviour
{
    private Animator animator;

    [Header("Animator Parameter")]
    public string detectionBoolName;

    [Header("Entry State")]
    public string entryStateName;

    void Awake()
    {
        animator = GetComponent<Animator>();

        if (!string.IsNullOrEmpty(detectionBoolName))
        {
            animator.SetBool(detectionBoolName, false);
        }

        // Animation starts paused
        animator.speed = 0f;
    }

    // UPDATED: This is called by the GPS Button
    public void PlayAnimation()
    {
        if (animator == null) return;

        // 1. Thaw the animation speed
        animator.speed = 1f;

        // 2. Set the boolean to true (for transition logic)
        if (!string.IsNullOrEmpty(detectionBoolName))
        {
            animator.SetBool(detectionBoolName, true);
        }

        // 3. FORCE play the specific animation state immediately
        if (!string.IsNullOrEmpty(entryStateName))
        {
            animator.Play(entryStateName, 0, 0f);
        }

        Debug.Log("Animation started via GPS Button.");
    }

    public void StopAnimation()
    {
        if (animator == null) return;

        if (!string.IsNullOrEmpty(detectionBoolName))
        {
            animator.SetBool(detectionBoolName, false);
        }

        // Reset to beginning
        if (!string.IsNullOrEmpty(entryStateName))
        {
            animator.Play(entryStateName, 0, 0f);
            animator.Update(0f);
        }

        animator.speed = 0f;
    }
}