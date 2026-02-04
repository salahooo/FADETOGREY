using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    [Tooltip("Exact name of the scene to load")]
    public string levelNaam;

    [Tooltip("UI object that shows messages")]
    public GameObject uiObject;

    public bool isFinalLevel = false;

    private bool sceneIsLoading = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (sceneIsLoading)
            return;

        if (EnemyManager.Instance.AreAllEnemiesDefeated())
        {
            if (isFinalLevel)
            {
                if (uiObject != null)
                {
                    TextMeshProUGUI text = uiObject.GetComponent<TextMeshProUGUI>();
                    if (text != null)
                    {
                        text.text = "You have found peace!";
                    }

                    uiObject.SetActive(true);
                }

                DisablePlayer(other);
                return;
            }

            sceneIsLoading = true;
            SceneManager.LoadScene(levelNaam);
            return;
        }

        if (uiObject != null)
        {
            uiObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (sceneIsLoading)
            return;

        if (uiObject != null)
        {
            uiObject.SetActive(false);
        }
    }

    private void DisablePlayer(Collider2D player)
    {
        PlayerAnimation anim = player.GetComponent<PlayerAnimation>();
        if (anim != null) anim.enabled = false;

        EnergySystem energy = player.GetComponent<EnergySystem>();
        if (energy != null) energy.enabled = false;

        PlayerController controller = player.GetComponent<PlayerController>();
        if (controller != null) controller.enabled = false;

        PlayerInput input = player.GetComponent<PlayerInput>();
        if (input != null) input.enabled = false;

        PlayerAttack attack = player.GetComponent<PlayerAttack>();
        if (attack != null) attack.enabled = false;
    }
}
