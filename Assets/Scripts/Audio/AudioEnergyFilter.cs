using UnityEngine;
using UnityEngine.Audio;

namespace FadeToGrey
{
    /// <summary>
    /// Applies a low-pass filter via AudioMixer to simulate muffled audio at low energy.
    /// </summary>
    public class AudioEnergyFilter : MonoBehaviour
    {
        #region Serialized Fields
        /// <summary>
        /// Energy system driving the filter response.
        /// </summary>
        [SerializeField] private EnergySystem energySystem;

        /// <summary>
        /// AudioMixer that contains the exposed low-pass cutoff parameter.
        /// </summary>
        [SerializeField] private AudioMixer audioMixer;

        /// <summary>
        /// Name of the exposed AudioMixer parameter that controls the low-pass cutoff.
        /// </summary>
        [SerializeField] private string lowPassParameter = "LowPassCutoff";

        /// <summary>
        /// Cutoff frequency when energy is exhausted, producing the most muffled sound.
        /// </summary>
        [SerializeField] private float minCutoff = 500f;

        /// <summary>
        /// Cutoff frequency for clear audio at high energy.
        /// </summary>
        [SerializeField] private float maxCutoff = 22000f;

        /// <summary>
        /// Smoothing time to avoid abrupt audio changes.
        /// </summary>
        [SerializeField] private float smoothingTime = 0.2f;
        #endregion

        #region Private Fields
        /// <summary>
        /// Target cutoff derived from the energy value.
        /// </summary>
        private float targetCutoff;

        /// <summary>
        /// Current cutoff being applied to the mixer.
        /// </summary>
        private float currentCutoff;

        /// <summary>
        /// Velocity reference for SmoothDamp.
        /// </summary>
        private float cutoffVelocity;
        #endregion

        #region Unity Callbacks
        /// <summary>
        /// Initializes defaults and finds references.
        /// </summary>
        private void Awake()
        {
            if (energySystem == null)
            {
                energySystem = FindObjectOfType<EnergySystem>();
            }

            targetCutoff = maxCutoff;
            currentCutoff = maxCutoff;
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
        /// Smoothly moves the mixer cutoff toward the target value.
        /// </summary>
        private void Update()
        {
            if (audioMixer == null)
            {
                return;
            }

            currentCutoff = Mathf.SmoothDamp(currentCutoff, targetCutoff, ref cutoffVelocity, smoothingTime);
            audioMixer.SetFloat(lowPassParameter, currentCutoff);
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Updates the target cutoff based on current energy.
        /// </summary>
        /// <param name="newEnergy">New energy value from 0-100.</param>
        private void HandleEnergyChanged(float newEnergy)
        {
            float normalized = Mathf.Clamp01(newEnergy / 100f);

            // Energy above 50 percent stays clear, while lower energy progressively muffles the audio.
            float clarity = normalized >= 0.5f ? 1f : normalized / 0.5f;

            // Min cutoff around 500 Hz yields a dull, muffled tone. Max cutoff near 22000 Hz feels full range.
            targetCutoff = Mathf.Lerp(minCutoff, maxCutoff, clarity);
        }
        #endregion
    }
}
