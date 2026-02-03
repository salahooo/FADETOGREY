using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimation : MonoBehaviour
{
    // Reference to the Animator component
    private Animator animator;

    // Current movement input coming from the PlayerController
    private Vector2 moveInput;

    // Last non-zero movement direction
    // Used for idle facing direction and attack direction
    private Vector2 lastMoveDir;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        // Default facing direction is down/front
        lastMoveDir = Vector2.down;
    }

    // Called every frame by the movement system
    // Updates all movement-related animation parameters
    public void UpdateAnimation(Vector2 movement)
    {
        moveInput = movement;

        // Movement intensity used to switch between idle and walk
        float speed = movement.magnitude;

        // Send movement values to the Animator
        animator.SetFloat("MoveX", movement.x);
        animator.SetFloat("MoveY", movement.y);
        animator.SetFloat("Speed", speed);

        // Only update facing direction when the player is actually moving
        if (speed > 0.01f)
        {
            lastMoveDir = movement.normalized;

            animator.SetFloat("LastX", lastMoveDir.x);
            animator.SetFloat("LastY", lastMoveDir.y);
        }

        // Forward the last movement direction to the attack system
        // This allows attacks to use the correct facing direction
        GetComponent<PlayerAttack>()?.SetLastDirection(lastMoveDir);
    }

    // Triggers the stab animation
    // The actual hit logic is handled elsewhere
    public void TriggerStab()
    {
        animator.SetTrigger("Stab");
    }
}
