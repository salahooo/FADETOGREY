using UnityEngine;

namespace FadeToGrey
{
    /// <summary>
    /// Handles top-down 2D movement with Rigidbody2D physics, including energy-driven speed and input delay.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        #region Serialized Fields
        /// <summary>
        /// Rigidbody2D used to move the player with physics-aware velocity changes.
        /// </summary>
        [SerializeField] private Rigidbody2D body;

        /// <summary>
        /// Energy system that drives speed scaling and movement drain.
        /// </summary>
        [SerializeField] private EnergySystem energySystem;

        /// <summary>
        /// Maximum movement speed when fully energized.
        /// </summary>
        [SerializeField] private float maxSpeed = 6f;

        /// <summary>
        /// Minimum movement speed when fully exhausted.
        /// </summary>
        [SerializeField] private float minSpeed = 2f;

        /// <summary>
        /// Acceleration used when the player is actively moving.
        /// </summary>
        [SerializeField] private float acceleration = 15f;

        /// <summary>
        /// Deceleration applied when the player releases input.
        /// </summary>
        [SerializeField] private float deceleration = 18f;

        /// <summary>
        /// Standard input smoothing time for responsive controls.
        /// </summary>
        [SerializeField] private float inputSmoothTime = 0.08f;

        /// <summary>
        /// Slower input smoothing time that simulates delayed reactions at low energy.
        /// </summary>
        [SerializeField] private float lowEnergyInputSmoothTime = 0.25f;

        /// <summary>
        /// Energy value at or below this threshold triggers input delay behavior.
        /// </summary>
        [SerializeField] private float lowEnergyThreshold = 20f;

        /// <summary>
        /// Prevents tiny input noise from draining energy or causing drift.
        /// </summary>
        [SerializeField] private float movementDeadZone = 0.1f;
        #endregion

        #region Private Fields
        /// <summary>
        /// Raw input read from the player this frame.
        /// </summary>
        private Vector2 rawInput;

        /// <summary>
        /// Smoothed input used for movement and delayed response at low energy.
        /// </summary>
        private Vector2 smoothedInput;

        /// <summary>
        /// Velocity reference required by SmoothDamp for input smoothing.
        /// </summary>
        private Vector2 inputSmoothVelocity;

        /// <summary>
        /// Cached movement velocity to support smooth acceleration and deceleration.
        /// </summary>
        private Vector2 currentVelocity;
        #endregion

        #region Unity Callbacks
        /// <summary>
        /// Auto-assigns component references when the script is first added.
        /// </summary>
        private void Reset()
        {
            body = GetComponent<Rigidbody2D>();
        }

        /// <summary>
        /// Ensures all required references are ready before gameplay begins.
        /// </summary>
        private void Awake()
        {
            if (body == null)
            {
                body = GetComponent<Rigidbody2D>();
            }

            if (energySystem == null)
            {
                energySystem = FindObjectOfType<EnergySystem>();
            }
        }

        /// <summary>
        /// Reads input every frame and applies energy-based input delay.
        /// </summary>
        private void Update()
        {
            ReadInput();
            UpdateSmoothedInput();
        }

        /// <summary>
        /// Applies velocity changes in FixedUpdate for consistent physics behavior.
        /// </summary>
        private void FixedUpdate()
        {
            ApplyMovement();
        }
        #endregion

        #region Input Handling
        /// <summary>
        /// Reads raw axis input and clamps it for consistent top-down control.
        /// </summary>
        private void ReadInput()
        {
            // Raw input preserves quick direction changes before we apply delay.
            rawInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            rawInput = Vector2.ClampMagnitude(rawInput, 1f);
        }

        /// <summary>
        /// Applies smooth damping to input to simulate sluggish reaction time at low energy.
        /// </summary>
        private void UpdateSmoothedInput()
        {
            float currentEnergy = energySystem != null ? energySystem.CurrentEnergy : 100f;
            float smoothTime = currentEnergy <= lowEnergyThreshold ? lowEnergyInputSmoothTime : inputSmoothTime;

            // SmoothDamp creates a visible response delay, reinforcing the fatigue theme.
            smoothedInput = Vector2.SmoothDamp(smoothedInput, rawInput, ref inputSmoothVelocity, smoothTime);
        }
        #endregion

        #region Movement
        /// <summary>
        /// Calculates speed from energy and applies smooth acceleration and deceleration.
        /// </summary>
        private void ApplyMovement()
        {
            float normalizedEnergy = energySystem != null ? energySystem.NormalizedEnergy : 1f;
            float targetSpeed = Mathf.Lerp(minSpeed, maxSpeed, normalizedEnergy);

            // Lower energy reduces top speed, reinforcing the color-as-energy metaphor.
            Vector2 desiredVelocity = smoothedInput * targetSpeed;
            float accel = smoothedInput.sqrMagnitude > movementDeadZone * movementDeadZone ? acceleration : deceleration;

            // MoveTowards avoids overshoot while keeping motion responsive.
            currentVelocity = Vector2.MoveTowards(currentVelocity, desiredVelocity, accel * Time.fixedDeltaTime);
            body.linearVelocity = currentVelocity;

            if (energySystem != null && smoothedInput.sqrMagnitude > movementDeadZone * movementDeadZone)
            {
                // Drain energy only when the player is actively moving.
                energySystem.DrainForMovement(smoothedInput.magnitude, Time.fixedDeltaTime);
            }
        }
        #endregion
    }
}
