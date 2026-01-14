using UnityEngine;

namespace FadeToGrey
{
    /// <summary>
    /// Obstacle that visually pulses by scaling up and down.
    /// </summary>
    public class PulseObstacle : ObstacleBase
    {
        #region Serialized Fields
        /// <summary>
        /// Speed of the pulse animation.
        /// </summary>
        [SerializeField] private float pulseSpeed = 2f;

        /// <summary>
        /// Maximum scale increase relative to the starting size.
        /// </summary>
        [SerializeField] private float pulseScaleAmount = 0.3f;
        #endregion

        #region Private Fields
        /// <summary>
        /// Original scale used as the base for pulsing.
        /// </summary>
        private Vector3 baseScale;
        #endregion

        #region Unity Callbacks
        /// <summary>
        /// Caches the initial scale on startup.
        /// </summary>
        private void Awake()
        {
            baseScale = transform.localScale;
        }
        #endregion

        #region Behavior
        /// <summary>
        /// Expands and contracts the obstacle to visualize stress pressure.
        /// </summary>
        protected override void ApplyBehavior()
        {
            // Use a smooth sinusoid so the pulse feels organic rather than abrupt.
            float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
            float scaleMultiplier = 1f + pulseScaleAmount * pulse;
            transform.localScale = baseScale * scaleMultiplier;
        }
        #endregion
    }
}
