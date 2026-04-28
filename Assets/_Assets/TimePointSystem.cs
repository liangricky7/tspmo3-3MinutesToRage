using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimePointSystem : MonoBehaviour
{
    [Header("Timer Settings")]
    public int time = 180; // 3 minutes in seconds
    [SerializeField]
    public TextMeshProUGUI scoreText;
    [SerializeField]
    public TextMeshProUGUI timerText;

    [SerializeField]
    private LeaderboardManager leaderboardManager;

    public string playerId = "Ricky"; // temp, replace with actual player ID system
    public int TrueScore { get; private set; }
    public int Score { get; private set; }
    public bool GameActive { get; private set; }

    public static TimePointSystem Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        GameActive = true;
    }

    void Start()
    {
        scoreText.text = "0";
        StartTimer(time);
    }

    public void StartTimer(int timeLimit)
    {
        StartCoroutine(TimerCoroutine(timeLimit));
    }

    private IEnumerator TimerCoroutine(int timeLimit)
    {
        float timeRemaining = timeLimit;
        while (timeRemaining > 0)
        {
            if (timeRemaining >= 60f)
            {
                int minutes = Mathf.FloorToInt(timeRemaining / 60f);
                int seconds = Mathf.FloorToInt(timeRemaining % 60f);
                timerText.text = $"{minutes:00}:{seconds:00}";
            }
            else
            {
                timerText.text = Mathf.CeilToInt(timeRemaining).ToString();
            }
            timeRemaining -= 1;
            yield return new WaitForSeconds(1f);
        }

        GameActive = false;
        leaderboardManager.SubmitScore(playerId, Score);
        leaderboardManager.ShowLeaderboard(playerId, Score);
    }
    public void AddTrueScore(float addition)
    {
        TrueScore += (int) addition;
    }
    public void AddScore(float addition)
    {
        Score += (int) addition;
        scoreText.text = Score.ToString();
    }
}