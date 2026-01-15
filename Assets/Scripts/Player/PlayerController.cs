// NEW FILE
using UnityEngine;
using UnityEngine.InputSystem;

namespace FadeToGrey
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour, PlayerControls.IPlayerActions
    {
        #region Serialized Fields
        [SerializeField] private Rigidbody2D body;
        [SerializeField] private EnergySystem energySystem;

        [SerializeField] private float maxSpeed = 6f;
        [SerializeField] private float minSpeed = 2f;

        [SerializeField] private float acceleration = 15f;
        [SerializeField] private float deceleration = 18f;

        [SerializeField] private float inputSmoothTime = 0.08f;
        [SerializeField] private float lowEnergyInputSmoothTime = 0.25f;
        [SerializeField] private float lowEnergyThreshold = 20f;

        [SerializeField] private float inputDeadZone = 0.1f;
        #endregion

        #region Private Fields
        private PlayerControls controls;

        private Vector2 rawInput;
        private Vector2 smoothedInput;
        private Vector2 inputSmoothVelocity;
        private Vector2 currentVelocity;
        #endregion

        #region Properties
        public bool IsMoving => currentVelocity.sqrMagnitude > 0.0001f;
        #endregion

        #region Unity Lifecycle
        private void Reset()
        {
            body = GetComponent<Rigidbody2D>();
            if (body != null)
            {
                body.gravityScale = 0f;
            }
        }

        private void Awake()
        {
            if (body == null)
            {
                body = GetComponent<Rigidbody2D>();
            }

            if (body != null)
            {
                body.gravityScale = 0f;
                body.linearVelocity = Vector2.zero;
            }

            if (energySystem == null)
            {
                energySystem = FindFirstObjectByType<EnergySystem>();
            }

            controls = new PlayerControls();
            controls.Player.SetCallbacks(this);
            ClearInputState();
        }

        private void OnEnable()
        {
            controls?.Enable();
            ClearInputState();
            StopImmediately();
        }

        private void OnDisable()
        {
            controls?.Disable();
            ClearInputState();
            StopImmediately();
        }

        private void OnDestroy()
        {
            controls?.Dispose();
        }

        private void Update()
        {
            UpdateSmoothedInput();
        }

        private void FixedUpdate()
        {
            ApplyMovement();
        }
        #endregion

        #region Input System Callback
        public void OnMove(InputAction.CallbackContext context)
        {
            if (context.canceled)
            {
                rawInput = Vector2.zero;
                return;
            }

            rawInput = context.ReadValue<Vector2>();
            rawInput = Vector2.ClampMagnitude(rawInput, 1f);
        }
        #endregion

        #region Movement
        private void UpdateSmoothedInput()
        {
            rawInput = ApplyDeadZone(rawInput, inputDeadZone);

            if (rawInput == Vector2.zero)
            {
                smoothedInput = Vector2.zero;
                inputSmoothVelocity = Vector2.zero;
                return;
            }

            float smoothTime = GetInputSmoothTime();
            smoothedInput = Vector2.SmoothDamp(
                smoothedInput,
                rawInput,
                ref inputSmoothVelocity,
                smoothTime
            );
        }

        private void ApplyMovement()
        {
            if (body == null)
            {
                return;
            }

            if (smoothedInput == Vector2.zero)
            {
                currentVelocity = Vector2.zero;
                body.linearVelocity = Vector2.zero;
                return;
            }

            float normalizedEnergy = energySystem != null ? energySystem.NormalizedEnergy : 1f;
            float targetSpeed = Mathf.Lerp(minSpeed, maxSpeed, normalizedEnergy);
            Vector2 desiredVelocity = smoothedInput * targetSpeed;

            float accel = desiredVelocity.sqrMagnitude >= currentVelocity.sqrMagnitude
                ? acceleration
                : deceleration;

            currentVelocity = Vector2.MoveTowards(
                currentVelocity,
                desiredVelocity,
                accel * Time.fixedDeltaTime
            );

            body.linearVelocity = currentVelocity;

            if (energySystem != null)
            {
                energySystem.DrainForMovement(smoothedInput.magnitude, Time.fixedDeltaTime);
            }
        }
        #endregion

        #region Helpers
        private float GetInputSmoothTime()
        {
            float currentEnergy = energySystem != null ? energySystem.CurrentEnergy : 100f;
            return currentEnergy <= lowEnergyThreshold
                ? lowEnergyInputSmoothTime
                : inputSmoothTime;
        }

        private static Vector2 ApplyDeadZone(Vector2 value, float deadZone)
        {
            return value.sqrMagnitude <= deadZone * deadZone ? Vector2.zero : value;
        }

        private void ClearInputState()
        {
            rawInput = Vector2.zero;
            smoothedInput = Vector2.zero;
            inputSmoothVelocity = Vector2.zero;
            currentVelocity = Vector2.zero;
        }

        private void StopImmediately()
        {
            if (body != null)
            {
                body.linearVelocity = Vector2.zero;
            }
        }
        #endregion
    }
}
