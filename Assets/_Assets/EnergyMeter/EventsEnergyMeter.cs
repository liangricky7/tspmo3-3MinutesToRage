using System;
using UnityEngine;

public class EventsEnergyMeter : MonoBehaviour
{
    public static EventsEnergyMeter Instance { get; private set; }

    public event Action OnInsane;
    public event Action OnSane;

    [SerializeField]
    private bool isSane = true;

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

    public void TriggerSanity()
    {
        if (isSane)
        {
            return; // prevent triggering sanity if already sane
        }
        else
        {
            isSane = true;
        }
        OnSane?.Invoke();
        Debug.Log("Sanity triggered");
    }

    public void TriggerInsanity()
    {
        if (!isSane)
        {
            return; // prevent triggering insanity if already insane
        } 
        else
        {
            isSane = false;
        }
        OnInsane?.Invoke();
        Debug.Log("Insanity triggered");
    }
}