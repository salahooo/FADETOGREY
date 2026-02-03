using UnityEngine;

// Spawns EnergyOrbs and StressOrbs at random positions
// within a rectangular area in the world.
// Spawning happens once when the scene starts.
public class OrbSpawner : MonoBehaviour
{
    // Prefabs that will be spawned
    [Header("Prefabs")]
    [SerializeField] private GameObject energyOrbPrefab;
    [SerializeField] private GameObject stressOrbPrefab;

    // World-space boundaries where orbs are allowed to spawn
    [Header("Spawn Area (World Coordinates)")]
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;
    [SerializeField] private float minY = -6f;
    [SerializeField] private float maxY = 6f;

    // Minimum and maximum amount of energy orbs per scene
    [Header("Spawn Amount")]
    [SerializeField] private int minEnergyOrbs = 2;
    [SerializeField] private int maxEnergyOrbs = 3;

    // Minimum and maximum amount of stress orbs per scene
    [SerializeField] private int minStressOrbs = 2;
    [SerializeField] private int maxStressOrbs = 3;

    // Minimum distance required between spawned orbs
    // Prevents overlapping spawns
    [Header("Spawn Safety")]
    [SerializeField] private float minDistanceBetweenOrbs = 1.5f;

    // Called once when the scene starts
    private void Start()
    {
        // Spawn a random amount of energy orbs
        SpawnRandomOrbs(
            energyOrbPrefab,
            Random.Range(minEnergyOrbs, maxEnergyOrbs + 1)
        );

        // Spawn a random amount of stress orbs
        SpawnRandomOrbs(
            stressOrbPrefab,
            Random.Range(minStressOrbs, maxStressOrbs + 1)
        );
    }

    // Spawns a given number of orbs of the provided prefab
    private void SpawnRandomOrbs(GameObject prefab, int count)
    {
        // Safety check in case the prefab was not assigned
        if (prefab == null)
            return;

        int spawned = 0;
        int safety = 0;

        // Try to spawn until the desired count is reached
        // or until the safety limit is hit
        while (spawned < count && safety < 100)
        {
            safety++;

            // Pick a random position inside the spawn area
            Vector2 randomPos = new Vector2(
                Random.Range(minX, maxX),
                Random.Range(minY, maxY)
            );

            // Only spawn if no collider is too close
            if (IsPositionFree(randomPos))
            {
                Instantiate(prefab, randomPos, Quaternion.identity);
                spawned++;
            }
        }
    }

    // Checks whether a position is free of other colliders
    private bool IsPositionFree(Vector2 position)
    {
        // Check for any collider within the minimum distance
        Collider2D hit = Physics2D.OverlapCircle(
            position,
            minDistanceBetweenOrbs
        );

        // Position is free if nothing was hit
        return hit == null;
    }

#if UNITY_EDITOR
    // Draws the spawn area in the Scene view for debugging
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        // Calculate center of the spawn rectangle
        Vector3 center = new Vector3(
            (minX + maxX) / 2f,
            (minY + maxY) / 2f,
            0f
        );

        // Calculate size of the spawn rectangle
        Vector3 size = new Vector3(
            maxX - minX,
            maxY - minY,
            0f
        );

        Gizmos.DrawWireCube(center, size);
    }
#endif
}
