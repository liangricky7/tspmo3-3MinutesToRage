using UnityEngine;

public class CameraScream : MonoBehaviour
{
    [Header("Shake Settings")]
    public float shakeStrength   = 0.05f;
    public float shakeSpeed      = 20f;
    public float energyThreshold = 50f;

    private Vector3 originalLocalPos;

    void Start()
    {
        originalLocalPos = transform.localPosition;
    }

    void Update()
    {
        float energy = EnergyMeter.Instance.energy;

        if (energy > energyThreshold)
        {
            float t = Mathf.InverseLerp(energyThreshold, 250f, energy);
            float strength = shakeStrength * t;

            float offsetX = Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) * 2f - 1f;
            float offsetY = Mathf.PerlinNoise(0f, Time.time * shakeSpeed) * 2f - 1f;

            transform.localPosition = originalLocalPos + new Vector3(offsetX, offsetY, 0f) * strength;
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalLocalPos, 10f * Time.deltaTime);
        }
    }
}