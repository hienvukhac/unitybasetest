using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Liên Kết Camera (Rõ Ràng - Dependency Injection)")]
    [Tooltip("Kéo Camera có gắn ThirdPersonCamera vào đây")]
    [SerializeField] private ThirdPersonCamera playerCamera;

    [Header("Cài đặt di chuyển")]
    [SerializeField] private float moveSpeed = 3f;       
    [SerializeField] private float rotationSpeed = 12f;    

    [Header("Cài đặt nhảy và trọng lực")]
    [SerializeField] private float jumpHeight = 0.95f;       
    [SerializeField] private float gravityValue = -20f;   

    [Header("Game Feel Nâng Cao")]
    [Tooltip("Thời gian nhớ phím Space trước khi chạm đất (giây)")]
    [SerializeField] private float jumpBufferTime = 0.15f;
    [Tooltip("Thời gian ân hạn cho phép nhảy sau khi vừa trượt khỏi mép (giây)")]
    [SerializeField] private float coyoteTime = 0.12f;

    public event Action OnJump;
    public event Action OnDeath;

    private CharacterController characterController;
    private PlayerInput playerInput;

    private float verticalVelocity;
    private float normalizedMoveSpeed;
    private float jumpBufferCounter;
    private float coyoteTimeCounter;
    private bool isDead = false;

    private float groundCheckDistance = 0.25f;
    private LayerMask groundLayer = ~0;
    private bool isGrounded;

    public float NormalizedMoveSpeed => normalizedMoveSpeed;
    public bool IsMoving => playerInput != null && playerInput.MoveInputValue.sqrMagnitude > 0.01f && !isDead;
    public bool IsGrounded => isGrounded;
    public bool IsDead => isDead;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        if (playerCamera == null)
        {
            playerCamera = FindObjectOfType<ThirdPersonCamera>();
        }
    }

    private void Update()
    {
        if (isDead) return;

        Vector3 moveDirection = CalculateMoveDirection();

        RotateTowardsMoveDirection(moveDirection);

        ApplyJumpAndGravity();

        MoveCharacter(moveDirection);

        normalizedMoveSpeed = moveDirection.magnitude;
    }

    private Vector3 CalculateMoveDirection()
    {
        Vector2 moveInput = playerInput.MoveInputValue;

        if (playerCamera != null)
        {
            Vector3 camForward = playerCamera.PlanarForward;
            Vector3 camRight = playerCamera.PlanarRight;

            return (camForward * moveInput.y + camRight * moveInput.x);
        }

        return new Vector3(moveInput.x, 0f, moveInput.y);
    }

    private void RotateTowardsMoveDirection(Vector3 moveDirection)
    {
        if (moveDirection.sqrMagnitude <= 0.001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private void CheckGroundStatus()
    {
        bool controllerGrounded = characterController.isGrounded;
        Vector3 rayStart = transform.position + (Vector3.up * 0.1f);
        bool raycastGrounded = Physics.Raycast(rayStart, Vector3.down, groundCheckDistance + 0.1f, groundLayer, QueryTriggerInteraction.Ignore);
        isGrounded = controllerGrounded || raycastGrounded;
    }

    private void ApplyJumpAndGravity()
    {
        CheckGroundStatus();

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (playerInput.IsJumpButtonPressed)
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravityValue);
            isGrounded = false;
            coyoteTimeCounter = 0f;
            jumpBufferCounter = 0f;

            OnJump?.Invoke();
        }

        verticalVelocity += gravityValue * Time.deltaTime;
    }

    private void MoveCharacter(Vector3 moveDirection)
    {
        Vector3 finalVelocity = moveDirection * moveSpeed;
        finalVelocity.y = verticalVelocity;

        characterController.Move(finalVelocity * Time.deltaTime);

        if ((characterController.collisionFlags & CollisionFlags.Above) != 0 && verticalVelocity > 0f)
        {
            verticalVelocity = -2f;
        }
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        characterController.enabled = false;
        enabled = false;
        OnDeath?.Invoke();
    }
}
