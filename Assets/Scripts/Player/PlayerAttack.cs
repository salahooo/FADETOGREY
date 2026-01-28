using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(PlayerAnimation))]
public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackDuration = 0.25f;

    [Header("References")]
    [SerializeField] private PlayerAttackHitbox hitbox;

    private PlayerAnimation playerAnimation;
    private PlayerControls controls;

    private Vector2 lastAttackDir = Vector2.down;
    public bool IsAttacking { get; private set; }

    private void Awake()
    {
        playerAnimation = GetComponent<PlayerAnimation>();

        controls = new PlayerControls();
        controls.Player.Attack.performed += _ => TryAttack();
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    public void SetLastDirection(Vector2 dir)
    {
        if (dir.sqrMagnitude > 0.01f)
            lastAttackDir = dir.normalized;
    }

    private void TryAttack()
    {
        if (IsAttacking)
            return;

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        IsAttacking = true;

        playerAnimation.TriggerStab();
        hitbox.Activate(lastAttackDir);

        yield return new WaitForSeconds(attackDuration);

        hitbox.Deactivate();
        IsAttacking = false;
    }
}
