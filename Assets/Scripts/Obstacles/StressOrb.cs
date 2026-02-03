using System;
using UnityEngine;

/*
 * StressOrb behaviour:
 * - Detects the player via trigger collision
 * - Starts following the player while inside detection range
 * - Continuously drains player energy while chasing
 * - Applies screen effects at intervals while draining
 * - Can be stabbed by the player, causing knockback and destruction
 */
[RequireComponent(typeof(Rigidbody2D))]
public class StressOrb : MonoBehaviour
{
    /* -------------------- DETECTION SETTINGS -------------------- */

    // Radius used only for visual debugging (actual detection is via trigger collider)
    [Header("Detection")]
    [SerializeField] private float detectionRadius = 3f;

    /* -------------------- MOVEMENT SETTINGS -------------------- */

    // Speed at which the orb follows the player
    [Header("Movement")]
    [SerializeField] private float followSpeed = 3f;

    /* -------------------- ENERGY DRAIN -------------------- */

    // Amount of energy drained from the player per second
    [Header("Energy Drain")]
    [SerializeField] private float drainPerSecond = 10f;

    /* -------------------- VISUAL / FEEDBACK EFFECTS -------------------- */

    // Interval between screen damage / shake effects while draining
    [Header("Effects")]
    [SerializeField] private float shakeInterval = 1.0f;

    // Internal timer for effect triggering
    private float shakeTimer = 0f;

    /* -------------------- DEATH / KNOCKBACK -------------------- */

    // Speed applied to the orb when knocked back by a stab
    [Header("Death / Knockback")]
    [SerializeField] private float knockbackSpeed = 6f;

    // Delay before the orb is destroyed after being stabbed
    [SerializeField] private float destroyDelay = 0.15f;

    /* -------------------- RUNTIME REFERENCES -------------------- */

    // Reference to the player transform while chasing
    private Transform player;

    // Reference to the player's EnergySystem
    private EnergySystem playerEnergy;

    // Rigidbody used for movement
    private Rigidbody2D rb;

    /* -------------------- STATE FLAGS -------------------- */

    // True while the orb is actively chasing the player
    private bool isChasing;

    // True once the orb has been stabbed and is dying
    private bool isDying;

    private void Awake()
    {
        // Cache Rigidbody reference
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // Register this orb with the EnemyManager for tracking
        EnemyManager.Instance.RegisterEnemy();
    }

    private void FixedUpdate()
    {
        // Do nothing if not chasing, player is missing, or orb is dying
        if (!isChasing || player == null || isDying)
            return;

        // Move directly toward the player
        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = dir * followSpeed;
    }

    private void Update()
    {
        // While chasing and alive, drain energy and apply periodic effects
        if (isChasing && playerEnergy != null && !isDying)
        {
            // Drain player energy over time
            playerEnergy.Drain(drainPerSecond * Time.deltaTime);

            // Update effect timer
            shakeTimer -= Time.deltaTime;

            // Trigger damage effect at intervals
            if (shakeTimer <= 0f)
            {
                EffectsController.Instance.AddDamageEffect();
                shakeTimer = shakeInterval;
            }
        }
        else
        {
            // Reset timer when not actively draining
            shakeTimer = 0f;
        }
    }

    /* -------------------- DETECTION -------------------- */

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore collisions once dying
        if (isDying)
            return;

        // Player entered detection area
        if (other.CompareTag("Player"))
        {
            player = other.transform;
            playerEnergy = other.GetComponent<EnergySystem>();
            isChasing = true;
        }

        // Player attack hitbox collided with the orb
        if (other.CompareTag("PlayerAttack"))
        {
            DieWithKnockback(other.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Player left detection area
        if (other.CompareTag("Player"))
        {
            StopChasing();
        }
    }

    /* -------------------- HELPERS -------------------- */

    private void StopChasing()
    {
        // Stop following the player
        isChasing = false;
        player = null;
        playerEnergy = null;

        // Immediately stop movement
        rb.linearVelocity = Vector2.zero;
    }

    /* -------------------- DEATH -------------------- */

    private void DieWithKnockback(Transform attacker)
    {
        // Prevent multiple death triggers
        if (isDying)
            return;

        isDying = true;
        isChasing = false;

        // Clear references
        player = null;
        playerEnergy = null;

        // Calculate knockback direction away from attacker
        Vector2 knockDir = (transform.position - attacker.position).normalized;

        // Apply knockback velocity
        rb.linearVelocity = knockDir * knockbackSpeed;

        // Destroy orb after short delay to allow knockback to be visible
        Destroy(gameObject, destroyDelay);
    }

    private void OnDestroy()
    {
        // Notify EnemyManager that this orb has been defeated
        EnemyManager.Instance.EnemyDefeated();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // Visualize detection radius in editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
#endif
}
