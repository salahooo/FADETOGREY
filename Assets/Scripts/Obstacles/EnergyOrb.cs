using UnityEngine;

/*
 * EnergyOrb
 *
 * A collectible energy pickup used by the player.
 * When the player enters the trigger:
 * - Restores a random percentage of the player's maximum energy
 * - Destroys itself after being collected
 *
 * This script contains no movement or visual logic.
 * It relies purely on trigger interaction.
 */
public class EnergyOrb : MonoBehaviour
{
    // Minimum percentage of max energy that can be restored (0.3 = 30%)
    [Header("Energy Restore (%)")]
    [Range(0f, 1f)]
    [SerializeField] private float minRestorePercent = 0.3f;

    // Maximum percentage of max energy that can be restored (0.7 = 70%)
    [Range(0f, 1f)]
    [SerializeField] private float maxRestorePercent = 0.7f;

    /*
     * Triggered when another collider enters this object's trigger.
     * Only reacts to objects tagged as "Player".
     */
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore everything except the player
        if (!other.CompareTag("Player"))
            return;

        // Attempt to get the player's EnergySystem component
        EnergySystem energySystem = other.GetComponent<EnergySystem>();

        // If the player has no EnergySystem, do nothing
        if (energySystem == null)
            return;

        // Pick a random percentage between min and max
        float restorePercent = Random.Range(minRestorePercent, maxRestorePercent);

        // Convert percentage to an absolute energy value
        float restoreAmount = restorePercent * energySystem.MaxEnergy;

        // Restore energy to the player
        energySystem.AddEnergy(restoreAmount);

        // Remove the orb after it has been collected
        Destroy(gameObject);
    }
}
