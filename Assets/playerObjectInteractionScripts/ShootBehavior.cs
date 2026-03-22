using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootBehavior : MonoBehaviour
{
    [Header("References")]
    public Transform cam;

    [Header("Shot")]
    public float range = 50f;
    public float force = 20f;
    public float cooldown = 0.2f;
    public int magSize = 2;
    public int currentAmmo;
    public LayerMask hitLayers;

    public float _cooldownTimer = 0f;

    void Start()
    {
        currentAmmo = magSize;
    }

    void Update()
    {
        if (_cooldownTimer > 0f)
            _cooldownTimer -= Time.deltaTime;

        if (Input.GetMouseButtonDown(1) && _cooldownTimer <= 0f)
            Fire();
    }

    public void Fire()
    {
        /*currentAmmo--;
        if (currentAmmo == 0)
        {
            _cooldownTimer += 2f;
        }*/
        RaycastHit hit;
        if (!Physics.Raycast(cam.position, cam.forward, out hit, range, hitLayers))
            return;

        Breakable breakable = hit.collider.GetComponent<Breakable>();
        if (breakable != null)
        {
            breakable.Break(hit.point);
            _cooldownTimer = cooldown;
            return;
        }

        Rigidbody rb = hit.collider.attachedRigidbody;
        if (rb == null || rb.isKinematic) return;

        rb.AddForce(cam.forward * force, ForceMode.Impulse);

        _cooldownTimer = cooldown;
    }
}
