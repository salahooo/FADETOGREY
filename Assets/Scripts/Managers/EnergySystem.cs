using UnityEngine;
using System;

/// <summary>
/// Manages player energy using an exhaustion-based model.
/// - Energy drains while moving
/// - Recovery starts ONLY after depletion
/// - Recovery is paused if the player starts moving
/// </summary>
public class EnergySystem : MonoBehaviour
{
    // -------------------- SETTINGS --------------------

    [Header("Energy")]
    [SerializeField] private float maxEnergy = 100f;
    [SerializeField] private float startEnergy = 100f;

    [Header("Drain")]
    [SerializeField] private float passiveDrainPerSecond = 12f;

    [Header("Recovery")]
    [SerializeField] private float recoveryPerSecond = 15f;

    [Tooltip("Energy % required to regain movement")]
    [Range(0f, 1f)]
    [SerializeField] private float recoveryUnlockThreshold = 0.3f;

    // -------------------- STATE --------------------

    public float CurrentEnergy { get; private set; }
    public bool IsExhausted { get; private set; }

    public float MaxEnergy => maxEnergy;


    /// <summary>
    /// Set externally by PlayerController.
    /// Represents movement intent, NOT actual velocity.
    /// </summary>
    public bool IsMoving { get; set; }

    // -------------------- EVENTS --------------------

    public event Action<float> OnEnergyChanged;

    // -------------------- UNITY --------------------

    private void Awake()
    {
        CurrentEnergy = Mathf.Clamp(startEnergy, 0f, maxEnergy);
        IsExhausted = false;
        NotifyEnergyChanged();
    }

    private void Update()
    {
        HandleRecovery(Time.deltaTime);
    }

    // -------------------- DRAIN --------------------

    public void DrainOverTime(float deltaTime)
    {
        if (IsExhausted)
            return;

        Drain(passiveDrainPerSecond * deltaTime);
    }

    public void Drain(float amount)
    {
        if (amount <= 0f || IsExhausted)
            return;

        SetEnergy(CurrentEnergy - amount);

        if (CurrentEnergy <= 0f)
        {
            EnterExhaustion();
        }
    }

    // -------------------- RECOVERY --------------------

    private void HandleRecovery(float deltaTime)
    {
        // Recover ONLY if exhausted AND player is NOT moving
        if (!IsExhausted || IsMoving)
            return;

        SetEnergy(CurrentEnergy + recoveryPerSecond * deltaTime);

        // Unlock movement early
        if (NormalizedEnergy() >= recoveryUnlockThreshold)
        {
            IsExhausted = false;
        }

        // Clamp safety
        if (CurrentEnergy >= maxEnergy)
        {
            SetEnergy(maxEnergy);
        }
    }

    private void EnterExhaustion()
    {
        IsExhausted = true;
        SetEnergy(0f);
    }

    // -------------------- UTIL --------------------

    public float NormalizedEnergy()
    {
        return maxEnergy <= 0f ? 0f : CurrentEnergy / maxEnergy;
    }

    private void SetEnergy(float value)
    {
        float clamped = Mathf.Clamp(value, 0f, maxEnergy);

        if (Mathf.Approximately(clamped, CurrentEnergy))
            return;

        CurrentEnergy = clamped;
        NotifyEnergyChanged();
    }

    private void NotifyEnergyChanged()
    {
        OnEnergyChanged?.Invoke(CurrentEnergy);
    }


    public void Restore(float amount)
    {
        if (amount <= 0f)
            return;

        SetEnergy(CurrentEnergy + amount);
    }

}