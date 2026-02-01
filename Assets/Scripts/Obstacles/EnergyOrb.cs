using UnityEngine;

/// <summary>
/// Energy pickup.
/// Restores a random percentage of the player's max energy
/// when the player enters the trigger.
/// </summary>
public class EnergyOrb : MonoBehaviour
{
    [Header("Energy Restore (%)")]
    [Range(0f, 1f)]
    [SerializeField] private float minRestorePercent = 0.3f;

    [Range(0f, 1f)]
    [SerializeField] private float maxRestorePercent = 0.7f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        EnergySystem energySystem = other.GetComponent<EnergySystem>();

        if (energySystem == null)
            return;

        float restorePercent = Random.Range(minRestorePercent, maxRestorePercent);
        float restoreAmount = restorePercent * energySystem.MaxEnergy;

        energySystem.AddEnergy(restoreAmount);

        Destroy(gameObject);
    }
}
