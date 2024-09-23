using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PotData : MonoBehaviour
{
    public int Amount;
    public List<PlayerData> EligiblePlayers;
    public PhotonView photonViewPotData;
    private GamePlayManager gamePlayManager;

    private void Awake() {
        gamePlayManager = FindObjectOfType<GamePlayManager>();
    }

    public void resetPot()
    {
        Amount = 0;
        EligiblePlayers = new List<PlayerData>();
    }

    public List<PlayerData> GetEligiblePlayers(){
        return new List<PlayerData>(EligiblePlayers);
    }

    //[PunRPC]
    public void AddChips(int amount)
    {
        Amount += amount;
    }

    //[PunRPC]
    public void AddPlayer(int currentPlayerIndex)
    {
        PlayerData player = gamePlayManager.PlayersInOrder[currentPlayerIndex].GetComponent<PlayerData>();
        if (!EligiblePlayers.Contains(player))
        {
            EligiblePlayers.Add(player);
        }
    }

    //[PunRPC]
    public void RemovedPlayer(int currentPlayerIndex){
        PlayerData player = gamePlayManager.PlayersInOrder[currentPlayerIndex].GetComponent<PlayerData>();
        if (EligiblePlayers.Contains(player))
        {
            EligiblePlayers.Remove(player);
        }
    }
}
