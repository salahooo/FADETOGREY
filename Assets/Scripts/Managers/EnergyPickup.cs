// NEW FILE
using UnityEngine;

namespace FadeToGrey
{
    /// <summary>
    /// Collectible that restores energy and plays a pickup sound.
    /// </summary>
    public class EnergyPickup : MonoBehaviour
    {
        #region Serialized Fields
        /// <summary>
        /// Amount of energy restored on pickup.
        /// </summary>
        [SerializeField] private float restoreAmount = 20f;

        /// <summary>
        /// Audio clip to play when the pickup is collected.
        /// </summary>
        [SerializeField] private AudioClip pickupClip;

        /// <summary>
        /// Volume at which the pickup clip is played.
        /// </summary>
        [SerializeField] private float pickupVolume = 1f;
        #endregion

        #region Unity Callbacks
        /// <summary>
        /// Restores energy, plays audio, and destroys the pickup when collected.
        /// </summary>
        /// <param name="other">Collider that entered the pickup trigger.</param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            EnergySystem energySystem = other.GetComponentInParent<EnergySystem>();
            if (energySystem == null)
            {
                return;
            }

            energySystem.RestoreEnergy(restoreAmount);
            PlayPickupSound();

            Destroy(gameObject);
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Plays the pickup sound at the current world position.
        /// </summary>
        private void PlayPickupSound()
        {
            if (pickupClip == null)
            {
                return;
            }

            // PlayClipAtPoint creates a temporary audio source that survives this object.
            AudioSource.PlayClipAtPoint(pickupClip, transform.position, pickupVolume);
        }
        #endregion
    }
}

