using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyMeter : MonoBehaviour
{
    [Range(0f, 250f)] //this does nothing for capping the number; thats done in the addenergy func
    public float energy = 0f; // 0 to 250
    [SerializeField]
    private float decayRate = 0.55f; // how much energy decays per fixed update
    [SerializeField]
    private float energyHoldTime = 2f; // how long energy holds before decaying
    [SerializeField]
    private float maxEnergyHoldTime = 3.5f; // how long max energy holds before decaying
    [SerializeField]
    private float energyHoldTimer = 0f; // timer for energy hold
    [SerializeField]
    private bool timerTrigger = false; // flag to check if energy is added
    [SerializeField]
    private bool maxEnergyHoldTrigger = false; // flag to check if max energy is reached
    public static EnergyMeter Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }   
    }

    void Start()
    {
        timerTrigger = false;
    }

    void FixedUpdate()
    {
        if (timerTrigger)
        {
            energyHoldTimer += Time.fixedDeltaTime;
            if (maxEnergyHoldTrigger && energyHoldTimer >= maxEnergyHoldTime)
            {
                energyHoldTimer = 0f;
                timerTrigger = false;
                maxEnergyHoldTrigger = false;
                // Debug.Log("Max energy hold");
            }
            else if (!maxEnergyHoldTrigger && energyHoldTimer >= energyHoldTime)
            {
                energyHoldTimer = 0f;
                timerTrigger = false;
                // Debug.Log("normal energy hold");
            }
        }
        else
        {
            energy = energy - decayRate >= 0 ? energy - decayRate : 0;
        }
        
        if (energy < 100f)
        {
            EventsEnergyMeter.Instance.TriggerSanity();
        }
        else
        {
            EventsEnergyMeter.Instance.TriggerInsanity();
        }
    }

    public bool SanityCheck()
    {
        return energy > 100f;
    }

    public void AddEnergy(float amount)
    {
        if (amount <= 0.1f && amount >= -0.1f) return; // ignore very small energy additions

        if (amount < 0f)
        {
            energy = Mathf.Max(0f, energy + amount);
            return; // subtractions skip the hold timer logic
        }

        if (energy + amount > 250f)
        {
            energy = 250f;
            maxEnergyHoldTrigger = true; // set max energy hold trigger when max energy is reached
        }
        else
        {
            energy += amount;
        }
        timerTrigger = true; 
        energyHoldTimer = 0f;
    }
}
