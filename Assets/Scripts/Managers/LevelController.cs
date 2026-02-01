using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    [Tooltip("De exacte naam van de Scene die je wilt laden")]
    public string levelNaam;
    public GameObject uiObject;
    public bool isFinalLevel = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (EnemyManager.Instance.AreAllEnemiesDefeated()) {
                if (isFinalLevel) {
                    uiObject.GetComponent<TextMeshProUGUI>().text = "You have found peace!";
                    uiObject.SetActive(true);
                    other.GetComponent<PlayerAnimation>().enabled = false;
                    other.GetComponent<EnergySystem>().enabled = false;
                    other.GetComponent<PlayerController>().enabled = false;
                    other.GetComponent<PlayerInput>().enabled = false;
                    other.GetComponent<PlayerAttack>().enabled = false;
                    return;
                }
                SceneManager.LoadScene(levelNaam);
                return;
            }
            
            uiObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player"))
        {
            uiObject.SetActive(false);
        }
    }
}
