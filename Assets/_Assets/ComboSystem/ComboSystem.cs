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

    public bool isActivated = false;

    // combo timer variables
    private Coroutine comboTimerCoroutine;
    public float comboTimer = 0f;

    [SerializeField]
    public float[] ComboTimeLimit = new float[] { 0f, 2f, 1.5f, 1.2f, 1.2f, 1f, 1f };

    private void OnEnable()
    {
        EventsPlayerInteraction.Instance.EnemyKill += ActivateComboSystem;
        EventsPlayerInteraction.Instance.BreakableKill += ActivateComboSystem;
    }

    private void OnDisable()
    {
        EventsPlayerInteraction.Instance.EnemyKill -= ActivateComboSystem;
        EventsPlayerInteraction.Instance.BreakableKill -= ActivateComboSystem;
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
        if (Input.GetKeyDown(KeyCode.O))
        {
            ActivateComboSystem();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            UpgradeCombo();
        }
        Debug.Log($"Current combo tier: {CurrentComboTier}, Combo timer: {comboTimer}");
    }

    void ActivateComboSystem()
    {
        if (isActivated)
        {
            Debug.Log("Combo system already activated");
            return;
        }

        Debug.Log("Combo system activated");

        isActivated = true;
        CurrentComboTier = ComboTier.F;
        // pull up combo UI
        comboTimerCoroutine = StartCoroutine(StartTimer());
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
        isActivated = false;
    }
    
    void UpgradeCombo()
    {
        if (!isActivated)
        {
            Debug.Log("Combo system not activated yet");
            return;
        }
        if (CurrentComboTier < ComboTier.S)
        {
            CurrentComboTier++;
            StopCoroutine(comboTimerCoroutine);
            comboTimerCoroutine = StartCoroutine(StartTimer());
        }
    }
}