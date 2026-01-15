using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D body;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (animator != null)
        {
            animator.applyRootMotion = false;
        }

        if (body == null)
        {
            body = GetComponent<Rigidbody2D>();
        }
    }

    private void Reset()
    {
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (animator == null || body == null)
        {
            return;
        }

        Vector2 velocity = body.linearVelocity;

        // Used by many 2D character packs including Hisa Student
        animator.SetFloat("MoveX", velocity.x);
        animator.SetFloat("MoveY", velocity.y);
        animator.SetFloat("Speed", velocity.sqrMagnitude);
        animator.SetBool("IsMoving", velocity.sqrMagnitude > 0.1f);
    }
}
