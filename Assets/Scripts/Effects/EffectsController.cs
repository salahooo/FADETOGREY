using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EffectsController : MonoBehaviour
{
    [Header("References")]
    public EnergySystem energySystem; // SLEEP HIER JE PLAYER/ENERGYSYSTEM OP
    public Volume globalVolume;
    public GameObject spotPrefab;
    public Canvas uiCanvas;

    [Header("Visual Settings")]
    [Tooltip("Hoeveel saturatie bij 0 energie (-100 is zwart/wit)")]
    public float minSaturation = -100f;
    
    [Tooltip("Hoe donker het beeld wordt bij 0 energie (-2 is vrij donker)")]
    public float minExposure = -1.5f;

    // Interne variabelen
    private ColorAdjustments colorAdjustments;

    void Start()
    {
        // Haal de settings op uit het Volume profiel
        if (globalVolume != null && globalVolume.profile.TryGet(out ColorAdjustments adj))
        {
            colorAdjustments = adj;
        }
        else
        {
            Debug.LogWarning("Geen Global Volume of ColorAdjustments gevonden!");
        }

        ResetEffects();
    }

    void Update()
    {
        if (energySystem == null || colorAdjustments == null) return;

        UpdateVisuals();
        CheckExhaustionTrigger();
    }

    private void UpdateVisuals()
    {
        // Haal genormaliseerde energie op (tussen 0.0 en 1.0)
        float energyPercent = energySystem.NormalizedEnergy();

        // Bereken de nieuwe waardes gebaseerd op energie
        // Lerp gaat van A naar B. Als energy 1 is, pakken we 0. Als energy 0 is, pakken we minSaturation.
        float targetSat = Mathf.Lerp(minSaturation, 0f, energyPercent);
        float targetExp = Mathf.Lerp(minExposure, 0f, energyPercent);

        // Pas de post processing aan
        colorAdjustments.saturation.value = targetSat;
        colorAdjustments.postExposure.value = targetExp;
    }

    // Houdt bij of we al een effect hebben gespawned voor deze uitputtings-sessie
    private bool hasTriggeredExhaustionEffect = false;

    private void CheckExhaustionTrigger()
    {
        // Als de speler uitgeput is en we hebben nog geen vlek geplaatst
        if (energySystem.IsExhausted && !hasTriggeredExhaustionEffect)
        {
            AddDamageEffect();
            hasTriggeredExhaustionEffect = true;
        }
        // Reset de trigger als de speler weer energie heeft
        else if (!energySystem.IsExhausted)
        {
            hasTriggeredExhaustionEffect = false;
        }
    }

    public void ResetEffects()
    {
        if (colorAdjustments != null)
        {
            colorAdjustments.saturation.value = 0f;
            colorAdjustments.postExposure.value = 0f;
        }

        // Verwijder oude vlekken
        if (uiCanvas != null)
        {
            foreach (Transform child in uiCanvas.transform)
            {
                if (child.name.Contains("Spot")) Destroy(child.gameObject);
            }
        }
    }

    public void AddDamageEffect()
    {
        if (spotPrefab != null && uiCanvas != null)
        {
            GameObject newSpot = Instantiate(spotPrefab, uiCanvas.transform);
            newSpot.name = "Spot";

            RectTransform rect = newSpot.GetComponent<RectTransform>();
            float x = Random.Range(-400, 400); // Iets verkleind voor veiligheid
            float y = Random.Range(-250f, 250f);
            rect.anchoredPosition = new Vector2(x, y);
            
            float randomScale = Random.Range(0.8f, 1.5f);
            rect.localScale = new Vector3(randomScale, randomScale, 1f);
            rect.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
        }
    }
}