using TMPro;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    // Constants
    private const string EnemyTag = "Enemy";
    
    public static EnemyManager Instance;
    
    [SerializeField] private int enemyCount;
    public TextMeshProUGUI enemyCounterText;
    
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }
    
    public void EnemyDefeated() {
        enemyCount--;
        enemyCounterText.text = enemyCount.ToString();
        
        if (enemyCount <= 0) {
            if (GateController.Instance != null) {
                GateController.Instance.SetGateState(true);
            }
            Debug.Log("All enemies defeated!");
        }
        else {
            Debug.Log("Enemy Defeated! Remaining Enemies: " + enemyCount);
        }
    }
    
    public void RegisterEnemy() {
        enemyCount++;
        enemyCounterText.text = enemyCount.ToString();
        Debug.Log("Enemy Registered! Total Enemies: " + enemyCount);
    }
    
    /**
     * * Checks if all enemies have been defeated.
     */
    public bool AreAllEnemiesDefeated() {
        return enemyCount <= 0;
    }
}
