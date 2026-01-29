using UnityEngine;

/// <summary>
/// StressOrb:
/// - Detects player
/// - Follows player while in range
/// - Drains energy
/// - Gets knocked back and dies when stabbed
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class StressOrb : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float detectionRadius = 3f;

    [Header("Movement")]
    [SerializeField] private float followSpeed = 3f;

    [Header("Energy Drain")]
    [SerializeField] private float drainPerSecond = 10f;

    [Header("Death / Knockback")]
    [SerializeField] private float knockbackSpeed = 6f;
    [SerializeField] private float destroyDelay = 0.15f;

    private Transform player;
    private EnergySystem playerEnergy;
    private Rigidbody2D rb;

    private bool isChasing;
    private bool isDying;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (!isChasing || player == null || isDying)
            return;

        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = dir * followSpeed;
    }

    private void Update()
    {
        if (isChasing && playerEnergy != null && !isDying)
        {
            playerEnergy.Drain(drainPerSecond * Time.deltaTime);
        }
    }

    // -------------------- DETECTION --------------------

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDying)
            return;

        if (other.CompareTag("Player"))
        {
            player = other.transform;
            playerEnergy = other.GetComponent<EnergySystem>();
            isChasing = true;
        }

        if (other.CompareTag("PlayerAttack"))
        {
            DieWithKnockback(other.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StopChasing();
        }
    }

    // -------------------- HELPERS --------------------

    private void StopChasing()
    {
        isChasing = false;
        player = null;
        playerEnergy = null;

        // 🔑 CRITICAL FIX: stop movement
        rb.linearVelocity = Vector2.zero;
    }

    // -------------------- DEATH --------------------

    private void DieWithKnockback(Transform attacker)
    {
        if (isDying)
            return;

        isDying = true;
        isChasing = false;

        player = null;
        playerEnergy = null;

        Vector2 knockDir = (transform.position - attacker.position).normalized;

        rb.linearVelocity = knockDir * knockbackSpeed;

        Destroy(gameObject, destroyDelay);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
#endif
}
