using UnityEngine;

public class PaletteInsanityHook : MonoBehaviour
{
    private PaletteShiftController palette;
    private CameraFOV cameraFOV;

    [Header("Thresholds")]
    public float insanityStart = 100f;
    public float plateauStart  = 175f;
    public float maxEnergy     = 250f;

    [Header("FOV Settings")]
    public float insanityFOVBoost = 30f;

    void Start()
    {
        palette   = GetComponent<PaletteShiftController>();
        cameraFOV = Camera.main.GetComponent<CameraFOV>();

        if (EnergyMeter.Instance != null)
        {
            EnergyMeter.Instance.OnInsane += HandleInsanity;
            EnergyMeter.Instance.OnSane   += HandleSanity;
        }
    }

    void OnDestroy()
    {
        if (EnergyMeter.Instance != null)
        {
            EnergyMeter.Instance.OnInsane -= HandleInsanity;
            EnergyMeter.Instance.OnSane   -= HandleSanity;
        }
    }

    void Update()
    {
        float energy = EnergyMeter.Instance.energy;

        if (energy < insanityStart)
        {
            palette.intensity     = 0f;
            palette.hueShiftSpeed = 0f;
            cameraFOV.ResetFOV();
        }
        else if (energy < plateauStart)
        {
            float t = (energy - insanityStart) / (plateauStart - insanityStart);
            palette.intensity     = Mathf.Lerp(0f,    1f,   t);
            palette.hueShiftSpeed = Mathf.Lerp(0.05f, 0.6f, t);
            palette.SetModeImmediate(3);
            cameraFOV.SetFOV(cameraFOV.defaultFOV + insanityFOVBoost * t);
        }
        else
        {
            palette.intensity     = 1f;
            palette.hueShiftSpeed = 0.6f;
            palette.SetModeImmediate(3);
            cameraFOV.SetFOV(cameraFOV.defaultFOV + insanityFOVBoost);
        }
    }

    void HandleInsanity()
    {
        palette.SpikeCMY(1f, 0.8f);
    }

    void HandleSanity()
    {
        palette.hueShiftSpeed = 0f;
    }
}