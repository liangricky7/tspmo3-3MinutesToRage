using Hanzzz.MeshDemolisher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    [Header("Demolisher")]
    public Material interiorMaterial;
    public int breakPointCount = 8;
    public float breakRadius = 0.5f;

    [Header("Physics")]
    public float explosionForce = 400f;
    public float explosionRadius = 2f;
    public float pieceLifetime = 5f;

    private MeshDemolisher _meshDemolisher = new MeshDemolisher();
    private bool _isBroken = false;

    public void Break(Vector3 hitPoint)
    {
        if (_isBroken) return;
        _isBroken = true;

        List<Transform> breakPoints = GenerateBreakPoints(hitPoint);

        if (!_meshDemolisher.VerifyDemolishInput(gameObject, breakPoints))
        {
            CleanupBreakPoints(breakPoints);
            return;
        }

        List<GameObject> pieces = _meshDemolisher.Demolish(gameObject, breakPoints, interiorMaterial);

        foreach (GameObject piece in pieces)
        {
            piece.AddComponent<MeshCollider>().convex = true;
            Rigidbody rb = piece.AddComponent<Rigidbody>();
            rb.AddExplosionForce(explosionForce, hitPoint, explosionRadius);
            Destroy(piece, pieceLifetime);
        }

        EventsPlayerInteraction.Instance.TriggerBreakableKill();
        CleanupBreakPoints(breakPoints);
        Destroy(gameObject);
    }

    private List<Transform> GenerateBreakPoints(Vector3 center)
    {
        List<Transform> points = new List<Transform>();
        for (int i = 0; i < breakPointCount; i++)
        {
            GameObject bp = new GameObject("BP_" + i);
            bp.transform.position = center + Random.insideUnitSphere * breakRadius;
            points.Add(bp.transform);
        }
        return points;
    }

    private void CleanupBreakPoints(List<Transform> points)
    {
        foreach (Transform t in points)
            Destroy(t.gameObject);
    }
}