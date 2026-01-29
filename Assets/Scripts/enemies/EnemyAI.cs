using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    public Transform player;

    [Header("Movement")]
    public float speed = 2f;
    public float detectRange = 10f;
    public float attackRange = 0.6f;

    [Header("Energy Damage")]
    public float energyDamage = 15f;
    public float attackCooldown = 1f;

    [Header("Knockback")]
    public float knockbackForce = 6f;

    Rigidbody2D rb;
    float nextAttackTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
    }

    void FixedUpdate()
    {
        if (!player) return;

        float distance = Vector2.Distance(rb.position, player.position);

        // Outside detection range → stop
        if (distance > detectRange)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Inside attack range → stop and attack via trigger
        if (distance <= attackRange)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Move toward player
        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = dir * speed;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (Time.time < nextAttackTime) return;

        EnergySystem energy = other.GetComponent<EnergySystem>();
        Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();

        if (energy != null && playerRb != null)
        {
            // Drain energy
            energy.Drain(energyDamage);

            // Knockback
            Vector2 knockDir = (other.transform.position - transform.position).normalized;
            playerRb.linearVelocity = knockDir * knockbackForce;

            nextAttackTime = Time.time + attackCooldown;
        }
    }
}
