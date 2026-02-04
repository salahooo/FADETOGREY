using UnityEngine;
using UnityEngine.Audio;

public class MusicEnergyController : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private EnergySystem energySystem;

    [Header("Lowpass Range (Hz)")]
    [SerializeField] private float minCutoff = 300f;     // extremely muffled
    [SerializeField] private float maxCutoff = 22000f;   // fully clear

    [Header("Smoothing")]
    [SerializeField] private float smoothTime = 0.6f;

    private float currentCutoff;
    private float cutoffVelocity;

    private const string LOWPASS_PARAM = "MusicLowpass";

    private void Awake()
    {
        // Try to auto-find EnergySystem if not assigned
        if (energySystem == null)
            energySystem = FindObjectOfType<EnergySystem>();
    }

    private void Start()
    {
        currentCutoff = maxCutoff;
        mixer.SetFloat(LOWPASS_PARAM, currentCutoff);
    }

    private void Update()
    {
        if (energySystem == null || mixer == null)
            return;

        float energy01 = Mathf.Clamp01(energySystem.NormalizedEnergy());

        // Logarithmic interpolation so low energy sounds MUCH lower
        float targetCutoff = Mathf.Exp(
            Mathf.Lerp(
                Mathf.Log(minCutoff),
                Mathf.Log(maxCutoff),
                energy01
            )
        );

        // Smooth fade instead of snapping
        currentCutoff = Mathf.SmoothDamp(
            currentCutoff,
            targetCutoff,
            ref cutoffVelocity,
            smoothTime
        );

        mixer.SetFloat(LOWPASS_PARAM, currentCutoff);
    }
}
