using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    private GamePlayManager gamePlayManager;
    private UIManager uIManager;
    private PhotonView photonViewButton;

    private void Awake() {
        gamePlayManager = FindObjectOfType<GamePlayManager>();
        uIManager = FindObjectOfType<UIManager>();
        uIManager.raisePanel.SetActive(false);
    }
    public void TurnOffScene(GameObject scene){
        scene.SetActive(false);
    }

    public void TurnOnScene(GameObject scene){
        scene.SetActive(true);
    }

    public void StartGameButton(){
        gamePlayManager.photonViewGameManager.RPC("StartGame", RpcTarget.All);
    }

    // Call this method when the Call button is clicked
    public void OnCallButtonClicked()
    {
        Debug.Log("Start Call");
        int currentPlayerIndex = gamePlayManager.currentPlayerIndex;
        //gamePlayManager.playerMasterClient.GetComponent<PlayerData>().uIManager.photonViewUI.RPC("PlayerCall", RpcTarget.MasterClient, currentPlayerIndex);
        uIManager.photonViewUI.RPC("PlayerCall", RpcTarget.MasterClient, currentPlayerIndex);
    }

    // Call this method when the Fold button is clicked
    public void OnFoldButtonClicked()
    {
        Debug.Log("Start Fold");
        int currentPlayerIndex = gamePlayManager.currentPlayerIndex;
        // Debug.Log("User ID: " + gamePlayManager.playerMasterClient.GetComponent<PhotonView>().ViewID);
        uIManager.photonViewUI.RPC("PlayerFold", RpcTarget.MasterClient, currentPlayerIndex);
    }

    public void OnRaiseButtonClicked()
    {
        Debug.Log("Start Raise");
        int currentPlayerIndex = gamePlayManager.currentPlayerIndex;
        if(uIManager.raisePanel.activeSelf){
            uIManager.photonViewUI.RPC("PlayerRaise", RpcTarget.MasterClient, uIManager.raisePanel.GetComponent<RaiseSliderController>().getValueRaise(), currentPlayerIndex);
        } else {
            uIManager.raisePanel.SetActive(true);
        }
    }
}
