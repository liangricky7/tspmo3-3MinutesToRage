using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drinking : MonoBehaviour
{
    [Header("Animator")]

    public Animator animator;

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
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartDrink();
        }
    }

    public void StartDrink()
    {
        animator.SetBool("isDrinking", true);
    }

    public void Drink()
    {
        EnergyMeter.Instance.AddEnergy(25f);
    }

    public void EndDrink()
    {
        animator.SetBool("isDrinking", false);
    }
}
