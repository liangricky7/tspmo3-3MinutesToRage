using UnityEngine;

public class PaletteInsanityHook : MonoBehaviour
{
    private PaletteShiftController palette;
    private Camera cam;

    [Header("Thresholds")]
    public float insanityStart = 100f;
    public float plateauStart  = 175f;
    public float maxEnergy     = 250f;

    [Header("FOV Settings")]
    public float normalFOV = 60f;
    public float maxFOV    = 100f;
    public float fovSpeed  = 3f;

    void Start()
    {
        palette = GetComponent<PaletteShiftController>();
        cam     = Camera.main;

        if (EventsEnergyMeter.Instance != null)
        {
            EventsEnergyMeter.Instance.OnInsane += HandleInsanity;
            EventsEnergyMeter.Instance.OnSane   += HandleSanity;
        }
    }

    void OnDestroy()
    {
        if (EventsEnergyMeter.Instance != null)
        {
            EventsEnergyMeter.Instance.OnInsane -= HandleInsanity;
            EventsEnergyMeter.Instance.OnSane   -= HandleSanity;
        }
    }

    void Update()
    {
        float energy = EnergyMeter.Instance.energy;

        // palette
        if (energy < insanityStart)
        {
            palette.intensity     = 0f;
            palette.hueShiftSpeed = 0f;
        }
        else if (energy < plateauStart)
        {
            float t = (energy - insanityStart) / (plateauStart - insanityStart);
            palette.intensity     = Mathf.Lerp(0f,    1f,   t);
            palette.hueShiftSpeed = Mathf.Lerp(0.05f, 0.6f, t);
            palette.SetModeImmediate(3);
        }
        else
        {
            palette.intensity     = 1f;
            palette.hueShiftSpeed = 0.6f;
            palette.SetModeImmediate(3);
        }

        // FOV
        float targetFOV;
        if (energy >= insanityStart)
        {
            float t = Mathf.InverseLerp(insanityStart, maxEnergy, energy);
            targetFOV = Mathf.Lerp(normalFOV, maxFOV, t);
        }
        else
        {
            targetFOV = normalFOV;
        }

        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, fovSpeed * Time.deltaTime);
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