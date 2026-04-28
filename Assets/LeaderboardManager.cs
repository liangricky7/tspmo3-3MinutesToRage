using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    private AmazonDynamoDBClient _client;
    private const string TableName = "GameLeaderboard";

    void Awake()
    {
        var credentials = new BasicAWSCredentials(
        );

        _client = new AmazonDynamoDBClient(credentials, RegionEndpoint.USEast1); // match your region!
    }

    public async void SubmitScore(string playerId, string playerName, int score)
    {
        var request = new PutItemRequest
        {
            TableName = TableName,
            Item = new Dictionary<string, AttributeValue>
            {
                { "playerId",   new AttributeValue { S = playerId } },
                { "playerName", new AttributeValue { S = playerName } },
                { "score",      new AttributeValue { N = score.ToString() } }
            }
        };

        await _client.PutItemAsync(request);
        Debug.Log("Score submitted!");
    }
}