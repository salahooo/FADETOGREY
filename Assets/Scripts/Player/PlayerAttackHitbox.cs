using UnityEngine;

public class PlayerAttackHitbox : MonoBehaviour
{
    // Size of the hitbox collider
    [Header("Hitbox Settings")]
    [SerializeField] private Vector2 size = new Vector2(0.6f, 0.6f);

    // Distance in front of the player where the hitbox appears
    [SerializeField] private float distance = 0.5f;

    // Reference to the BoxCollider2D used as the hitbox
    private BoxCollider2D box;

    // Last attack direction, defaults to down/front
    private Vector2 lastDir = Vector2.down;

    private void Awake()
    {
        // Cache the collider component
        box = GetComponent<BoxCollider2D>();

        // Apply configured hitbox size
        box.size = size;

        // Disable hitbox by default
        gameObject.SetActive(false);
    }

    // Called when an attack starts
    // Positions the hitbox in front of the player
    public void Activate(Vector2 direction)
    {
        lastDir = direction.normalized;

        // Move hitbox in front of the player based on direction
        transform.localPosition = lastDir * distance;

        // Enable hitbox so it can detect collisions
        gameObject.SetActive(true);
    }

    // Called when the attack ends
    // Disables the hitbox so it no longer detects collisions
    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    // Draws a visual representation of the hitbox in the editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, size);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collided object has an EnemyHealth component
        EnemyHealth enemy = other.GetComponent<EnemyHealth>();

        if (enemy != null)
        {
            // Apply a single hit of damage
            enemy.TakeDamage(1);
        }
    }
}
