using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drinking : MonoBehaviour
{
    public static Drinking Instance { get; private set; }

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

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EnergyMeter.Instance.AddEnergy(25f);
        }
    }
}
