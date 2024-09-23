using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using TMPro;
using Photon.Pun;


public class PlayFabLeaderboardManager : MonoBehaviour
{
    public GameObject itemLeaderboardPrefab;
    public Transform objectParent;

    private void Start() {
        GetLeaderboard();
        UpdatePlayerWinCount(1);
    }

    private void OnEnable() {
        GetLeaderboard();
    }
    // Hàm lấy thông tin leaderboard từ PlayFab
    public void GetLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "Winning", // Tên leaderboard
            StartPosition = 0, // Bắt đầu từ vị trí đầu tiên
            MaxResultsCount = 100 // Số lượng người chơi tối đa để hiển thị
        };

        PlayFabClientAPI.GetLeaderboard(request, OnGetLeaderboardSuccess, OnPlayFabError);
    }

    // Hàm được gọi khi lấy leaderboard thành công
    private void OnGetLeaderboardSuccess(GetLeaderboardResult result)
    {
        // Clear previous leaderboard entries to avoid duplicating them
        foreach (Transform child in objectParent)
        {
            Destroy(child.gameObject);
        }
        
        foreach (var item in result.Leaderboard)
        {
            // Instantiate a new leaderboard item prefab
            GameObject newLeaderboardItem = Instantiate(itemLeaderboardPrefab, objectParent);
            
            // Set the data for the new leaderboard item
            // Access the LeaderboardItem component and set the text values
            LeaderboardItem leaderboardItem = newLeaderboardItem.GetComponent<LeaderboardItem>();
            leaderboardItem.playerNameText.text = item.DisplayName ?? item.PlayFabId; // Use DisplayName if available, otherwise PlayFabId
            if(item.Position < 10){
                leaderboardItem.ratingsText.text = "0" + (item.Position + 1).ToString(); // The player's score
            } else {
                leaderboardItem.ratingsText.text = (item.Position + 1).ToString(); // The player's score
            }
            
            
            // Debug log to show information in the console
            Debug.Log(item.Position + " - " + item.PlayFabId + " - " + item.StatValue);
        }
    }

    // Hàm được gọi khi có lỗi xảy ra
    private void OnPlayFabError(PlayFabError error)
    {
        Debug.LogError("Error while communicating with PlayFab: " + error.ErrorMessage);
    }

    // Hàm cập nhật số trận thắng cho người chơi
    public void UpdatePlayerWinCount(int winsToAdd)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "Winning", // Tên leaderboard
                    Value = winsToAdd // Giá trị để cộng vào số trận chiến thắng hiện tại
                    //Value = 1
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnUpdatePlayerWinCountSuccess, OnPlayFabError);
    }

    // Hàm được gọi khi cập nhật số trận chiến thắng thành công
    private void OnUpdatePlayerWinCountSuccess(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Player win count successfully updated.");
    }
}