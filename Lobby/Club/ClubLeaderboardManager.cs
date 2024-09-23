using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;

public class ClubLeaderboardManager : MonoBehaviourPunCallbacks
{
    // A struct to hold leaderboard data
    public struct LeaderboardEntry
    {
        public string playerId;
        public string playerName;
        public int score;
    }

    // Event to update UI when leaderboard data is ready
    public delegate void OnLeaderboardUpdate(List<LeaderboardEntry> leaderboard);
    public static event OnLeaderboardUpdate LeaderboardUpdated;

    // The ID of the leaderboard
    private string leaderboardId = "ClubLeaderboard";

    // Call this method to refresh the leaderboard
    public void UpdateLeaderboard()
    {
        // Make sure the leaderboard is only updated by the master client
        if (!PhotonNetwork.IsMasterClient) return;

        // Request the leaderboard from PlayFab
        GetLeaderboardRequest request = new GetLeaderboardRequest
        {
            StatisticName = leaderboardId,
            StartPosition = 0,
            MaxResultsCount = 10 // You can change this number to display more entries
        };
        
        PlayFabClientAPI.GetLeaderboard(request, OnGetLeaderboardSuccess, OnGetLeaderboardFailure);
    }

    // Callback for successful leaderboard retrieval
    private void OnGetLeaderboardSuccess(GetLeaderboardResult result)
    {
        // Convert PlayFab leaderboard data to our struct
        List<LeaderboardEntry> leaderboardEntries = result.Leaderboard.Select(entry => new LeaderboardEntry
        {
            playerId = entry.PlayFabId,
            playerName = entry.DisplayName,
            score = entry.StatValue
        }).ToList();

        // Trigger the event to update the UI
        LeaderboardUpdated?.Invoke(leaderboardEntries);
    }

    // Callback for failed leaderboard retrieval
    private void OnGetLeaderboardFailure(PlayFabError error)
    {
        Debug.LogError("Failed to retrieve leaderboard: " + error.GenerateErrorReport());
        // Handle the error, for example, by showing an error message to the player
    }

    // Call this method to submit a score to the leaderboard
    public void SubmitScore(string playerId, int score)
    {
        // Update the player's score on the leaderboard
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
        {
            FunctionName = "UpdatePlayerScore",
            FunctionParameter = new { playerId = playerId, score = score },
            GeneratePlayStreamEvent = true
        };

        PlayFabClientAPI.ExecuteCloudScript(request, OnExecuteCloudScriptSuccess, OnExecuteCloudScriptFailure);
    }

    // Callback for successful Cloud Script execution
    private void OnExecuteCloudScriptSuccess(ExecuteCloudScriptResult result)
    {
        // Successfully updated the score
        // Optionally, refresh the leaderboard to reflect the new score
        UpdateLeaderboard();
    }

    // Callback for failed Cloud Script execution
    private void OnExecuteCloudScriptFailure(PlayFabError error)
    {
        Debug.LogError("Failed to execute cloud script: " + error.GenerateErrorReport());
        // Handle the error, for example, by showing an error message to the player
    }
}