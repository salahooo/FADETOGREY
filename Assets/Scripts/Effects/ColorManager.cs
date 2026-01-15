// NEW FILE
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace FadeToGrey
{
    /// <summary>
    /// Controls global color desaturation based on player energy using URP Color Adjustments.
    /// </summary>
    public class ColorManager : MonoBehaviour
    {
        #region Serialized Fields
        /// <summary>
        /// Energy system that drives saturation changes.
        /// </summary>
        [SerializeField] private EnergySystem energySystem;

        /// <summary>
        /// Volume that holds the Color Adjustments override.
        /// </summary>
        [SerializeField] private Volume volume;

        /// <summary>
        /// Time used to smooth the saturation transitions.
        /// </summary>
        [SerializeField] private float saturationSmoothTime = 0.25f;
        #endregion

        #region Private Fields
        /// <summary>
        /// Cached Color Adjustments override extracted from the Volume profile.
        /// </summary>
        private ColorAdjustments colorAdjustments;

        /// <summary>
        /// Target saturation value derived from the current energy.
        /// </summary>
        private float targetSaturation;

        /// <summary>
        /// Velocity reference for SmoothDamp.
        /// </summary>
        private float saturationVelocity;
        #endregion

        #region Unity Callbacks
        /// <summary>
        /// Locates required references before the first frame.
        /// </summary>
        private void Awake()
        {
            if (energySystem == null)
            {
                energySystem = FindFirstObjectByType<EnergySystem>();
            }

            if (volume == null)
            {
                volume = FindFirstObjectByType<Volume>();
            }

            if (volume != null && volume.profile != null)
            {
                volume.profile.TryGet(out colorAdjustments);
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
        /// Smoothly moves saturation toward the target value each frame.
        /// </summary>
        private void Update()
        {
            if (colorAdjustments == null)
            {
                return;
            }

            float currentSaturation = colorAdjustments.saturation.value;
            float smoothed = Mathf.SmoothDamp(currentSaturation, targetSaturation, ref saturationVelocity, saturationSmoothTime);
            colorAdjustments.saturation.Override(smoothed);
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Updates the saturation target when energy changes.
        /// </summary>
        /// <param name="newEnergy">New energy value from 0-100.</param>
        private void HandleEnergyChanged(float newEnergy)
        {
            float normalized = Mathf.Clamp01(newEnergy / 100f);

            // A linear lerp from -50 to 0 keeps colors readable while still feeling drained.
            // The reduced saturation simulates burnout by visually stripping vibrancy as energy fades.
            targetSaturation = Mathf.Lerp(-50f, 0f, normalized);
        }
        #endregion
    }
}

