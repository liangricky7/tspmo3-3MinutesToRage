using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    [Header("AWS")]
    private AmazonDynamoDBClient _client;
    private const string TableName = "3min-leaderboard";

    [Header("UI References")]
    public GameObject leaderboardPanel; // the whole panel to show/hide
    public Transform leaderboardContainer;  // the Vertical Layout Group object
    public GameObject leaderboardRowPrefab; // the row prefab
    public GameObject playerRankPanel;      // the bottom panel
    public Transform playerRankContainer;   // where player row goes inside that panel

    void Awake()
    {
        var credentials = new BasicAWSCredentials(
        );
        _client = new AmazonDynamoDBClient(credentials, RegionEndpoint.USEast2);
        Debug.Log("DynamoDB client initialized!"); // add this to confirm it ran
    }

    // Call this when you want to show leaderboard
    // Pass in the current player's id and score
    public async void ShowLeaderboard(string currentPlayerId, int currentPlayerScore)
    {
        leaderboardPanel.SetActive(true);

        // --- Fetch all scores from DynamoDB ---
        var request = new ScanRequest { TableName = TableName };
        var response = await _client.ScanAsync(request);

        // Sort descending by score
        var sorted = response.Items
            .OrderByDescending(item => int.Parse(item["score"].N))
            .ToList();

        // --- Clear old rows ---
        foreach (Transform child in leaderboardContainer)
            Destroy(child.gameObject);
        foreach (Transform child in playerRankContainer)
            Destroy(child.gameObject);

        // --- Find player's rank in full list ---
        int playerRank = sorted.FindIndex(
            item => item["playerId"].S == currentPlayerId
        ) + 1; // +1 because list is 0-indexed

        bool playerInTopTen = playerRank >= 1 && playerRank <= 10;

        // --- Populate top 10 rows ---
        int displayCount = Mathf.Min(10, sorted.Count);
        for (int i = 0; i < displayCount; i++)
        {
            var item = sorted[i];
            string name = item["playerName"].S;
            int score = int.Parse(item["score"].N);
            bool isCurrentPlayer = item["playerId"].S == currentPlayerId;

            var rowObj = Instantiate(leaderboardRowPrefab, leaderboardContainer);
            rowObj.GetComponent<LeaderboardRow>().SetData(i + 1, name, score, isCurrentPlayer);
        }

        // --- Show player rank panel if not top 10 ---
        if (!playerInTopTen && playerRank > 0)
        {
            playerRankPanel.SetActive(true);

            var playerItem = sorted[playerRank - 1];
            string name = playerItem["playerName"].S;

            var rowObj = Instantiate(leaderboardRowPrefab, playerRankContainer);
            rowObj.GetComponent<LeaderboardRow>().SetData(playerRank, name, currentPlayerScore, true);
        }
        else
        {
            playerRankPanel.SetActive(false);
        }
    }

    public async void SubmitScore(string playerName, int score)
    {
        var request = new PutItemRequest
        {
            TableName = TableName,
            Item = new Dictionary<string, AttributeValue>
            {
                { "playerId",   new AttributeValue { S = playerName } },
                { "playerName", new AttributeValue { S = playerName } },
                { "score",      new AttributeValue { N = score.ToString() } }
            }
        };

        await _client.PutItemAsync(request);
    }
}