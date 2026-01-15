// NEW FILE
using TMPro;
using UnityEngine;

namespace FadeToGrey
{
    /// <summary>
    /// Displays current energy percentage and changes text color based on intensity.
    /// </summary>
    public class EnergyUI : MonoBehaviour
    {
        #region Serialized Fields
        /// <summary>
        /// Energy system that provides values for the UI.
        /// </summary>
        [SerializeField] private EnergySystem energySystem;

        /// <summary>
        /// Text component used to display the energy percentage.
        /// </summary>
        [SerializeField] private TMP_Text energyText;

        /// <summary>
        /// Color used when energy is high.
        /// </summary>
        [SerializeField] private Color highEnergyColor = new Color(0.2f, 0.85f, 0.4f, 1f);

        /// <summary>
        /// Color used when energy is mid-range.
        /// </summary>
        [SerializeField] private Color midEnergyColor = new Color(0.95f, 0.7f, 0.2f, 1f);

        /// <summary>
        /// Color used when energy is low.
        /// </summary>
        [SerializeField] private Color lowEnergyColor = new Color(0.9f, 0.25f, 0.25f, 1f);

        /// <summary>
        /// Energy value at or below which the low color is used.
        /// </summary>
        [SerializeField] private float lowEnergyThreshold = 20f;

        /// <summary>
        /// Energy value at or below which the mid color is used.
        /// </summary>
        [SerializeField] private float midEnergyThreshold = 50f;
        #endregion

        #region Unity Callbacks
        /// <summary>
        /// Attempts to locate references automatically.
        /// </summary>
        private void Awake()
        {
            if (energySystem == null)
            {
                energySystem = FindFirstObjectByType<EnergySystem>();
            }

            if (energyText == null)
            {
                energyText = GetComponent<TMP_Text>();
            }
        }

        /// <summary>
        /// Subscribes to energy updates when enabled.
        /// </summary>
        private void OnEnable()
        {
            if (energySystem != null)
            {
                energySystem.OnEnergyChanged += HandleEnergyChanged;
            }
        }

        /// <summary>
        /// Unsubscribes from energy updates when disabled.
        /// </summary>
        private void OnDisable()
        {
            if (energySystem != null)
            {
                energySystem.OnEnergyChanged -= HandleEnergyChanged;
            }
        }

        /// <summary>
        /// Initializes the UI with the current energy value.
        /// </summary>
        private void Start()
        {
            if (energySystem != null)
            {
                HandleEnergyChanged(energySystem.CurrentEnergy);
            }
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Updates the UI when energy changes.
        /// </summary>
        /// <param name="newEnergy">New energy value.</param>
        private void HandleEnergyChanged(float newEnergy)
        {
            if (energyText == null)
            {
                return;
            }

            int percent = Mathf.RoundToInt(newEnergy);
            // Whole numbers read faster during play.
            energyText.text = $"{percent}%";
            // Color shifts reinforce the mental state at a glance.
            energyText.color = ResolveEnergyColor(newEnergy);
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Chooses a text color based on the current energy level.
        /// </summary>
        /// <param name="energy">Energy value to evaluate.</param>
        /// <returns>Color representing the energy state.</returns>
        private Color ResolveEnergyColor(float energy)
        {
            if (energy <= lowEnergyThreshold)
            {
                return lowEnergyColor;
            }

            if (energy <= midEnergyThreshold)
            {
                return midEnergyColor;
            }

            return highEnergyColor;
        }
        #endregion
    }
}

