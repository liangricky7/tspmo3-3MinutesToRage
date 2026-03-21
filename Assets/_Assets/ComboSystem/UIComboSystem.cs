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

        TextMeshProUGUI[] texts = gameObject.GetComponentsInChildren<TextMeshProUGUI>();
        tierText = texts[0];
        timerProgressText = texts[1];
    }

    // Update is called once per frame
    void Update()
    {
        if (ComboSystem.Instance == null) return;
        canvasGroup.alpha = ComboSystem.Instance.isActivated ? 1f : 0f;
        canvasGroup.interactable = ComboSystem.Instance.isActivated;
        canvasGroup.blocksRaycasts = ComboSystem.Instance.isActivated;
        tierText.text = ComboSystem.Instance.CurrentComboTier.ToString();
        timerProgressText.text = (ComboSystem.Instance.comboTimer / ComboSystem.Instance.ComboTimeLimit[(int)ComboSystem.Instance.CurrentComboTier]).ToString("P0");
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
