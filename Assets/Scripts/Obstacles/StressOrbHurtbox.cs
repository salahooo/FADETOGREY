using UnityEngine;

public class StressOrbHurtbox : MonoBehaviour
{
    private StressOrb parent;

    private void Awake()
    {
        parent = GetComponentInParent<StressOrb>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (parent == null)
            return;

        if (other.CompareTag("PlayerAttack"))
        {
            parent.DieWithKnockback(other.transform);
        }
    }
}
