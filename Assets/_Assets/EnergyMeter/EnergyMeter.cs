using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyMeter : MonoBehaviour
{
    [Range(0f, 250f)] //this does nothing for capping the number; thats done in the addenergy func
    public float energy = 0f; // 0 to 250
    [SerializeField]
    private float decayRate = 0.7f; // how much energy decays per fixed update

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

    void FixedUpdate()
    {
        energy = energy - decayRate >= 0 ? energy - decayRate : 0;
    }

    public void AddEnergy(float amount)
    {
        energy = energy + amount > 250 ? 250 : energy + amount;
    }
}
