using UnityEngine;

namespace FadeToGrey
{
    /// <summary>
    /// Base class for obstacles that drain player energy on contact.
    /// </summary>
    public class ObstacleBase : MonoBehaviour
    {
        #region Serialized Fields
        /// <summary>
        /// Amount of energy removed when the player collides with this obstacle.
        /// </summary>
        [SerializeField] protected float energyDamage = 10f;

        /// <summary>
        /// Optional override for the energy system if it is not on the player root.
        /// </summary>
        [SerializeField] private EnergySystem energySystemOverride;
        #endregion

        #region Unity Callbacks
        /// <summary>
        /// Calls the obstacle behavior every frame.
        /// </summary>
        protected virtual void Update()
        {
            ApplyBehavior();
        }

        /// <summary>
        /// Applies energy damage when a player trigger enters.
        /// </summary>
        /// <param name="other">Collider that entered the trigger.</param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            EnergySystem targetEnergy = ResolveEnergySystem(other);
            if (targetEnergy == null)
            {
                return;
            }

            // Obstacles represent stress spikes that immediately drain energy.
            targetEnergy.ApplyObstacleDamage(energyDamage);
        }
        #endregion

        #region Behavior
        /// <summary>
        /// Virtual method for obstacle-specific movement or visuals.
        /// </summary>
        protected virtual void ApplyBehavior()
        {
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Finds the energy system attached to the player or uses the override if set.
        /// </summary>
        /// <param name="other">Collider that entered the trigger.</param>
        /// <returns>Resolved energy system reference or null.</returns>
        private EnergySystem ResolveEnergySystem(Collider2D other)
        {
            if (energySystemOverride != null)
            {
                return energySystemOverride;
            }

            return other.GetComponentInParent<EnergySystem>();
        }
        #endregion
    }
}
