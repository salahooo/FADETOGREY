// NEW FILE
using System;
using UnityEngine;

namespace FadeToGrey
{
    /// <summary>
    /// Core energy model that drives movement speed, post-processing saturation, and audio filtering.
    /// </summary>
    public class EnergySystem : MonoBehaviour
    {
        #region Serialized Fields
        /// <summary>
        /// Maximum energy value. Default is 100 to match the design specification.
        /// </summary>
        [SerializeField] private float maxEnergy = 100f;

        /// <summary>
        /// Energy value assigned at startup.
        /// </summary>
        [SerializeField] private float startingEnergy = 100f;

        /// <summary>
        /// Base energy drain per second while moving.
        /// </summary>
        [SerializeField] private float movementDrainPerSecond = 6f;

        /// <summary>
        /// Multiplier applied when obstacles deal damage.
        /// </summary>
        [SerializeField] private float obstacleDamageMultiplier = 1f;

        /// <summary>
        /// Multiplier applied to energy restoration pickups.
        /// </summary>
        [SerializeField] private float pickupRestoreMultiplier = 1f;
        #endregion

        #region Events
        /// <summary>
        /// Raised whenever the energy value changes.
        /// </summary>
        public event Action<float> OnEnergyChanged;
        #endregion

        #region Properties
        /// <summary>
        /// Current energy value, clamped between 0 and the maximum.
        /// </summary>
        public float CurrentEnergy => currentEnergy;

        /// <summary>
        /// Maximum possible energy value.
        /// </summary>
        public float MaxEnergy => maxEnergy;

        /// <summary>
        /// Current energy as a 0-1 normalized value.
        /// </summary>
        public float NormalizedEnergy => maxEnergy <= 0f ? 0f : currentEnergy / maxEnergy;
        #endregion

        #region Private Fields
        /// <summary>
        /// Backing field for the current energy.
        /// </summary>
        private float currentEnergy;
        #endregion

        #region Unity Callbacks
        /// <summary>
        /// Initializes the energy value before gameplay begins.
        /// </summary>
        private void Awake()
        {
            currentEnergy = Mathf.Clamp(startingEnergy, 0f, maxEnergy);
        }

        /// <summary>
        /// Broadcasts the initial energy value once the scene starts.
        /// </summary>
        private void Start()
        {
            RaiseEnergyChanged();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Drains energy based on movement intensity and elapsed time.
        /// </summary>
        /// <param name="movementMagnitude">Magnitude of movement input from 0-1.</param>
        /// <param name="deltaTime">Time step used to scale drain.</param>
        public void DrainForMovement(float movementMagnitude, float deltaTime)
        {
            if (movementMagnitude <= 0f || deltaTime <= 0f)
            {
                return;
            }

            float amount = movementDrainPerSecond * movementMagnitude * deltaTime;
            ModifyEnergy(-amount);
        }

        /// <summary>
        /// Applies additional drain when the player collides with an obstacle.
        /// </summary>
        /// <param name="damage">Base damage value supplied by the obstacle.</param>
        public void ApplyObstacleDamage(float damage)
        {
            if (damage <= 0f)
            {
                return;
            }

            float amount = damage * obstacleDamageMultiplier;
            ModifyEnergy(-amount);
        }

        /// <summary>
        /// Restores energy when the player collects a pickup.
        /// </summary>
        /// <param name="amount">Base amount to restore.</param>
        public void RestoreEnergy(float amount)
        {
            if (amount <= 0f)
            {
                return;
            }

            float scaledAmount = amount * pickupRestoreMultiplier;
            ModifyEnergy(scaledAmount);
        }

        /// <summary>
        /// Directly sets energy to a specific value.
        /// </summary>
        /// <param name="value">New energy value to apply.</param>
        public void SetEnergy(float value)
        {
            ModifyEnergy(value - currentEnergy);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Modifies the current energy by a delta and clamps it to valid bounds.
        /// </summary>
        /// <param name="delta">Change in energy to apply.</param>
        private void ModifyEnergy(float delta)
        {
            float previous = currentEnergy;
            // Clamp to keep the energy contract consistent across all systems.
            currentEnergy = Mathf.Clamp(currentEnergy + delta, 0f, maxEnergy);

            if (!Mathf.Approximately(previous, currentEnergy))
            {
                RaiseEnergyChanged();
            }
        }

        /// <summary>
        /// Invokes the energy changed event.
        /// </summary>
        private void RaiseEnergyChanged()
        {
            OnEnergyChanged?.Invoke(currentEnergy);
        }
        #endregion
    }
}

