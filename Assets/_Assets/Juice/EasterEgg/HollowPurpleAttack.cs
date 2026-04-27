using UnityEngine;

public class HollowPurpleAttack : MonoBehaviour
{
    [Header("Keybind")]
    public KeyCode activationKey = KeyCode.H;

    [Header("Requirements")]
    public float minimumEnergy = 200f;
    public float cooldown = 15f;

    [Header("References")]
    public HollowPurpleEffect visualEffect;

    private float cooldownTimer = 0f;

    void Update()
    {
        cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(activationKey) && cooldownTimer <= 0f && EnergyMeter.Instance.energy >= minimumEnergy)
            Fire();
    }

    // Called by voice command — no restrictions, no cooldown consumed
    public void FireFromVoice()
    {
        if (visualEffect != null)
            visualEffect.Trigger();

        CameraEvents.TriggerShake(1.2f, 0.4f);

        foreach (HitCircle enemy in FindObjectsOfType<HitCircle>())
            enemy.DestroyEnemy();

        foreach (Breakable b in FindObjectsOfType<Breakable>())
            b.Break(b.transform.position);

        EnergyMeter.Instance.AddEnergy(50f);
    }

    void Fire()
    {
        cooldownTimer = cooldown;

        if (visualEffect != null)
            visualEffect.Trigger();

        CameraEvents.TriggerShake(1.2f, 0.4f);

        foreach (HitCircle enemy in FindObjectsOfType<HitCircle>())
            enemy.DestroyEnemy();

        foreach (Breakable b in FindObjectsOfType<Breakable>())
            b.Break(b.transform.position);

        EnergyMeter.Instance.AddEnergy(50f);
    }
}
