using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayAnimationScript : MonoBehaviour
{
    private Animator animator;

    [Header("Animator Parameter")]
    [Tooltip("Bool parameter name used in this Animator")]
    public string detectionBoolName;

    [Header("Entry State")]
    [Tooltip("Exact name of the first state after Entry")]
    public string entryStateName;

    void Awake()
    {
        animator = GetComponent<Animator>();

        if (!string.IsNullOrEmpty(detectionBoolName))
        {
            animator.SetBool(detectionBoolName, false);
        }

        // Pause animation until detected
        animator.speed = 0f;
    }

    // Call this when target is detected
    public void PlayAnimation()
    {
        if (animator == null || string.IsNullOrEmpty(detectionBoolName))
            return;

        animator.speed = 1f;
        animator.SetBool(detectionBoolName, true);
    }

    // Call this when target is lost
    public void StopAnimation()
    {
        if (animator == null || string.IsNullOrEmpty(detectionBoolName))
            return;

        animator.SetBool(detectionBoolName, false);

        // 🔥 RESET TO ENTRY STATE
        if (!string.IsNullOrEmpty(entryStateName))
        {
            animator.Play(entryStateName, 0, 0f);
            animator.Update(0f); // force immediate reset
        }

        animator.speed = 0f;
    }
}
