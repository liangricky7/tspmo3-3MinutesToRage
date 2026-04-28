using TMPro;
using UnityEngine;

public class LeaderboardRow : MonoBehaviour
{
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI scoreText;

    public void SetData(int rank, string playerName, int score, bool highlight = false)
    {
        rankText.text = $"#{rank}";
        nameText.text = playerName;
        scoreText.text = score.ToString("N0"); // formats 1000 as 1,000

        // highlight player's own row differently
        if (highlight)
        {
            rankText.color = Color.yellow;
            nameText.color = Color.yellow;
            scoreText.color = Color.yellow;
        }
    }
}