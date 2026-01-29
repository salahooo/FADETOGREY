using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EffectsController : MonoBehaviour
{
    [Header("Settings")]
    public Volume globalVolume; // Sleep hier je GlobalVolume object in
    public GameObject spotPrefab; // Sleep hier je BloodSpotPrefab in
    public Canvas uiCanvas; // Sleep hier je Canvas in

    [Header("Game Over Settings")]
    public float fadeSpeed = 0.5f;

    // Interne variabelen
    private ColorAdjustments colorAdjustments;
    private float currentSat = 0f; // In PostProcess is 0 normaal, -100 is zwart/wit
    private float currentExposure = 0f; // 0 is normaal, lager is donkerder
    public bool isFadingOut = true;

    void Start()
    {
        // Haal de settings op uit het Volume profiel
        if (globalVolume.profile.TryGet(out ColorAdjustments adj))
        {
            colorAdjustments = adj;
        }
        
        ResetEffects();
    }

    public void ResetEffects()
    {
        isFadingOut = false;
        currentSat = 0f;
        currentExposure = 0f;
        
        // Reset Post Processing waarden
        if (colorAdjustments != null)
        {
            colorAdjustments.saturation.value = 0f;
            colorAdjustments.postExposure.value = 0f;
        }

        // Verwijder alle oude vlekken
        if (uiCanvas != null)
        {
            foreach (Transform child in uiCanvas.transform)
            {
                // Let op: verwijder niet andere UI elementen als je die hebt!
                if (child.name.Contains("Spot")) Destroy(child.gameObject);
            }
        }
    }

    // --- PUBLIC FUNCTIES ---

    public void AddDamageEffect()
    {
        // 1. Maak een vlek aan
        if (spotPrefab != null && uiCanvas != null)
        {
            GameObject newSpot = Instantiate(spotPrefab, uiCanvas.transform);
            newSpot.name = "Spot"; // Zodat we hem later kunnen vinden

            // Zet hem op een willekeurige plek binnen het canvas
            RectTransform rect = newSpot.GetComponent<RectTransform>();
            float x = Random.Range(-800, 800);
            float y = Random.Range(-450f, 450f);
            rect.anchoredPosition = new Vector2(x, y);
            
            // Varieer de grootte en draaiing voor variatie
            float randomScale = Random.Range(0.8f, 1.5f);
            rect.localScale = new Vector3(randomScale, randomScale, 1f);
            rect.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
        }

        // 2. Camera Shake (optioneel, als je dat script nog hebt)
        if (CameraShaker.Instance != null)
        {
            CameraShaker.Instance.Shake(0.3f, 0.2f);
        }
    }

    public void StartGameOverFade()
    {
        isFadingOut = true;
    }

    void Update()
    {
        if (isFadingOut && colorAdjustments != null)
        {
            Debug.Log(currentSat + " | " + currentExposure);
            // Stap 1: Kleur verliezen (Saturatie naar -100)
            if (currentSat > -100f) 
            {
                currentSat -= Time.deltaTime * (fadeSpeed * 30f);
                colorAdjustments.saturation.value = currentSat;
            }
            // Stap 2: Donker worden (Exposure omlaag)
            else if (currentExposure > -5f) 
            {
                currentExposure -= Time.deltaTime * fadeSpeed;
                colorAdjustments.postExposure.value = currentExposure;
            }
        }

        // Check eerst of er wel een toetsenbord is aangesloten (voor de zekerheid)
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            AddDamageEffect();
        }
    }
}