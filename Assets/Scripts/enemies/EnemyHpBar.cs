using UnityEngine;

public class EnemyHPBar : MonoBehaviour
{
    EnemyHealth health;
    [SerializeField] Transform bar;

    void Awake()
    {
        health = GetComponentInParent<EnemyHealth>();
    }

    void Update()
    {
        if (health == null || bar == null) return;

        float pct = health.GetHPPercent();
        bar.localScale = new Vector3(pct, 1f, 1f);
    }
}
