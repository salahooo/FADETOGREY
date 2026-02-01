using UnityEngine;
using System;

public class EnergySystem : MonoBehaviour
{
    // -------------------- SETTINGS --------------------

    [Header("Energy")]
    [SerializeField] private float maxEnergy = 100f;
    [SerializeField] private float startEnergy = 100f;

    [Header("Drain & Recovery")]
    [SerializeField] private float passiveDrainPerSecond = 10f; // Iets hoger gezet voor test
    [SerializeField] private float recoveryPerSecond = 15f;
    [Range(0f, 1f)]
    [SerializeField] private float recoveryUnlockThreshold = 0.3f;

    [Header("Debug / Testing")]
    [Tooltip("Vink dit aan om energie verlies te testen zonder te bewegen")]
    [SerializeField] private bool testAutoDrain = false; 

    // -------------------- STATE --------------------

    // [field: SerializeField] zorgt dat je deze waarde live in de inspector ziet!
    [field: SerializeField] public float CurrentEnergy { get; private set; }
    public bool IsExhausted { get; private set; }
    public float MaxEnergy => maxEnergy;

    public bool IsMoving { get; set; }

    // -------------------- EVENTS --------------------
    public event Action<float> OnEnergyChanged;

    // -------------------- UNITY --------------------

    private void Awake()
    {
        // Zet de energie op startwaarde bij begin
        CurrentEnergy = startEnergy;
        IsExhausted = false;
    }

    private void Update()
    {
        // TEST MODUS: Als het vinkje aanstaat, doen we alsof we bewegen
        if (testAutoDrain)
        {
            DrainOverTime(Time.deltaTime);
        }
        else
        {
            // NORMALE MODUS: Reageer op movement
            if (IsMoving)
            {
                DrainOverTime(Time.deltaTime);
            }
            else
            {
                HandleRecovery(Time.deltaTime);
            }
        }
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
        // Alleen recoveren als we NIET bewegen (en test modus uit staat)
        if (IsMoving || testAutoDrain) return;

        if (!IsExhausted && CurrentEnergy < maxEnergy)
        {
             // Normaal herstel
             SetEnergy(CurrentEnergy + (recoveryPerSecond * 0.5f) * deltaTime);
        }
        else if (IsExhausted)
        {
            // Herstel tijdens uitputting
            SetEnergy(CurrentEnergy + recoveryPerSecond * deltaTime);

            // Als we boven de drempel (bv 30%) zijn, mag je weer bewegen
            if (NormalizedEnergy() >= recoveryUnlockThreshold)
            {
                IsExhausted = false;
                Debug.Log("Energy hersteld - Je mag weer bewegen!");
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
    
    public void AddEnergy(float amount) {
        if (amount <= 0f) return;

        SetEnergy(CurrentEnergy + amount);
    }
}