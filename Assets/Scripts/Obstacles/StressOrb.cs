// NEW FILE
using UnityEngine;

namespace FadeToGrey
{
    /// <summary>
    /// Obstacle that slowly homes toward the player using Rigidbody2D velocity steering.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class StressOrb : ObstacleBase
    {
        #region Serialized Fields
        /// <summary>
        /// Rigidbody2D used to move the orb with physics interactions.
        /// </summary>
        [SerializeField] private Rigidbody2D body;

        /// <summary>
        /// Target the orb will pursue, typically the player.
        /// </summary>
        [SerializeField] private Transform target;

        /// <summary>
        /// Maximum speed of the homing movement.
        /// </summary>
        [SerializeField] private float moveSpeed = 2.5f;

        /// <summary>
        /// Steering responsiveness toward the target.
        /// </summary>
        [SerializeField] private float steeringSharpness = 4f;
        #endregion

        #region Unity Callbacks
        /// <summary>
        /// Ensures references are assigned before behavior begins.
        /// </summary>
        private void Awake()
        {
            if (body == null)
            {
                body = GetComponent<Rigidbody2D>();
            }
        }

        /// <summary>
        /// Attempts to locate the player if no target is assigned.
        /// </summary>
        private void Start()
        {
            if (target == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    target = player.transform;
                }
            }
        }
        #endregion

        #region Behavior
        /// <summary>
        /// Steers the orb toward the target with gentle homing motion.
        /// </summary>
        protected override void ApplyBehavior()
        {
            if (body == null || target == null)
            {
                return;
            }

            Vector2 toTarget = (target.position - transform.position).normalized;
            Vector2 desiredVelocity = toTarget * moveSpeed;

            // Lerp provides a subtle, lingering pursuit rather than an instant snap.
            body.linearVelocity = Vector2.Lerp(body.linearVelocity, desiredVelocity, steeringSharpness * Time.deltaTime);
        }
        #endregion
    }
}

