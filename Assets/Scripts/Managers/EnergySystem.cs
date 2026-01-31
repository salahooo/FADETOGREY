using UnityEngine;
using System;

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

    // Wordt gezet door je PlayerMovement script
    public bool IsMoving { get; set; }

    // -------------------- EVENTS --------------------
    public event Action<float> OnEnergyChanged;

    // -------------------- UNITY --------------------

    private void Awake()
    {
        CurrentEnergy = Mathf.Clamp(startEnergy, 0f, maxEnergy);
        IsExhausted = false;
    }

    private void Update()
    {
        // Voor testdoeleinden: Als je geen movement script hebt, zet dit aan om drain te testen:
        // DrainOverTime(Time.deltaTime); 
        
        HandleRecovery(Time.deltaTime);
    }

    // -------------------- LOGIC --------------------

    public void DrainOverTime(float deltaTime)
    {
        if (IsExhausted) return;
        Drain(passiveDrainPerSecond * deltaTime);
    }

    public void Drain(float amount)
    {
        if (amount <= 0f || IsExhausted) return;

        SetEnergy(CurrentEnergy - amount);

        if (CurrentEnergy <= 0f)
        {
            EnterExhaustion();
        }
    }

    private void HandleRecovery(float deltaTime)
    {
        if (!IsExhausted && !IsMoving && CurrentEnergy < maxEnergy)
        {
             SetEnergy(CurrentEnergy + (recoveryPerSecond * 0.5f) * deltaTime);
        }

        if (IsExhausted && !IsMoving)
        {
            SetEnergy(CurrentEnergy + recoveryPerSecond * deltaTime);

            if (NormalizedEnergy() >= recoveryUnlockThreshold)
            {
                IsExhausted = false;
            }
        }
    }

    private void EnterExhaustion()
    {
        IsExhausted = true;
        SetEnergy(0f);
    }

    public float NormalizedEnergy()
    {
        return maxEnergy <= 0f ? 0f : CurrentEnergy / maxEnergy;
    }

    public void SetEnergy(float value)
    {
        float clamped = Mathf.Clamp(value, 0f, maxEnergy);
        if (Mathf.Approximately(clamped, CurrentEnergy)) return;

        CurrentEnergy = clamped;
        OnEnergyChanged?.Invoke(CurrentEnergy);
    }
    
    public void addEnergy(float amount)
    {
        if (amount <= 0f) return;

        SetEnergy(CurrentEnergy + amount);
    }
}