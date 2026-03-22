using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimePointSystem : MonoBehaviour
{
    [SerializeField]
    public TextMeshProUGUI scoreText;
    [SerializeField]
    public TextMeshProUGUI timerText;

    [SerializeField]
    private Canvas gameOverCanvas;

    public float Score { get; private set; }
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
        StartTimer(4);
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
        gameOverCanvas.gameObject.SetActive(true);
        gameOverCanvas.GetComponentInChildren<TextMeshProUGUI>().text = $"Game Over!\nFinal Score: {Score}";
    }
    public void AddScore(float addition)
    {
        Score += addition;
        scoreText.text = Score.ToString();
    }
}