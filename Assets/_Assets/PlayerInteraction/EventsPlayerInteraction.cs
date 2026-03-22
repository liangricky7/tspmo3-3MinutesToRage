using System;
using UnityEngine;

public class EventsPlayerInteraction : MonoBehaviour
{
    public static EventsPlayerInteraction Instance { get; private set; }

    public event Action MeleeAttack;
    public event Action RangedAttack;

    public event Action EnemyKill;
    public event Action BreakableKill;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    void Update() // temporary update function for testing key presses
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            TriggerEnemyKill();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            TriggerBreakableKill();
        }
    }

    public void TriggerMeleeAnim()
    {
        MeleeAttack?.Invoke();
    }

    public void TriggerShootAnim()
    {
        RangedAttack?.Invoke();
    }

    public void TriggerEnemyKill()
    {
        EnemyKill?.Invoke();
    }

    public void TriggerBreakableKill()
    {
        BreakableKill?.Invoke();
    }
}