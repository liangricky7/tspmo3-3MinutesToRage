using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PeakRageEffect : MonoBehaviour
{
    public Material glitchMaterial;

    [Header("Thresholds")]
    public float peakStart = 200f;
    public float maxEnergy = 250f;

    [Header("Settings")]
    public float speed = 1f;

    private float intensity = 0f;

    void Update()
    {
        float energy = EnergyMeter.Instance != null ? EnergyMeter.Instance.energy : 0f;
        float target = energy >= peakStart ? Mathf.InverseLerp(peakStart, maxEnergy, energy) : 0f;
        intensity = Mathf.Lerp(intensity, target, Time.deltaTime * 3f);

        if (glitchMaterial != null)
            glitchMaterial.SetFloat("_Speed", speed);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (glitchMaterial != null && intensity > 0.001f)
        {
            glitchMaterial.SetFloat("_Intensity", intensity);
            Graphics.Blit(src, dest, glitchMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
