using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatBehavior : MonoBehaviour
{
    [Header("References")]
    public Transform cam;

    [Header("Attack")]
    public float hitRange = 3f;
    public float hitForce = 20f;
    public float cooldown = 0.4f;
    public float energyReward = 30f;
    public LayerMask hitLayers;

    [Header("Diagonal Force")]
    public float forceDirectionX = 1f;
    public float forceDirectionY = -0.5f;

    [Header("Audio")]
    public AudioClip swingSound;
    public AudioClip hitSound;
    private AudioSource audioSource;

    private float _cooldownTimer = 0f;
    private bool _swingFromRight = true;

    [Header("Animator")]
    public Animator animator;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (_cooldownTimer > 0f)
            _cooldownTimer -= Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && _cooldownTimer <= 0f && animator.GetBool("isAttacking") == false)
            StartAttack();
    }

    void StartAttack()
    {
        animator.SetBool("isAttacking", true);
        PlaySound(swingSound); // plays on swing
    }

    public void Attack()
    {
        RaycastHit hit;
        if (!Physics.Raycast(cam.position, cam.forward, out hit, hitRange, hitLayers))
            return;

        HitCircle hitCircle = hit.collider.GetComponentInParent<HitCircle>();
        if (hitCircle != null)
        {
            if (hitCircle.TryHit())
            {
                EnergyMeter.Instance.AddEnergy(energyReward);
                hitCircle.DestroyEnemy();
                _cooldownTimer = cooldown;
                PlaySound(hitSound); // plays on hit
            }
            return;
        }

        Breakable breakable = hit.collider.GetComponent<Breakable>();
        if (breakable != null)
        {
            breakable.Break(hit.point);
            _cooldownTimer = cooldown;
            PlaySound(hitSound);
            return;
        }

        Rigidbody rb = hit.collider.attachedRigidbody;
        if (rb == null || rb.isKinematic) return;

        float currentX = _swingFromRight ? forceDirectionX : -forceDirectionX;
        _swingFromRight = !_swingFromRight;

        Vector3 swingDir = (cam.right * currentX + cam.up * forceDirectionY + cam.forward).normalized;

        rb.velocity = Vector3.zero;
        rb.AddForce(swingDir * hitForce, ForceMode.Impulse);

        _cooldownTimer = cooldown;
        PlaySound(hitSound);
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }

    public void EndAttack()
    {
        animator.SetBool("isAttacking", false);
    }
}