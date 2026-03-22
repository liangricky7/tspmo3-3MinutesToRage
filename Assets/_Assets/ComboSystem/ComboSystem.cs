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
    public float[] ComboScores = new float[] { 1f, 51f, 100f, 150f, 200f, 300f, 400f };
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
        if (Input.GetKeyDown(KeyCode.K))
        {
            UpgradeCombo();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            CurrentComboScore += 30f;
            if (CurrentComboTier == ComboTier.SUPER) comboTimer = 0f;
        }
        if (CurrentComboScore >= ComboScores[(int)CurrentComboTier])
        {
            UpgradeCombo();
        }
        // Debug.Log($"Current combo tier: {CurrentComboTier}, Combo timer: {comboTimer}");
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