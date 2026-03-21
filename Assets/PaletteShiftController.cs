using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PaletteShiftController : MonoBehaviour
{
    public Material paletteMat;
    public float hueShiftSpeed = 0.2f;

    [Range(0,1)] public float intensity = 1f;

    private float hueShift     = 0f;
    private int   currentMode  = 0;

    private float cyanSpike    = 0f;
    private float magentaSpike = 0f;
    private float yellowSpike  = 0f;

    void Update()
    {
        if (currentMode == 3)
        {
            hueShift = (hueShift + Time.deltaTime * hueShiftSpeed) % 1f;
            paletteMat.SetFloat("_HueShift", hueShift);
        }

        // Decay spikes back to zero over time
        cyanSpike    = Mathf.Max(0f, cyanSpike    - Time.deltaTime);
        magentaSpike = Mathf.Max(0f, magentaSpike - Time.deltaTime);
        yellowSpike  = Mathf.Max(0f, yellowSpike  - Time.deltaTime);

        paletteMat.SetFloat("_CyanSpike",    cyanSpike);
        paletteMat.SetFloat("_MagentaSpike", magentaSpike);
        paletteMat.SetFloat("_YellowSpike",  yellowSpike);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (paletteMat != null)
        {
            paletteMat.SetFloat("_Intensity", intensity);
            Graphics.Blit(src, dest, paletteMat);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }

    // ---- CMY Spike Functions ----

    public void SpikeCyan(float amount = 1f, float duration = 0.5f)
    {
        StartCoroutine(SpikeChannel("cyan", amount, duration));
    }

    public void SpikeMagenta(float amount = 1f, float duration = 0.5f)
    {
        StartCoroutine(SpikeChannel("magenta", amount, duration));
    }

    public void SpikeYellow(float amount = 1f, float duration = 0.5f)
    {
        StartCoroutine(SpikeChannel("yellow", amount, duration));
    }

    public void SpikeCMY(float amount = 1f, float duration = 0.5f)
    {
        SpikeCyan(amount, duration);
        SpikeMagenta(amount, duration);
        SpikeYellow(amount, duration);
    }

    IEnumerator SpikeChannel(string channel, float amount, float duration)
    {
        SetChannel(channel, amount);

        float holdTime = duration * 0.3f;
        yield return new WaitForSeconds(holdTime);

        float elapsed  = 0f;
        float fadeTime = duration * 0.7f;
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            SetChannel(channel, Mathf.Lerp(amount, 0f, elapsed / fadeTime));
            yield return null;
        }

        SetChannel(channel, 0f);
    }

    void SetChannel(string channel, float val)
    {
        switch (channel)
        {
            case "cyan":    cyanSpike    = val; break;
            case "magenta": magentaSpike = val; break;
            case "yellow":  yellowSpike  = val; break;
        }
    }

    // ---- Mode Functions ----

    public void SetModeImmediate(int mode)
    {
        if (currentMode == mode) return;
        currentMode = mode;
        paletteMat.SetInt("_Mode", mode);
    }

    public void ShiftTo(int mode, float duration = 0.5f)
    {
        currentMode = mode;
        paletteMat.SetInt("_Mode", mode);
        StopAllCoroutines();
        StartCoroutine(BlendIn(duration));
    }

    public void ResetPalette(float duration = 0.5f)
    {
        StartCoroutine(BlendOut(duration));
    }

    IEnumerator BlendIn(float duration)
    {
        float elapsed       = 0f;
        float startIntensity = paletteMat.GetFloat("_Intensity");
        while (elapsed < duration)
        {
            elapsed  += Time.deltaTime;
            intensity = Mathf.Lerp(startIntensity, 1f, elapsed / duration);
            yield return null;
        }
    }

    IEnumerator BlendOut(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed  += Time.deltaTime;
            intensity = Mathf.Lerp(1f, 0f, elapsed / duration);
            yield return null;
        }
    }
}