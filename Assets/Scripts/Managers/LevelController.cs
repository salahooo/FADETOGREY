using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    [Tooltip("De exacte naam van de Scene die je wilt laden")]
    public string levelNaam;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Speler gevonden, level laden: " + levelNaam);
            SceneManager.LoadScene(levelNaam);
        }
    }
}
