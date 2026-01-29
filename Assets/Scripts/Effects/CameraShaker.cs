using UnityEngine;
using System.Collections;

public class CameraShaker : MonoBehaviour
{
    // Singleton instance zodat we het makkelijk kunnen aanroepen
    public static CameraShaker Instance { get; private set; }

    private Vector3 originalPos;
    private Coroutine currentShakeRoutine;

    void Awake()
    {
        Instance = this;
        originalPos = transform.localPosition;
    }
    
    // Roep dit aan: CameraShaker.Instance.Shake(0.5f, 0.2f);
    public void Shake(float duration, float magnitude)
    {
        if (currentShakeRoutine != null)
        {
            StopCoroutine(currentShakeRoutine);
            transform.localPosition = originalPos; // Reset voor de zekerheid
        }
        currentShakeRoutine = StartCoroutine(DoShake(duration, magnitude));
    }

    private IEnumerator DoShake(float duration, float magnitude)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // Genereer een willekeurige positie rondom het origineel
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.deltaTime;
            // Wacht één frame
            yield return null;
        }

        // Zet de camera terug op zijn plek
        transform.localPosition = originalPos;
        currentShakeRoutine = null;
    }
}