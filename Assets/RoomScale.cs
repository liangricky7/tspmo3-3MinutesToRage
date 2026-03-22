using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScale : MonoBehaviour
{
    [SerializeField] public EnergyMeter energyMeter;
    [SerializeField] public Transform environment;

    private Vector3 originalScale;
    private float previousScaleMult = 1f;
    private Rigidbody[] rigidbodies;

    private void Awake()
    {
        energyMeter = FindObjectOfType<EnergyMeter>();
        originalScale = environment.localScale;
        rigidbodies = FindObjectsOfType<Rigidbody>();
    }

    private void Update()
    {
        if (energyMeter == null) return;

        float energy = energyMeter.energy;
        float t = Mathf.Clamp01((energy - 100f) / (250f - 100f));
        float scaleMult = Mathf.Lerp(1f, 2.5f, t);

        if (Mathf.Approximately(scaleMult, previousScaleMult)) return;

        // Ratio between new and old scale
        float scaleRatio = scaleMult / previousScaleMult;

        // Reposition each rigidbody relative to the environment pivot
        foreach (Rigidbody rb in rigidbodies)
        {
            Vector3 localPos = environment.InverseTransformPoint(rb.position);
            Vector3 scaledLocalPos = localPos * scaleRatio;
            rb.MovePosition(environment.TransformPoint(scaledLocalPos));
        }

        environment.localScale = originalScale * scaleMult;
        previousScaleMult = scaleMult;
    }
}
