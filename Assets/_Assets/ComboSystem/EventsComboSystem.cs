using System;
using UnityEngine;

public class EventsComboSystem : MonoBehaviour
{
    public static EventsComboSystem Instance { get; private set; }

    public event Action ActivateComboSystem;
    public event Action DeactivateComboSystem;
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

    public void TriggerComboSystem()
    {
        ActivateComboSystem?.Invoke();
    }

    public void UntriggerComboSystem()
    {
        DeactivateComboSystem?.Invoke();
    }
}