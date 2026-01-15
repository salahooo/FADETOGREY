// NEW FILE
using UnityEngine;

namespace FadeToGrey
{
    /// <summary>
    /// Smoothly follows a target in 2D using damped motion.
    /// </summary>
    public class CameraFollow2D : MonoBehaviour
    {
        #region Serialized Fields
        /// <summary>
        /// Target transform for the camera to follow.
        /// </summary>
        [SerializeField] private Transform target;

        /// <summary>
        /// Time it takes to smooth toward the target position.
        /// </summary>
        public float damping = 0.2f;

        /// <summary>
        /// Optional offset applied to the target position.
        /// </summary>
        [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

        /// <summary>
        /// If true, captures the starting offset from the target at runtime.
        /// </summary>
        [SerializeField] private bool useInitialOffset = true;
        #endregion

        #region Private Fields
        /// <summary>
        /// Velocity reference used by SmoothDamp.
        /// </summary>
        private Vector3 followVelocity;
        #endregion

        #region Unity Callbacks
        /// <summary>
        /// Stores the initial offset if configured to do so.
        /// </summary>
        private void Start()
        {
            if (target != null && useInitialOffset)
            {
                offset = transform.position - target.position;
                offset.z = -10f; // IMPORTANT: force camera Z
            }
        }

        /// <summary>
        /// Follows the target after all movement has been applied.
        /// </summary>
        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            Vector3 desiredPosition = target.position + offset;
            // SmoothDamp keeps the camera responsive without jittery motion.
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref followVelocity, damping);
        }
        #endregion
    }
}

