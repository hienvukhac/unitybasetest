using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [Header("Parameters Name")]
    [SerializeField] private string speedParam = "Speed";
    [SerializeField] private string jumpParam = "Jump";
    [SerializeField] private string isGroundedParam = "IsGrounded";
    [SerializeField] private string deathParam = "Death";

    private int speedHash;
    private int jumpHash;
    private int isGroundedHash;
    private int deathHash;

    private PlayerMovement playerMovement;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        speedHash = Animator.StringToHash(speedParam);
        jumpHash = Animator.StringToHash(jumpParam);
        isGroundedHash = Animator.StringToHash(isGroundedParam);
        deathHash = Animator.StringToHash(deathParam);
    }

    private void OnEnable()
    {
        if (playerMovement != null)
        {
            playerMovement.OnJump += HandleJump;
            playerMovement.OnDeath += HandleDeath;
        }
    }

    private void OnDisable()
    {
        if (playerMovement != null)
        {
            playerMovement.OnJump -= HandleJump;
            playerMovement.OnDeath -= HandleDeath;
        }
    }

    private void Update()
    {
        if (animator == null || playerMovement.IsDead) return;

        UpdateMovementAnimations();
        animator.SetBool(isGroundedHash, playerMovement.IsGrounded);
    }

    private void UpdateMovementAnimations()
    {
        float targetSpeed = playerMovement.IsMoving ? 1f : 0f;
        animator.SetFloat(speedHash, targetSpeed);
    }

    private void HandleJump()
    {
        if (animator != null)
        {
            animator.SetTrigger(jumpHash);
        }
    }

    private void HandleDeath()
    {
        if (animator != null)
        {
            animator.SetTrigger(deathHash);
        }
    }
}
