using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using PlayFab.MultiplayerModels;
using UnityEngine;

public class SettingGame : MonoBehaviourPunCallbacks
{
    public void SitOut(){
        
    }
    public void ExitTable()
    {
        GamePlayManager.instance.LeaveRoom();
    }
}
