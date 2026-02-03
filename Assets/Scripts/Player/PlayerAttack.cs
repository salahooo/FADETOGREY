using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(PlayerAnimation))]
public class PlayerAttack : MonoBehaviour
{
    // Duration (in seconds) that the attack stays active
    [Header("Attack Settings")]
    [SerializeField] private float attackDuration = 0.25f;

    // Reference to the attack hitbox object
    [Header("References")]
    [SerializeField] private PlayerAttackHitbox hitbox;

    // Cached reference to the animation controller
    private PlayerAnimation playerAnimation;

    // Input system instance for handling attack input
    private PlayerControls controls;

    // Last valid direction used for attacks
    // Defaults to down/front
    private Vector2 lastAttackDir = Vector2.down;

    // Indicates whether the player is currently attacking
    public bool IsAttacking { get; private set; }

    private void Awake()
    {
        // Cache PlayerAnimation component
        playerAnimation = GetComponent<PlayerAnimation>();

        // Initialize input system and bind attack input
        controls = new PlayerControls();
        controls.Player.Attack.performed += _ => TryAttack();
    }

    // Enable input when the object becomes active
    private void OnEnable() => controls.Enable();

    // Disable input when the object becomes inactive
    private void OnDisable() => controls.Disable();

    // Called externally (from PlayerAnimation)
    // Updates the direction used for the next attack
    public void SetLastDirection(Vector2 dir)
    {
        if (dir.sqrMagnitude > 0.01f)
            lastAttackDir = dir.normalized;
    }

    // Attempts to start an attack
    // Prevents overlapping attacks
    private void TryAttack()
    {
        if (IsAttacking)
            return;

        StartCoroutine(AttackRoutine());
    }

    // Controls attack timing, animation, and hitbox activation
    private IEnumerator AttackRoutine()
    {
        IsAttacking = true;

        // Trigger stab animation
        playerAnimation.TriggerStab();

        // Activate hitbox in the last movement direction
        hitbox.Activate(lastAttackDir);

        // Wait for the attack duration
        yield return new WaitForSeconds(attackDuration);

        // Disable hitbox after attack window
        hitbox.Deactivate();

        IsAttacking = false;
    }
}
