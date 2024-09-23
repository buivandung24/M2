using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Manages all pots in the game and handles pot distribution
public class PotManager : MonoBehaviour
{
    private PotData pots;
    private HandEvaluator handEvaluator;
    public PhotonView photonViewPotManager;
    private GamePlayManager gamePlayManager;

    private void Awake()
    {
        handEvaluator = GetComponent<HandEvaluator>();
        gamePlayManager = FindObjectOfType<GamePlayManager>();
        pots = FindObjectOfType<PotData>();
    }

    // Call this method to create a new pot
    public void CreatePot()
    {
        pots.resetPot();
    }

    // Call this method when a player bets or raises
    // Pass in the amount the player is contributing to the pot
    [PunRPC]
    public void AddToPot(int currentPlayerIndex, int amount)
    {
        pots.AddChips(amount);
        pots.AddPlayer(currentPlayerIndex);
    }

    [PunRPC]
    public void RemovedPlayerPot(int currentPlayerIndex)
    {
        pots.RemovedPlayer(currentPlayerIndex);
    }
}
