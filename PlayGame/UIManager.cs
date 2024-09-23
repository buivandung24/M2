using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using Photon.Pun;
using System.Collections;
using Unity.VisualScripting;

public class UIManager : MonoBehaviourPun
{
    [Header("Player UI")]
    public TMP_Text potText; // Text UI for displaying the pot amount
    public TMP_Text potTextMain; // Text UI for displaying the pot amount

    [Header("Action UI")]
    public Button callButton; // Call action button
    public Button foldButton; // Fold action button
    public Button raiseButton; // Raise action button
    public GameObject raisePanel;

    [Header("Room UI")]
    public TMP_Text roomIDText; // TMP UI for displaying room ID
    //time
    public TMP_Text timeCountDown;
    public float turnDuration = 30f; // Thời gian tối đa của một lượt chơi, ví dụ 30 giây
    public float turnTimer;
    //photon view
    public PhotonView photonViewUI;

    //class
    private PotData potData;
    private GamePlayManager gamePlayManager;
    private PlayerCreator playerCreator;
    private PotManager potManager;

    //variable
    public int smallBlind;
    public int bigBlind;
    public int bet;
    public GameObject myPlayer;
    public GameObject darkScene;
    public GameObject InfoPersonalScene;

    void Awake()
    {
        photonViewUI = GetComponent<PhotonView>();
        potData = FindObjectOfType<PotData>();
        gamePlayManager = FindObjectOfType<GamePlayManager>();
        playerCreator = FindObjectOfType<PlayerCreator>();
        potManager = FindObjectOfType<PotManager>();
    }
    private void Start() {
        UpdateRoomID();
        //variable
        potText.text = "0";
        potTextMain.text = potText.text;
        smallBlind = 500;
        bigBlind = 1000;
        bet = bigBlind;
        
    }

    private void Update() {
        potText.text = potData.Amount + "";
        updateButtonPlayGame(myPlayer.GetComponent<PlayerData>().canPlay);
        if(!raiseButton.gameObject.activeSelf) raisePanel.SetActive(false);
    }

    // Call this method to update the pot UI
    [PunRPC]
    public void UpdatePot()
    {
        potTextMain.text = potText.text;
    }

    // Photon Remote Procedure Call for a player's call
    [PunRPC]
    public void PlayerCall(int currentPlayerIndex)
    {
        myPlayer.GetComponent<PlayerData>().canPlay = false;
        myPlayer.GetComponent<PlayerData>().photonViewPlayer.RPC("SetCanPlay", RpcTarget.All, false);
        if(PhotonNetwork.IsMasterClient){
            potManager.photonViewPotManager.RPC("AddToPot", RpcTarget.All, currentPlayerIndex, bet);
            gamePlayManager.PlayersInOrder[currentPlayerIndex].GetComponent<PlayerData>().photonViewPlayer.RPC("AddChips", RpcTarget.All, -bet);
            photonView.RPC("UpdateGameState", RpcTarget.All, potData.Amount);
            gamePlayManager.photonViewGameManager.RPC("OnStopAllCoroutines", RpcTarget.MasterClient);
            Debug.Log("PlayerCall done");
        }
    }

    // Photon Remote Procedure Call for a player's fold
    [PunRPC]
    public void PlayerFold(int currentPlayerIndex)
    {
        myPlayer.GetComponent<PlayerData>().canPlay = false;
        myPlayer.GetComponent<PlayerData>().photonViewPlayer.RPC("SetCanPlay", RpcTarget.All, false);
        if(PhotonNetwork.IsMasterClient){
            potManager.photonViewPotManager.RPC("RemovedPlayerPot", RpcTarget.All, currentPlayerIndex);
            gamePlayManager.PlayersInOrder[currentPlayerIndex].GetComponent<PlayerData>().photonViewPlayer.RPC("OnsetHasFolded", RpcTarget.All, true);
            gamePlayManager.photonViewGameManager.RPC("OnStopAllCoroutines", RpcTarget.MasterClient);
            Debug.Log("PlayerFold done");
        }
    }

    // Photon Remote Procedure Call for a player's raise
    [PunRPC]
    public void PlayerRaise(int amount, int currentPlayerIndex)
    {
        myPlayer.GetComponent<PlayerData>().canPlay = false;
        myPlayer.GetComponent<PlayerData>().photonViewPlayer.RPC("SetCanPlay", RpcTarget.All, false);
        if(PhotonNetwork.IsMasterClient){
            if(amount <= 16){
                bet *= amount;
            } else {
                bet = gamePlayManager.PlayersInOrder[currentPlayerIndex].GetComponent<PlayerData>().Chips;
            }
            photonViewUI.RPC("UpdateBet", RpcTarget.All, bet);
            potManager.AddToPot(currentPlayerIndex, bet);
            //photonViewUI.RPC("UpdateListEligiblePlayers", RpcTarget.All, currentPlayerIndex);
            gamePlayManager.PlayersInOrder[currentPlayerIndex].GetComponent<PlayerData>().photonViewPlayer.RPC("AddChips", RpcTarget.All, -bet);
            photonView.RPC("UpdateGameState", RpcTarget.All, potData.Amount);
            gamePlayManager.photonViewGameManager.RPC("OnStopAllCoroutines", RpcTarget.MasterClient);
            Debug.Log("PlayerCall done");
        }
    }

    [PunRPC]
    private void UpdateGameState(int updatedPotAmount)
    {
        potData.Amount = updatedPotAmount;
    }

    [PunRPC]
    private void UpdateBet(int updatedBet)
    {
        bet = updatedBet;
    }

    
    // Call this method to update the room ID UI
    public void UpdateRoomID()
    {
        if (PhotonNetwork.InRoom)
        {
            roomIDText.text = PhotonNetwork.CurrentRoom.Name;
        }
    }

    public void resetBet(){
        bet = bigBlind;
    }

    public void updateButtonPlayGame(bool isActive){
        callButton.gameObject.SetActive(isActive);
        raiseButton.gameObject.SetActive(isActive);
        foldButton.gameObject.SetActive(isActive);
    }
}