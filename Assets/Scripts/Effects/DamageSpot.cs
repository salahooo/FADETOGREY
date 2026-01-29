using UnityEngine;
using UnityEngine.UI;

public class DamageSpot : MonoBehaviour
{
    [Header("Instellingen")]
    public float timeBeforeFade = 2.0f; // Hoe lang blijft hij volledig zichtbaar?
    public float fadeDuration = 2.0f;   // Hoe lang duurt het wegvagen?
    public float dripSpeed = 10.0f;     // Hoe snel zakt hij naar beneden?

    private Image spotImage;
    private float timer = 0f;
    private float startAlpha;

    void Start()
    {
        spotImage = GetComponent<Image>();
        
        // Onthoud de begin-transparantie die je in de prefab hebt ingesteld
        if (spotImage != null)
        {
            startAlpha = spotImage.color.a;
        }
        
        dripSpeed = Random.Range(dripSpeed * 0.5f, dripSpeed * 1.5f);
    }

    void Update()
    {
        timer += Time.deltaTime;

        // 1. Het "Drip" effect (langzaam naar beneden zakken)
        // We passen de positie aan. Omdat het UI is, werkt transform.position prima.
        transform.position -= new Vector3(0, dripSpeed * Time.deltaTime, 0);

        // 2. Het Fade effect
        if (timer > timeBeforeFade)
        {
            // Bereken hoe ver we zijn met faden (van 0.0 tot 1.0)
            float fadeTimer = timer - timeBeforeFade;
            float progress = fadeTimer / fadeDuration;

            if (spotImage != null)
            {
                Color newColor = spotImage.color;
                // Lerp van StartAlpha naar 0
                newColor.a = Mathf.Lerp(startAlpha, 0f, progress);
                spotImage.color = newColor;
            }

            // 3. Opruimen als hij helemaal onzichtbaar is
            if (progress >= 1.0f)
            {
                Destroy(gameObject);
            }
        }
    }
}