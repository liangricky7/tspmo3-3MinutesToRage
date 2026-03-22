using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;

public class ShootBehavior : MonoBehaviour
{
    [Header("References")]
    public Transform cam;

    [Header("Shot")]
    public float range = 50f;
    public float force = 20f;
    public LayerMask hitLayers;
    public float lastShotTime = 0f;
    public float cooldownDuration = 3f;
    public bool onCooldown = false;

    [Header("Animator")]
    public Animator animator;

    [Header("CanShoot")]
    public bool canShoot = false;
    public TextMeshProUGUI readyText;
    [SerializeField]
    public Image cooldownBar;
    [SerializeField]
    public Image fillImage;

    void OnDisable()
    {
        EventsEnergyMeter.Instance.OnSane -= DisallowShooting;
        EventsEnergyMeter.Instance.OnInsane -= AllowShooting;
    }

    void Start()
    {
        EventsEnergyMeter.Instance.OnSane += DisallowShooting;
        EventsEnergyMeter.Instance.OnInsane += AllowShooting;
    }

    void AllowShooting()
    {
        canShoot = true;
    }

    void DisallowShooting()
    {
        canShoot = false;
    }

    void Update()
    {
        if ((Time.time - lastShotTime) >= cooldownDuration)
        {
            onCooldown = false;
        }

        if (onCooldown)
        {
            
        }
        else
        {
            
        }

        if (canShoot && !onCooldown && Input.GetMouseButtonDown(1) && animator.GetBool("ShootingGun") == false)
            StartShoot();
    }

    public void StartShoot()
    {
        animator.SetBool("ShootingGun", true);
    }

    public void Fire()
    {
        lastShotTime = Time.time;
        RaycastHit hit;
        if (!Physics.Raycast(cam.position, cam.forward, out hit, range, hitLayers))
            return;

        Breakable breakable = hit.collider.GetComponent<Breakable>();
        if (breakable != null)
        {
            breakable.Break(hit.point);
            return;
        }

        Rigidbody rb = hit.collider.attachedRigidbody;
        if (rb == null || rb.isKinematic) return;

        rb.AddForce(cam.forward * force, ForceMode.Impulse);
    }

    public void EndShoot()
    {
        animator.SetBool("ShootingGun", false);
        onCooldown = true;
    }

    // void SetFill(float t)
    // {
    //     fillImage.fillAmount = t;
    //     fillImage.color = Color.Lerp(depletedColor readyColor, t);
    // }
}
