using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHP = 2;

    int currentHP;

    void Start()
{
    currentHP = maxHP;
}

    public void TakeDamage(int amount)
    {
        currentHP -= amount;

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

    public float GetHPPercent()
    {
        return (float)currentHP / maxHP;
    }
}
