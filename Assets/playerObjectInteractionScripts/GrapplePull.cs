using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplePull : MonoBehaviour
{
    [Header("References")]
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappable;
    public LineRenderer lr;

    [Header("Grapple")]
    public float maxGrappleDistance = 30f;
    public float pullForce = 15f;
    public float pullStopDistance = 1.5f;

    [Header("Cooldown")]
    public float grapplingCd = 0.5f;

    [Header("Input")]
    public KeyCode grappleKey = KeyCode.Mouse1;

    private float grapplingCdTimer = 0f;
    private Rigidbody pulledObject = null;
    private bool grappling = false;

    void Start()
    {
        if (lr != null) lr.enabled = false;
    }

    void Update()
    {
        if (grapplingCdTimer > 0f)
            grapplingCdTimer -= Time.deltaTime;

        if (Input.GetKeyDown(grappleKey))
        {
            if (grappling) 
            { 
                StopGrapple();
                grapplingCdTimer = grapplingCd / 2;
            }
            else StartGrapple();
        }

        if (grappling && pulledObject != null)
            PullObject();
    }

    void LateUpdate()
    {
        if (grappling && pulledObject != null && lr != null)
        {
            lr.SetPosition(0, gunTip.position);
            lr.SetPosition(1, pulledObject.position);
        }
    }

    void StartGrapple()
    {
        if (grapplingCdTimer > 0f) return;

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappable))
        {
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb == null) return;

            // Disable enemy tracking while grappled
            EnemyBehavior tracker = hit.collider.GetComponent<EnemyBehavior>();
            if (tracker != null) tracker.isGrappled = true;

            pulledObject = rb;
            grappling = true;

            if (lr != null)
            {
                lr.enabled = true;
                lr.SetPosition(0, gunTip.position);
                lr.SetPosition(1, pulledObject.position);
            }
        }
    }

    void PullObject()
    {
        Vector3 directionToPlayer = (transform.position - pulledObject.position);
        float distance = directionToPlayer.magnitude;

        if (distance <= pullStopDistance)
        {
            StopGrapple();
            return;
        }

        if (pulledObject.isKinematic)
            pulledObject.MovePosition(pulledObject.position + directionToPlayer.normalized * pullForce * Time.deltaTime);
        else
            pulledObject.velocity = directionToPlayer.normalized * pullForce;
    }

    void StopGrapple()
{
    if (pulledObject != null)
    {
        // Re-enable enemy tracking on release
        EnemyBehavior tracker = pulledObject.GetComponent<EnemyBehavior>();
        if (tracker != null) tracker.isGrappled = false;

        pulledObject.velocity = Vector3.zero;
        pulledObject = null;
    }

    grappling = false;
    grapplingCdTimer = grapplingCd;
    if (lr != null) lr.enabled = false;
}

    public bool IsGrappling() => grappling;
}
