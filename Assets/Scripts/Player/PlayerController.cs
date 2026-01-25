using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles top-down player movement using Rigidbody2D.
/// Movement speed is affected by the player's current energy.
/// Fully respects exhaustion state.
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

    [Header("Energy Influence")]
    [SerializeField] private float minEnergySpeedMultiplier = 0.25f;

    // -------------------- COMPONENT REFERENCES --------------------

    private Rigidbody2D rb;
    private PlayerAnimation playerAnimation;
    private EnergySystem energySystem;

    // -------------------- INPUT STATE --------------------

    private Vector2 rawInput;
    private Vector2 smoothedInput;

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
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void FixedUpdate()
    {
        // Report movement intent to EnergySystem
        energySystem.IsMoving = IsMovingIntent();

        if (energySystem.IsExhausted)
        {
            smoothedInput = Vector2.zero;
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

        Vector2 targetVelocity = smoothedInput * maxSpeed * speedMultiplier;

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
        if (IsMovingIntent())
        {
            energySystem.DrainOverTime(Time.fixedDeltaTime);
        }
    }

    private bool IsMovingIntent()
    {
        return smoothedInput.sqrMagnitude > 0.01f;
    }
}
