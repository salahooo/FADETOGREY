using UnityEngine;

/// <summary>
/// Controls the stab hitbox position and activation.
/// No damage logic here.
/// </summary>
public class PlayerAttackHitbox : MonoBehaviour
{
    [Header("Hitbox Settings")]
    [SerializeField] private Vector2 size = new Vector2(0.6f, 0.6f);
    [SerializeField] private float distance = 0.5f;

    private BoxCollider2D box;
    private Vector2 lastDir = Vector2.down;

    private void Awake()
    {
        box = GetComponent<BoxCollider2D>();
        box.size = size;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Called when attack starts
    /// </summary>
    public void Activate(Vector2 direction)
    {
        lastDir = direction.normalized;
        transform.localPosition = lastDir * distance;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Called when attack ends
    /// </summary>
    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    // DEBUG VISUAL
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, size);
    }
}
