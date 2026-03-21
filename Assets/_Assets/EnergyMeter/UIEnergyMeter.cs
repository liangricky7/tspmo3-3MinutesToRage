using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIEnergyMeter : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI energyText;
    // Start is called before the first frame update
    void Start()
    {
        energyText = gameObject.GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (EnergyMeter.Instance == null) return;
        energyText.text = EnergyMeter.Instance.energy.ToString("F0");
    }
}
