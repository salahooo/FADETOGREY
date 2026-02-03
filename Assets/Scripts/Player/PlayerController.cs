using UnityEngine;
using UnityEngine.InputSystem;

// Handles top-down movement for the player using Rigidbody2D.
// Movement speed and responsiveness are influenced by energy level.
// Supports sprinting and respects exhaustion and attack states.
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerAnimation))]
[RequireComponent(typeof(EnergySystem))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float maxSpeed = 5f;          // Base walking speed
    [SerializeField] private float acceleration = 20f;     // How fast the player accelerates
    [SerializeField] private float deceleration = 25f;     // How fast the player slows down
    [SerializeField] private float inputDeadzone = 0.1f;   // Ignores very small input values

    [Header("Sprinting")]
    [SerializeField] private float sprintSpeedMultiplier = 1.6f; // Extra speed while sprinting
    [SerializeField] private float sprintDrainMultiplier = 2.5f; // Extra energy drain while sprinting

    [Header("Energy Influence")]
    [SerializeField] private float minEnergySpeedMultiplier = 0.25f; // Minimum speed at very low energy

    // Core components
    private Rigidbody2D rb;
    private PlayerAnimation playerAnimation;
    private EnergySystem energySystem;

    // Input state
    private Vector2 rawInput;        // Direct input from the player
    private Vector2 smoothedInput;   // Smoothed input for weighty movement
    private bool isSprinting;        // Whether the sprint button is held

    private PlayerControls controls;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAnimation = GetComponent<PlayerAnimation>();
        energySystem = GetComponent<EnergySystem>();

        controls = new PlayerControls();

        // Movement input
        controls.Player.Move.performed += OnMove;
        controls.Player.Move.canceled += _ => rawInput = Vector2.zero;

        // Sprint input
        controls.Player.Sprint.performed += _ => isSprinting = true;
        controls.Player.Sprint.canceled += _ => isSprinting = false;
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void FixedUpdate()
    {
        // Inform the EnergySystem whether the player is trying to move
        energySystem.IsMoving = IsMovingIntent();

        // Fully stop movement if the player is exhausted
        if (energySystem.IsExhausted)
        {
            smoothedInput = Vector2.zero;
            rb.linearVelocity = Vector2.zero;
            playerAnimation.UpdateAnimation(Vector2.zero);
            return;
        }

        // Prevent movement while attacking
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

    // Reads movement input from the Input System
    private void OnMove(InputAction.CallbackContext context)
    {
        rawInput = context.ReadValue<Vector2>();

        // Ignore very small accidental input
        if (rawInput.magnitude < inputDeadzone)
            rawInput = Vector2.zero;
    }

    // Gradually moves input toward the target direction for smooth control
    private void UpdateSmoothedInput()
    {
        float rate = rawInput.magnitude > 0f ? acceleration : deceleration;

        smoothedInput = Vector2.MoveTowards(
            smoothedInput,
            rawInput,
            rate * Time.fixedDeltaTime
        );
    }

    // Applies velocity to the Rigidbody based on energy and sprinting state
    private void ApplyMovement()
    {
        float energyFactor = energySystem.NormalizedEnergy();

        // No movement if energy is empty
        if (energyFactor <= 0f)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Scale movement speed based on energy level
        float speedMultiplier = Mathf.Lerp(
            minEnergySpeedMultiplier,
            1f,
            energyFactor
        );

        float finalSpeed = maxSpeed * speedMultiplier;

        // Apply sprint bonus if sprinting and moving
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

    // Sends movement data to the animation system
    private void UpdateAnimation()
    {
        playerAnimation.UpdateAnimation(smoothedInput);
    }

    // Drains energy while the player is moving
    private void DrainEnergyIfMoving()
    {
        if (!IsMovingIntent())
            return;

        float drainMultiplier = isSprinting ? sprintDrainMultiplier : 1f;
        energySystem.DrainOverTime(Time.fixedDeltaTime * drainMultiplier);
    }

    // Returns true if the player intends to move
    private bool IsMovingIntent()
    {
        return smoothedInput.sqrMagnitude > 0.01f;
    }
}
