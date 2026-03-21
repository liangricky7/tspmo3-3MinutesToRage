using System;
using System.Collections;
using UnityEngine;

public enum ComboTier
{
    None,
    F,
    D,
    C,
    B,
    A,
    S,
}

public class ComboSystem : MonoBehaviour
{
    public static ComboSystem Instance { get; private set; }
    public ComboTier CurrentComboTier { get; private set; } = ComboTier.None;
    // used for turning off and on UI    
    public bool isActivated = false;

    // combo timer variables
    private Coroutine comboTimerCoroutine;
    public float comboTimer = 0f;

    [SerializeField]
    public float[] ComboTimeLimit = new float[] { 0f, 4f, 4f, 3f, 3f, 2f, 2f };

    // combo scoring
    public float CurrentComboScore { get; private set; } = 0f;

    [SerializeField]
    public float[] ComboScores = new float[] { 1f, 50f, 100f, 150f, 200f, 300f, 400f };

    private void OnEnable()
    {
        EventsPlayerInteraction.Instance.EnemyKill += ProcessEnemyKill;
        EventsPlayerInteraction.Instance.BreakableKill += ProcessBreakableKill;
    }

    private void OnDisable()
    {
        EventsPlayerInteraction.Instance.EnemyKill += ProcessEnemyKill;
        EventsPlayerInteraction.Instance.BreakableKill += ProcessBreakableKill;
    }

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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            UpgradeCombo();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            CurrentComboScore += 30f;
            if (CurrentComboTier == ComboTier.S) comboTimer = 0f;
        }
        if (CurrentComboScore >= ComboScores[(int)CurrentComboTier])
        {
            UpgradeCombo();
        }
        Debug.Log($"Current combo tier: {CurrentComboTier}, Combo timer: {comboTimer}");
    }

    private IEnumerator StartTimer()
    {
        comboTimer = 0f;
        while (comboTimer < ComboTimeLimit[(int)CurrentComboTier])
        {
            comboTimer += Time.deltaTime;
            yield return null;
        }
        CurrentComboTier = ComboTier.None;
        CurrentComboScore = 0;
        isActivated = false;
    }

    void UpgradeCombo()
    {
        // used for turning off and on UI
        isActivated = true;

        if (CurrentComboTier < ComboTier.S)
        {
            CurrentComboTier++;
            if (comboTimerCoroutine != null) StopCoroutine(comboTimerCoroutine);
            comboTimerCoroutine = StartCoroutine(StartTimer());
        }
    }

    void ProcessEnemyKill()
    {
        CurrentComboScore += 100f;
        if (CurrentComboTier == ComboTier.S) comboTimer = 0f;
    }
    
    void ProcessBreakableKill()
    {
        CurrentComboScore += 50f;
        if (CurrentComboTier == ComboTier.S) comboTimer = 0f;
    }
}