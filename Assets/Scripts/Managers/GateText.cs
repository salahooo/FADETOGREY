using UnityEngine;

public class GateText : MonoBehaviour
{
    public GameObject uiObject;
    public GateController gateController;

    private void OnTriggerEnter2D(Collider2D other) {
        if (gateController.IsOpen)
            return;
        
        if (other.CompareTag("Player"))
        {
            uiObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (gateController.IsOpen)
            return;
        
        if (other.CompareTag("Player"))
        {
            uiObject.SetActive(false);
        }
    }
}
