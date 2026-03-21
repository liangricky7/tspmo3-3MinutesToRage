using System;
using UnityEngine;

public class EventsPlayerInteraction : MonoBehaviour
{
    public static EventsPlayerInteraction Instance { get; private set; }

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

    public void TriggerEnemyKill()
    {
        EnemyKill?.Invoke();
    }

    public void TriggerBreakableKill()
    {
        BreakableKill?.Invoke();
    }
}