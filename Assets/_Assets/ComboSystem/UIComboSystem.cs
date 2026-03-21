using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIComboSystem : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    [SerializeField]
    private TextMeshProUGUI tierText;
    [SerializeField]
    private TextMeshProUGUI timerProgressText;


    private void OnDisable()
    {
        EventsPlayerInteraction.Instance.EnemyKill -= ProcessEnemyKill;
        EventsPlayerInteraction.Instance.BreakableKill -= ProcessBreakableKill;

        EventsComboSystem.Instance.ActivateComboSystem -= ActivateComboUI;
        EventsComboSystem.Instance.DeactivateComboSystem -= DeactivateComboUI;
    }

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    // Start is called before the first frame update
    void Start()
    {
        EventsPlayerInteraction.Instance.EnemyKill += ProcessEnemyKill;
        EventsPlayerInteraction.Instance.BreakableKill += ProcessBreakableKill;

        EventsComboSystem.Instance.ActivateComboSystem += ActivateComboUI;
        EventsComboSystem.Instance.DeactivateComboSystem += DeactivateComboUI;

        TextMeshProUGUI[] texts = gameObject.GetComponentsInChildren<TextMeshProUGUI>();
        tierText = texts[0];
        timerProgressText = texts[1];

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false; 
    }

    // Update is called once per frame
    void Update()
    {
        if (ComboSystem.Instance == null) return;

        tierText.text = ComboSystem.Instance.CurrentComboTier.ToString();
        timerProgressText.text = (ComboSystem.Instance.comboTimer / ComboSystem.Instance.ComboTimeLimit[(int)ComboSystem.Instance.CurrentComboTier]).ToString("P0");
    }

    private void ActivateComboUI()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
    
    private void DeactivateComboUI()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false; 
    }

    void ProcessEnemyKill()
    {
        UIComboSystemLog.Instance.AddEntry("Enemy killed! +" + ComboSystem.Instance.ComboScores[(int)ComboSystem.Instance.CurrentComboTier] + " points");
    }
    
    void ProcessBreakableKill()
    {
        UIComboSystemLog.Instance.AddEntry("Breakable destroyed! +" + ComboSystem.Instance.ComboScores[(int)ComboSystem.Instance.CurrentComboTier] + " points");
    }
}
