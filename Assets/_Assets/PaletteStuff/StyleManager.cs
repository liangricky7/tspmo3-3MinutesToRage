using UnityEngine;

public class StyleManager : MonoBehaviour
{
    public static StyleManager Instance;

    [Header("Style Settings")]
    public float decayRate    = 0.15f;
    public float pointsPerKill = 0.2f;

    [Range(0,1)]
    public float styleHeat = 0f;

    private PaletteShiftController palette;

    void Awake()
    {
        Instance = this;
        palette  = Camera.main.GetComponent<PaletteShiftController>();
    }

    void Update()
    {
        styleHeat = Mathf.Max(0f, styleHeat - decayRate * Time.deltaTime);
        UpdatePaletteFromHeat(styleHeat);

        // ---- Test Keys ----
        if (Input.GetKeyDown(KeyCode.Alpha1)) AddStyle(0.2f);
        if (Input.GetKeyDown(KeyCode.Alpha2)) AddStyle(0.4f);
        if (Input.GetKeyDown(KeyCode.Alpha3)) styleHeat = 1f;
        if (Input.GetKeyDown(KeyCode.Alpha4)) styleHeat = 0f;
        if (Input.GetKeyDown(KeyCode.Alpha5)) palette.SetModeImmediate(5);
        if (Input.GetKeyDown(KeyCode.Alpha6)) palette.SetModeImmediate(6);

        if (Input.GetKeyDown(KeyCode.Z)) palette.SpikeCyan(1f,    0.5f);
        if (Input.GetKeyDown(KeyCode.X)) palette.SpikeMagenta(1f, 0.5f);
        if (Input.GetKeyDown(KeyCode.C)) palette.SpikeYellow(1f,  0.5f);
        if (Input.GetKeyDown(KeyCode.V)) palette.SpikeCMY(1f,     0.5f);
    }

    public void AddStyle(float amount)
    {
        styleHeat = Mathf.Min(1f, styleHeat + amount);
    }

    public void RegisterKill()
    {
        AddStyle(pointsPerKill);
    }

    void UpdatePaletteFromHeat(float heat)
    {
        if (heat < 0.2f)
        {
            palette.intensity      = Mathf.Lerp(0f, 0.1f, heat / 0.2f);
            palette.SetModeImmediate(3);
            palette.hueShiftSpeed  = 0.05f;
        }
        else if (heat < 0.4f)
        {
            palette.intensity      = Mathf.Lerp(0.1f, 0.35f, (heat - 0.2f) / 0.2f);
            palette.SetModeImmediate(3);
            palette.hueShiftSpeed  = 0.1f;
        }
        else if (heat < 0.6f)
        {
            palette.intensity      = Mathf.Lerp(0.35f, 0.6f, (heat - 0.4f) / 0.2f);
            palette.SetModeImmediate(3);
            palette.hueShiftSpeed  = 0.25f;
        }
        else if (heat < 0.8f)
        {
            palette.intensity      = Mathf.Lerp(0.6f, 0.85f, (heat - 0.6f) / 0.2f);
            palette.SetModeImmediate(4);
            palette.hueShiftSpeed  = 0.5f;
        }
        else
        {
            palette.intensity      = Mathf.Lerp(0.85f, 1f, (heat - 0.8f) / 0.2f);
            palette.SetModeImmediate(1);
            palette.hueShiftSpeed  = 1.2f;
        }
    }
}