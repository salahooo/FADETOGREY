using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;

    private Vector2 moveInput;
    private Vector2 lastMoveDir;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        lastMoveDir = Vector2.down; // default facing front
    }

    /// <summary>
    /// Call this every frame from movement code
    /// </summary>
    public void UpdateAnimation(Vector2 movement)
    {
        moveInput = movement;

        float speed = movement.magnitude;

        animator.SetFloat("MoveX", movement.x);
        animator.SetFloat("MoveY", movement.y);
        animator.SetFloat("Speed", speed);

        if (speed > 0.01f)
        {
            lastMoveDir = movement.normalized;

            animator.SetFloat("LastX", lastMoveDir.x);
            animator.SetFloat("LastY", lastMoveDir.y);
        }
    }
}
