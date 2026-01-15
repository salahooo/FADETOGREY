using UnityEngine;
using FadeToGrey;  

public class FakeIdleBreathing : MonoBehaviour
{
    public PlayerController player;

    [Header("Breathing Settings")]
    public float speed = 1f;
    public float intensity = 0.015f;

    [SerializeField] private Transform visualRoot;

    private Vector3 startScale;

    void Awake()
    {
        if (visualRoot == null)
        {
            enabled = false;
            return;
        }

        startScale = visualRoot.localScale;
    }

    void Update()
    {
        if (player == null)
            return;

        if (player.IsMoving)
        {
            visualRoot.localScale = startScale;
            return;
        }

        float breathe = Mathf.Sin(Time.time * speed) * intensity;
        visualRoot.localScale = startScale + Vector3.one * breathe;
    }
}
