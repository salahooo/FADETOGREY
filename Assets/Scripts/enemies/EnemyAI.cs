using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    public Transform player;

    [Header("Movement")]
    public float speed = 3f;
    public float detectRange = 10f;

    [Header("Energy Damage")]
    public float energyDamage = 15f;
    public float attackCooldown = 1f;

    Rigidbody2D rb;
    EnergySystem playerEnergy;
    float nextAttackTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
    }

    void Start()
    {
        if (player != null)
        {
            playerEnergy = player.GetComponent<EnergySystem>();
        }
    }

    void FixedUpdate()
    {
        if (!player) return;

        float distance = Vector2.Distance(rb.position, player.position);

        if (distance > detectRange)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = dir * speed;
    }

    void OnTriggerStay2D(Collider2D other)
{
    if (!other.CompareTag("Player")) return;

    if (Time.time < nextAttackTime) return;

    EnergySystem energy = other.GetComponent<EnergySystem>();

    if (energy != null)
    {
        energy.Drain(energyDamage);
        nextAttackTime = Time.time + attackCooldown;
    }
}
}
