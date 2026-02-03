using UnityEngine;
using System;

public class EnergySystem : MonoBehaviour
{
    // -------------------- SETTINGS --------------------

    // Maximum and starting energy values for the player
    [Header("Energy")]
    [SerializeField] private float maxEnergy = 100f;
    [SerializeField] private float startEnergy = 100f;

    // Energy drain and recovery tuning
    [Header("Drain & Recovery")]
    [SerializeField] private float passiveDrainPerSecond = 10f; // Energy lost per second while moving
    [SerializeField] private float recoveryPerSecond = 15f;     // Energy restored per second while resting

    // Percentage of max energy required to exit exhaustion state
    [Range(0f, 1f)]
    [SerializeField] private float recoveryUnlockThreshold = 0.3f;

    // Debug option to force energy drain without player movement
    [Header("Debug / Testing")]
    [Tooltip("Enable to simulate energy drain without moving")]
    [SerializeField] private bool testAutoDrain = false; 

    // -------------------- STATE --------------------

    // Current energy value (visible in Inspector but not editable)
    [field: SerializeField]
    public float CurrentEnergy { get; private set; }

    // Indicates whether the player is fully exhausted
    public bool IsExhausted { get; private set; }

    // Public read-only access to maximum energy
    public float MaxEnergy => maxEnergy;

    // Set externally by PlayerController to indicate movement intent
    public bool IsMoving { get; set; }

    // -------------------- EVENTS --------------------

    // Fired whenever the energy value changes
    public event Action<float> OnEnergyChanged;

    // -------------------- UNITY --------------------

    private void Awake()
    {
        // Initialize energy and exhaustion state on start
        CurrentEnergy = startEnergy;
        IsExhausted = false;
    }

    private void Update()
    {
        // Debug mode: always drain energy regardless of movement
        if (testAutoDrain)
        {
            DrainOverTime(Time.deltaTime);
        }
        else
        {
            // Normal gameplay behavior
            if (IsMoving)
            {
                // Drain energy while the player is moving
                DrainOverTime(Time.deltaTime);
            }
            else
            {
                // Recover energy while idle
                HandleRecovery(Time.deltaTime);
            }
        }
    }

    // -------------------- ENERGY LOGIC --------------------

    // Applies continuous energy drain based on time
    public void DrainOverTime(float deltaTime)
    {
        if (IsExhausted)
            return;

        Drain(passiveDrainPerSecond * deltaTime);
    }

    // Reduces energy by a fixed amount
    public void Drain(float amount)
    {
        if (amount <= 0f || IsExhausted)
            return;

        SetEnergy(CurrentEnergy - amount);

        // Enter exhaustion state when energy reaches zero
        if (CurrentEnergy <= 0f)
        {
            EnterExhaustion();
        }
    }

    // Handles energy recovery when the player is not moving
    private void HandleRecovery(float deltaTime)
    {
        // Do not recover while moving or during forced test drain
        if (IsMoving || testAutoDrain)
            return;

        if (!IsExhausted && CurrentEnergy < maxEnergy)
        {
            // Normal passive recovery while idle
            SetEnergy(CurrentEnergy + (recoveryPerSecond * 0.5f) * deltaTime);
        }
        else if (IsExhausted)
        {
            // Faster recovery during exhaustion
            SetEnergy(CurrentEnergy + recoveryPerSecond * deltaTime);

            // Allow movement again once recovery threshold is reached
            if (NormalizedEnergy() >= recoveryUnlockThreshold)
            {
                IsExhausted = false;
                Debug.Log("Energy recovered - movement unlocked");
            }
        }
    }

    // Forces the player into exhaustion state
    private void EnterExhaustion()
    {
        IsExhausted = true;
        SetEnergy(0f);
    }

    // Returns energy as a 0–1 normalized value
    public float NormalizedEnergy()
    {
        return maxEnergy <= 0f ? 0f : CurrentEnergy / maxEnergy;
    }

    // Sets energy safely and triggers change event
    public void SetEnergy(float value)
    {
        float clamped = Mathf.Clamp(value, 0f, maxEnergy);

        if (Mathf.Approximately(clamped, CurrentEnergy))
            return;

        CurrentEnergy = clamped;
        OnEnergyChanged?.Invoke(CurrentEnergy);
    }

    // Adds energy (used by pickups like EnergyOrb)
    public void AddEnergy(float amount)
    {
        if (amount <= 0f)
            return;

        SetEnergy(CurrentEnergy + amount);
    }
}
