using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PaletteShiftController : MonoBehaviour
{
    public Material paletteMat;
    public float hueShiftSpeed = 0.2f;

    [Range(0,1)] public float intensity = 1f;

    private float hueShift = 0f;
    private int currentMode = 0;

    void Update()
    {
        // continuously animate hue shift when in mode 3
        if (currentMode == 3)
        {
            hueShift = (hueShift + Time.deltaTime * hueShiftSpeed) % 1f;
            paletteMat.SetFloat("_HueShift", hueShift);
        }
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

    // Call this from anywhere in your game to switch palettes
    public void ShiftTo(int mode, float duration = 0.5f)
    {
        currentMode = mode;
        paletteMat.SetInt("_Mode", mode);
        StopAllCoroutines();
        StartCoroutine(BlendIn(duration));
    }

    IEnumerator BlendIn(float duration)
    {
        float elapsed = 0f;
        float startIntensity = paletteMat.GetFloat("_Intensity");
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            intensity = Mathf.Lerp(startIntensity, 1f, elapsed / duration);
            yield return null;
        }
    }

    public void ResetPalette(float duration = 0.5f)
    {
        StartCoroutine(BlendOut(duration));
    }

    IEnumerator BlendOut(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            intensity = Mathf.Lerp(1f, 0f, elapsed / duration);
            yield return null;
        }
    }
}