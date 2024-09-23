using UnityEngine;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Realtime;
using System.Collections.Generic;

public class PlayerCreator : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public Transform[] spawnPoints;
    public GameObject player;
    //class
    private GamePlayManager gamePlayManager;
    private UIManager uIManager;
    private void Awake()
    {
        GetPlayerDisplayNameFromPlayFab();
        gamePlayManager = FindObjectOfType<GamePlayManager>();
        uIManager = FindObjectOfType<UIManager>();
    }

    public void GetPlayerDisplayNameFromPlayFab()
    {
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), result =>
        {
            string playFabPlayerName = result.AccountInfo.Username;

            // Removed the check and assignment for playFabPlayerNameText

            CreatePlayer(playFabPlayerName);
        }, error =>
        {
            Debug.LogError("Could not get PlayFab player name: " + error.GenerateErrorReport());
        });
    }

    public void CreatePlayer(string playerName)
    {
        if(PhotonNetwork.LocalPlayer.IsLocal){
            int playerChips = GetPlayerChipsFromPlayFab();
            int spawnIndex = FindAvailableSeat();

            if (spawnIndex >= 0)
            {
                player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoints[spawnIndex]. position, Quaternion.identity);
                gamePlayManager.myPlayer = player;
                uIManager.myPlayer = player;
                ExitGames.Client.Photon.Hashtable playerCustomProps = new ExitGames.Client.Photon.Hashtable
                {
                    { "spawnIndex", spawnIndex }
                };
                PhotonNetwork.SetPlayerCustomProperties(playerCustomProps);
            }
            else
            {
                Debug.LogError("Không có vị trí trống trên bàn.");
            }
        }
    }

    private int GetPlayerChipsFromPlayFab()
    {
        // This should likely be replaced with a PlayFab API call to get the actual chips value
        return PlayerPrefs.GetInt("playerChips", 1000);
    }

    public int FindAvailableSeat()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (spawnPoints[i].childCount == 0)
            {
                return i;
            }
        }
        return -1;
    }
}
