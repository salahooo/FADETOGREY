using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    // Constants
    private const string EnemyTag = "Enemy";
    
    public static EnemyManager Instance;
    
    [SerializeField] private int enemyCount;
    
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }
    
    /**
     * * Initializes the enemy count at the start of the game.
     */
    void Start() {
        enemyCount = GameObject.FindGameObjectsWithTag(EnemyTag).Length;
        Debug.Log("Initial Enemy Count: " + enemyCount);
    }
    
    public void EnemyDefeated() {
        enemyCount--;
        
        if (enemyCount <= 0) {
            Debug.Log("All enemies defeated!");
        }
        else {
            enemyCount--;
            Debug.Log("Enemy Defeated! Remaining Enemies: " + enemyCount);
        }
    }
    
    /**
     * * Checks if all enemies have been defeated.
     */
    public bool AreAllEnemiesDefeated() {
        return enemyCount <= 0;
    }
}
