using UnityEngine;

public sealed class PlayerAnimationController : MonoBehaviour
{
    private static readonly int BlendHash =
        Animator.StringToHash("Blend");

    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Animator animator;
    [SerializeField, Min(0f)] private float transitionDuration = 0.1f;

    private const float IdleBlend = 0f;
    private const float RunBlend = 0.6f;

    private void Update()
    {
        float targetBlend =
            playerMovement.IsMoving ? RunBlend : IdleBlend;

        animator.SetFloat(
            BlendHash,
            targetBlend,
            transitionDuration,
            Time.deltaTime);
    }
}