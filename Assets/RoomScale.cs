using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScale : MonoBehaviour
{
    [SerializeField] public EnergyMeter energyMeter;
    [SerializeField] public ObjectSpawner objectSpawner;
    [SerializeField] public Transform environment;

    private Vector3 originalScale;
    private float previousScaleMult = 1f;

    private void Awake()
    {
        energyMeter = FindObjectOfType<EnergyMeter>();
        objectSpawner = FindObjectOfType<ObjectSpawner>();
        originalScale = environment.localScale;
    }

    private void Update()
    {
        if (energyMeter == null) return;

        float energy = energyMeter.energy;
        float t = Mathf.Clamp01((energy - 100f) / (250f - 100f));
        float scaleMult = Mathf.Lerp(1f, 2.5f, t);

        if (Mathf.Approximately(scaleMult, previousScaleMult)) return;

        float scaleRatio = scaleMult / previousScaleMult;

        // Refresh every frame so newly spawned rigidbodies are included
        Rigidbody[] rigidbodies = FindObjectsOfType<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies)
        {
            Vector3 localPos = environment.InverseTransformPoint(rb.position);
            Vector3 scaledLocalPos = localPos * scaleRatio;
            rb.MovePosition(environment.TransformPoint(scaledLocalPos));
        }

        if (objectSpawner != null)
        {
            Vector3 localCenter = environment.InverseTransformPoint(objectSpawner.boxCenter);
            objectSpawner.boxCenter = environment.TransformPoint(localCenter * scaleRatio);
            objectSpawner.boxSize *= scaleRatio;
        }

        environment.localScale = originalScale * scaleMult;
        previousScaleMult = scaleMult;
    }
}