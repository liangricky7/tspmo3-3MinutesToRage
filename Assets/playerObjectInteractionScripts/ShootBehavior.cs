using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShootBehavior : MonoBehaviour
{
    [Header("References")]
    public Transform cam;

    [Header("Shot")]
    public float range = 50f;
    public float force = 20f;
    public float energyReward = 30f;
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
    public UnityEngine.UI.Image cooldownBar;
    [SerializeField]
    public UnityEngine.UI.Image fillImage;

    public Color readyColor = new Color(0.118f, 1f, 0.973f, 1f);
    public Color depletedColor = new Color(0.3f, 0.3f, 0.3f, 0.6f);


    void OnDisable()
    {
        EventsEnergyMeter.Instance.OnSane -= DisallowShooting;
        EventsEnergyMeter.Instance.OnInsane -= AllowShooting;
    }

    void Start()
    {
        EventsEnergyMeter.Instance.OnSane += DisallowShooting;
        EventsEnergyMeter.Instance.OnInsane += AllowShooting;
        
        // make sure ui turned off on start
        cooldownBar.gameObject.SetActive(false);
        readyText.gameObject.SetActive(false);
    }

    void AllowShooting()
    {
        canShoot = true;
    }

    void DisallowShooting()
    {
        canShoot = false;
        cooldownBar.gameObject.SetActive(false);
        readyText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!canShoot) return;
        
        if ((Time.time - lastShotTime) >= cooldownDuration)
        {
            onCooldown = false;
        }

        if (onCooldown)
        {
            cooldownBar.gameObject.SetActive(true);
            readyText.gameObject.SetActive(false);

            SetFill((Time.time - lastShotTime) / cooldownDuration);
        }
        else
        {
            cooldownBar.gameObject.SetActive(false);
            readyText.gameObject.SetActive(true);
        }

        if (!onCooldown && Input.GetMouseButtonDown(1) && animator.GetBool("ShootingGun") == false)
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

        HitCircle hitCircle = hit.collider.GetComponentInParent<HitCircle>();
        if (hitCircle != null)
        {
            if (hitCircle.TryHit())
            {
                EnergyMeter.Instance.AddEnergy(energyReward);
                hitCircle.DestroyEnemy();
            }
            return;
        }

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

    void SetFill(float t)
    {
        fillImage.fillAmount = t;
        fillImage.color = Color.Lerp(depletedColor, readyColor, t);
    }
}
