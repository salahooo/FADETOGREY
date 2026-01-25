using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothTime = 0.15f;

    private Vector3 velocity = Vector3.zero;

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 targetPosition = new Vector3(
            target.position.x,
            target.position.y,
            transform.position.z // keep camera Z
        );

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );
    }
}
