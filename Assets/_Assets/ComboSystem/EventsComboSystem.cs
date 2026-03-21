using System;
using UnityEngine;

public class EventsComboSystem : MonoBehaviour
{
    public static EventsComboSystem Instance { get; private set; }

    public event Action OnPlayerDied;
    public event Action<int> OnScoreChanged;   // passes an int payload
    public event Action<string> OnItemPickedUp; // passes a string payload

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

    public void TriggerPlayerDied()
    {
        OnPlayerDied?.Invoke();
    }

    public void TriggerScoreChanged(int newScore)
    {
        OnScoreChanged?.Invoke(newScore);
    }

    public void TriggerItemPickedUp(string itemName)
    {
        OnItemPickedUp?.Invoke(itemName);
    }
}