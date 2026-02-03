using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    // The transform the camera should follow (usually the player)
    [SerializeField] private Transform target;

    // Time value used by SmoothDamp to control how smooth the camera movement is
    [SerializeField] private float smoothTime = 0.15f;

    // Internal velocity reference required by SmoothDamp
    // This value is modified automatically by Unity
    private Vector3 velocity = Vector3.zero;

    // LateUpdate is used so the camera moves after the player has moved
    // This prevents jitter and ensures smooth following
    private void LateUpdate()
    {
        // If no target is assigned, do nothing
        if (target == null)
            return;

        // Desired camera position:
        // - Follow target X and Y
        // - Keep the current Z position of the camera
        Vector3 targetPosition = new Vector3(
            target.position.x,
            target.position.y,
            transform.position.z // keep camera Z
        );

        // Smoothly move the camera towards the target position
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );
    }
}
