using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.GroupsModels;
using TMPro;
using UnityEngine;


public class ClubInfo : MonoBehaviour
{
    public TMP_Text ClubId;
    public TMP_Text ClubName;
    public List<ClubMember> ClubMembers = new List<ClubMember>();
    public TMP_Text DescriptionClub;
    public ClubLobby clubLobby;

    private void Update() {
        
    }

    public void SetClubInfo(string SetClubId, string SetClubName, List<ClubMember> SetClubMembers, string SetDescriptionClub) {
        ClubId.text = "ID: " + SetClubId;
        ClubName.text = SetClubName;
        DescriptionClub.text = SetDescriptionClub;
        ClubMembers = SetClubMembers;
    }

    public void SetNewClubName(string NewClubName){
        ClubName.text = NewClubName;
    }

    public void SetNewDescriptionClub(string NewDescriptionClub){
        DescriptionClub.text = NewDescriptionClub;
    }

    public void AddNewMenbers(ClubMember newMenBer){
        ClubMembers.Add(newMenBer);
    }

    // Method to get club details from PlayFab
    public void GetClubDetails(string groupId)
    {
        Debug.Log("start Club Details");
        var request = new GetGroupRequest
        {
            Group = new EntityKey { Id = groupId }
        };

        PlayFabGroupsAPI.GetGroup(request, OnGetClubDetailsSuccess, OnGetClubDetailsFailure);
    }

    private void OnGetClubDetailsSuccess(GetGroupResponse response)
    {
        SetClubInfo(response.Group.Id, response.GroupName, ClubMembers, ""); // replace with actual description
        Debug.Log("Get Club Details Done");
        //clubLobby.FetchRoomList();
    }

    private void OnGetClubDetailsFailure(PlayFabError error)
    {
        Debug.LogError("Failed to get club details: " + error.GenerateErrorReport());
    }
}


