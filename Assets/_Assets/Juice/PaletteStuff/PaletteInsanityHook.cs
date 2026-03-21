using UnityEngine;

public class PaletteInsanityHook : MonoBehaviour
{
    private PaletteShiftController palette;

    [Header("Thresholds")]
    public float insanityStart   = 100f;  // matches EnergyMeter's sanity threshold
    public float plateauStart    = 175f;  // where effect hits full intensity
    public float maxEnergy       = 250f;  // matches EnergyMeter cap

    void Start()
    {
        palette = GetComponent<PaletteShiftController>();

        EventsEnergyMeter.Instance.OnInsane += HandleInsanity;
        EventsEnergyMeter.Instance.OnSane   += HandleSanity;
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

        if (energy < insanityStart)
        {
            // Below 100 - no effect
            palette.intensity     = 0f;
            palette.hueShiftSpeed = 0f;
        }
        else if (energy < plateauStart)
        {
            // Between 100 and 175 - gradually ramp up
            float t = (energy - insanityStart) / (plateauStart - insanityStart);

            palette.intensity     = Mathf.Lerp(0f,    1f,   t);
            palette.hueShiftSpeed = Mathf.Lerp(0.05f, 0.6f, t);
            palette.SetModeImmediate(3);
        }
        else
        {
            // 175 and above - plateaued at full intensity
            palette.intensity     = 1f;
            palette.hueShiftSpeed = 0.6f;
            palette.SetModeImmediate(3);
        }
    }

    void HandleInsanity()
    {
        // Fire a CMY blast when insanity threshold is first crossed
        palette.SpikeCMY(1f, 0.8f);
    }

    void HandleSanity()
    {
        // Snap speed down when returning to sane
        palette.hueShiftSpeed = 0f;
    }
}