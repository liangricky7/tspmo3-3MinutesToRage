using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIEnergyMeter : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.UI.Image fillImage;

    public Color readyColor = new Color(0.118f, 1f, 0.973f, 1f);
    public Color depletedColor = new Color(0.3f, 0.3f, 0.3f, 0.6f);


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (EnergyMeter.Instance == null) return;
        SetFill(EnergyMeter.Instance.energy/250f);
    }

    void SetFill(float t)
    {
        fillImage.fillAmount = t;
        fillImage.color = Color.Lerp(depletedColor, readyColor, t);
    }
}
