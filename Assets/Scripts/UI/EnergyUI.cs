using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays the player's energy as a UI bar.
/// Subscribes to EnergySystem events and updates the fill amount and color.
/// This script contains NO gameplay logic.
/// </summary>
public class EnergyUI : MonoBehaviour
{
    // -------------------- REFERENCES --------------------

    [Header("References")]
    [SerializeField] private Image energyFillImage;
    [SerializeField] private EnergySystem energySystem;

    // -------------------- VISUAL SETTINGS --------------------

    [Header("Energy Colors")]
    [SerializeField] private Color highEnergyColor = Color.green;
    [SerializeField] private Color mediumEnergyColor = Color.yellow;
    [SerializeField] private Color lowEnergyColor = Color.red;

    [Header("Energy Thresholds (Normalized 0–1)")]
    [Tooltip("Below this value, energy is considered low")]
    [Range(0f, 1f)]
    [SerializeField] private float mediumThreshold = 0.5f;

    [Tooltip("Below this value, energy is considered critical")]
    [Range(0f, 1f)]
    [SerializeField] private float lowThreshold = 0.2f;

    // -------------------- UNITY LIFECYCLE --------------------

    private void Awake()
    {
        // Defensive programming: fail early if setup is wrong
        if (energyFillImage == null)
            Debug.LogError("EnergyUI: Energy Fill Image is not assigned.");

        if (energySystem == null)
            Debug.LogError("EnergyUI: EnergySystem reference is not assigned.");
    }

    private void OnEnable()
    {
        if (energySystem == null)
            return;

        energySystem.OnEnergyChanged += HandleEnergyChanged;

        // Sync UI immediately on enable
        HandleEnergyChanged(energySystem.CurrentEnergy);
    }

    private void OnDisable()
    {
        if (energySystem == null)
            return;

        energySystem.OnEnergyChanged -= HandleEnergyChanged;
    }

    // -------------------- EVENT HANDLING --------------------

    /// <summary>
    /// Called whenever the energy value changes.
    /// Updates fill amount and visual color.
    /// </summary>
    private void HandleEnergyChanged(float currentEnergy)
    {
        if (energyFillImage == null || energySystem == null)
            return;

        float normalized = energySystem.NormalizedEnergy();

        energyFillImage.fillAmount = normalized;
        energyFillImage.color = GetColorForEnergy(normalized);
    }

    // -------------------- HELPERS --------------------

    /// <summary>
    /// Determines the color of the energy bar based on energy level.
    /// </summary>
    private Color GetColorForEnergy(float normalizedEnergy)
    {
        if (normalizedEnergy <= lowThreshold)
            return lowEnergyColor;

        if (normalizedEnergy <= mediumThreshold)
            return mediumEnergyColor;

        return highEnergyColor;
    }
}
