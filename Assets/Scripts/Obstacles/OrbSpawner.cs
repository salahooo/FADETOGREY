using UnityEngine;

/// <summary>
/// Spawns EnergyOrbs and StressOrbs at random positions
/// inside a defined rectangular area.
/// One-time spawn on scene start.
/// </summary>
public class OrbSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject energyOrbPrefab;
    [SerializeField] private GameObject stressOrbPrefab;

    [Header("Spawn Area (World Coordinates)")]
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;
    [SerializeField] private float minY = -6f;
    [SerializeField] private float maxY = 6f;

    [Header("Spawn Amount")]
    [SerializeField] private int minEnergyOrbs = 2;
    [SerializeField] private int maxEnergyOrbs = 3;

    [SerializeField] private int minStressOrbs = 2;
    [SerializeField] private int maxStressOrbs = 3;

    [Header("Spawn Safety")]
    [SerializeField] private float minDistanceBetweenOrbs = 1.5f;

    private void Start()
    {
        SpawnRandomOrbs(
            energyOrbPrefab,
            Random.Range(minEnergyOrbs, maxEnergyOrbs + 1)
        );

        SpawnRandomOrbs(
            stressOrbPrefab,
            Random.Range(minStressOrbs, maxStressOrbs + 1)
        );
    }

    private void SpawnRandomOrbs(GameObject prefab, int count)
    {
        if (prefab == null)
            return;

        int spawned = 0;
        int safety = 0;

        while (spawned < count && safety < 100)
        {
            safety++;

            Vector2 randomPos = new Vector2(
                Random.Range(minX, maxX),
                Random.Range(minY, maxY)
            );

            if (IsPositionFree(randomPos))
            {
                Instantiate(prefab, randomPos, Quaternion.identity);
                spawned++;
            }
        }
    }

    private bool IsPositionFree(Vector2 position)
    {
        Collider2D hit = Physics2D.OverlapCircle(
            position,
            minDistanceBetweenOrbs
        );

        return hit == null;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Vector3 center = new Vector3(
            (minX + maxX) / 2f,
            (minY + maxY) / 2f,
            0f
        );

        Vector3 size = new Vector3(
            maxX - minX,
            maxY - minY,
            0f
        );

        Gizmos.DrawWireCube(center, size);
    }
#endif
}
