using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles top-down player movement using Rigidbody2D.
/// Supports sprinting and integrates with the EnergySystem exhaustion model.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerAnimation))]
[RequireComponent(typeof(EnergySystem))]
public class PlayerController : MonoBehaviour
{
    // -------------------- MOVEMENT SETTINGS --------------------

    [Header("Movement")]
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float deceleration = 25f;
    [SerializeField] private float inputDeadzone = 0.1f;

    [Header("Sprinting")]
    [SerializeField] private float sprintSpeedMultiplier = 1.6f;
    [SerializeField] private float sprintDrainMultiplier = 2.5f;

    [Header("Energy Influence")]
    [SerializeField] private float minEnergySpeedMultiplier = 0.25f;

    
    // -------------------- COMPONENTS --------------------

    private Rigidbody2D rb;
    private PlayerAnimation playerAnimation;
    private EnergySystem energySystem;

    // -------------------- INPUT STATE --------------------

    private Vector2 rawInput;
    private Vector2 smoothedInput;
    private bool isSprinting;

    private PlayerControls controls;

    // -------------------- UNITY --------------------

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAnimation = GetComponent<PlayerAnimation>();
        energySystem = GetComponent<EnergySystem>();

        controls = new PlayerControls();

        controls.Player.Move.performed += OnMove;
        controls.Player.Move.canceled += _ => rawInput = Vector2.zero;

        controls.Player.Sprint.performed += _ => isSprinting = true;
        controls.Player.Sprint.canceled += _ => isSprinting = false;
    }

    

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void FixedUpdate()
    {
        // Report movement intent (used by EnergySystem recovery logic)
        energySystem.IsMoving = IsMovingIntent();

        if (energySystem.IsExhausted)
        {
            smoothedInput = Vector2.zero;
            rb.linearVelocity = Vector2.zero;
            playerAnimation.UpdateAnimation(Vector2.zero);
            return;
        }

        if (TryGetComponent<PlayerAttack>(out var attack) && attack.IsAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            playerAnimation.UpdateAnimation(Vector2.zero);
            return;
        }


        UpdateSmoothedInput();
        ApplyMovement();
        UpdateAnimation();
        DrainEnergyIfMoving();
    }

    // -------------------- INPUT --------------------

    private void OnMove(InputAction.CallbackContext context)
    {
        rawInput = context.ReadValue<Vector2>();

        if (rawInput.magnitude < inputDeadzone)
            rawInput = Vector2.zero;
    }

    // -------------------- MOVEMENT --------------------

    private void UpdateSmoothedInput()
    {
        float rate = rawInput.magnitude > 0f ? acceleration : deceleration;

        smoothedInput = Vector2.MoveTowards(
            smoothedInput,
            rawInput,
            rate * Time.fixedDeltaTime
        );
    }

    private void ApplyMovement()
    {
        float energyFactor = energySystem.NormalizedEnergy();

        if (energyFactor <= 0f)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float speedMultiplier = Mathf.Lerp(
            minEnergySpeedMultiplier,
            1f,
            energyFactor
        );

        float finalSpeed = maxSpeed * speedMultiplier;

        if (isSprinting && IsMovingIntent())
        {
            finalSpeed *= sprintSpeedMultiplier;
        }

        Vector2 targetVelocity = smoothedInput * finalSpeed;

        rb.linearVelocity = Vector2.MoveTowards(
            rb.linearVelocity,
            targetVelocity,
            acceleration * Time.fixedDeltaTime
        );
    }

    // -------------------- ANIMATION --------------------

    private void UpdateAnimation()
    {
        playerAnimation.UpdateAnimation(smoothedInput);
    }

    // -------------------- ENERGY --------------------

    private void DrainEnergyIfMoving()
    {
        if (!IsMovingIntent())
            return;

        float drainMultiplier = isSprinting ? sprintDrainMultiplier : 1f;
        energySystem.DrainOverTime(Time.fixedDeltaTime * drainMultiplier);
    }

    private bool IsMovingIntent()
    {
        return smoothedInput.sqrMagnitude > 0.01f;
    }
}
