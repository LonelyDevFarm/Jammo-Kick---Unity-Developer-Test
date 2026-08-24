using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public sealed class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField, Min(0f)] private float moveSpeed = 4f;
    [SerializeField, Min(0f)] private float rotationSpeed = 720f;

    [Header("Grounding")]
    [SerializeField, Min(0f)] private float gravity = 20f;
    [SerializeField] private float groundedVelocity = -2f;

    private CharacterController characterController;
    private float verticalVelocity;

    public bool IsMoving { get; private set; }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Vector2 input = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical"));

        Vector3 moveDirection = new Vector3(input.x, 0f, input.y);

        if (moveDirection.sqrMagnitude > 1f)
        {
            moveDirection.Normalize();
        }

        IsMoving = moveDirection.sqrMagnitude > 0.001f;

        if (IsMoving)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(moveDirection, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime);
        }

        UpdateVerticalVelocity();

        Vector3 velocity =
            moveDirection * moveSpeed +
            Vector3.up * verticalVelocity;

        characterController.Move(velocity * Time.deltaTime);
    }

    private void UpdateVerticalVelocity()
    {
        if (characterController.isGrounded &&
            verticalVelocity < 0f)
        {
            verticalVelocity = groundedVelocity;
            return;
        }

        verticalVelocity -= gravity * Time.deltaTime;
    }
}