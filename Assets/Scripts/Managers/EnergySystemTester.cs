// using UnityEngine;
// using System;

// public class EnergySystem : MonoBehaviour
// {
//     [Header("Energy Settings")]
//     [SerializeField] private float maxEnergy = 100f;
//     [SerializeField] private float startEnergy = 100f;

//     [Header("Drain Settings")]
//     [SerializeField] private float passiveDrainPerSecond = 1f;

//     public float CurrentEnergy { get; private set; }

//     public event Action<float> OnEnergyChanged;

//     // -------------------- SETUP --------------------

//     private void Awake()
//     {
//         CurrentEnergy = Mathf.Clamp(startEnergy, 0f, maxEnergy);
//         NotifyChange();
//     }

//     // -------------------- UPDATE --------------------

//     private void Update()
//     {
//         Drain(passiveDrainPerSecond * Time.deltaTime);
//     }

//     // -------------------- CORE API --------------------

//     public void Drain(float amount)
//     {
//         if (amount <= 0f || CurrentEnergy <= 0f)
//             return;

//         float previous = CurrentEnergy;

//         CurrentEnergy = Mathf.Clamp(CurrentEnergy - amount, 0f, maxEnergy);

//         if (!Mathf.Approximately(previous, CurrentEnergy))
//             NotifyChange();
//     }

//     public void Restore(float amount)
//     {
//         if (amount <= 0f || CurrentEnergy >= maxEnergy)
//             return;

//         float previous = CurrentEnergy;

//         CurrentEnergy = Mathf.Clamp(CurrentEnergy + amount, 0f, maxEnergy);

//         if (!Mathf.Approximately(previous, CurrentEnergy))
//             NotifyChange();
//     }

//     public float NormalizedEnergy()
//     {
//         return CurrentEnergy / maxEnergy;
//     }

//     // -------------------- INTERNAL --------------------

//     private void NotifyChange()
//     {
//         OnEnergyChanged?.Invoke(CurrentEnergy);
//     }
// }
