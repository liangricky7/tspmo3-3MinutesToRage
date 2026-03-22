using System;
using System.Collections;
using UnityEditor.PackageManager;
using UnityEngine;

public enum ComboTier
{
    None,
    FIRSTBLOOD,
    DECENT,
    CAPABLE,
    BETTER,
    AWESOME,
    SUPER,
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
    public float[] ComboScores = new float[] { 51f, 100f, 150f, 250f, 350f, 400f, 500f };
    [SerializeField]
    public float[] ComboMult = new float[] { 0, 1, 1.1f, 1.4f, 1.8f, 2f, 3f };


    private void OnDisable()
    {
        EventsPlayerInteraction.Instance.EnemyKill -= ProcessEnemyKill;
        EventsPlayerInteraction.Instance.BreakableKill -= ProcessBreakableKill;
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

    private void Start()
    {
        EventsPlayerInteraction.Instance.EnemyKill += ProcessEnemyKill;
        EventsPlayerInteraction.Instance.BreakableKill += ProcessBreakableKill;
    }

    void Update()
    {
        if (CurrentComboScore >= ComboScores[(int)CurrentComboTier])
        {
            Debug.Log($"Combo tier up! Previous tier: {CurrentComboTier}, Combo Score: {CurrentComboScore}");
            UpgradeCombo();
        }
        Debug.Log($"Current combo tier: {CurrentComboTier}, Combo Score: {CurrentComboScore}");
    }

    private IEnumerator StartTimer()
    {
        comboTimer = 0f;
        while (comboTimer < ComboTimeLimit[(int)CurrentComboTier])
        {
            comboTimer += Time.deltaTime;
            yield return null;
        }

        TimePointSystem.Instance.AddScore(CurrentComboScore * ComboMult[(int)CurrentComboTier]);

        CurrentComboTier = ComboTier.None;
        comboTimer = 0f;
        CurrentComboScore = 0;
        EventsComboSystem.Instance.UntriggerComboSystem();
    }

    void UpgradeCombo()
    {
        // used for turning off and on UI
        EventsComboSystem.Instance.TriggerComboSystem();

        if (CurrentComboTier < ComboTier.SUPER)
        {
            CurrentComboTier++;
            if (comboTimerCoroutine != null) StopCoroutine(comboTimerCoroutine);
            comboTimerCoroutine = StartCoroutine(StartTimer());
        }

        if (CurrentComboTier == ComboTier.SUPER) comboTimer = 0f;
    }

    void ProcessEnemyKill()
    {
        CurrentComboScore += 100f;
        if (CurrentComboTier == ComboTier.SUPER) comboTimer = 0f;
    }
    
    void ProcessBreakableKill()
    {
        CurrentComboScore += 50f;
        if (CurrentComboTier == ComboTier.SUPER) comboTimer = 0f;
    }
}