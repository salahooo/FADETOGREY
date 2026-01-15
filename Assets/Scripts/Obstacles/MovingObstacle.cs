// NEW FILE
using UnityEngine;

namespace FadeToGrey
{
    /// <summary>
    /// Obstacle that moves horizontally back and forth.
    /// </summary>
    public class MovingObstacle : ObstacleBase
    {
        #region Serialized Fields
        /// <summary>
        /// Distance the obstacle travels from its start position.
        /// </summary>
        [SerializeField] private float moveDistance = 2f;

        /// <summary>
        /// Speed multiplier for the horizontal movement.
        /// </summary>
        [SerializeField] private float moveSpeed = 1.5f;
        #endregion

        #region Private Fields
        /// <summary>
        /// Cached start position for oscillation.
        /// </summary>
        private Vector3 startPosition;
        #endregion

        #region Unity Callbacks
        /// <summary>
        /// Stores the initial position to oscillate around.
        /// </summary>
        private void Awake()
        {
            startPosition = transform.position;
        }
        #endregion

        #region Behavior
        /// <summary>
        /// Oscillates the obstacle on the X axis to create predictable stress patterns.
        /// </summary>
        protected override void ApplyBehavior()
        {
            // Sine-driven motion keeps the obstacle predictable for learning patterns.
            float offset = Mathf.Sin(Time.time * moveSpeed) * moveDistance;
            transform.position = startPosition + new Vector3(offset, 0f, 0f);
        }
        #endregion
    }
}

