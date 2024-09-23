using UnityEngine;
using Photon.Pun;
using PlayFab;
using System.Collections.Generic;
using PlayFab.GroupsModels;
using Newtonsoft.Json;
using TMPro;
using PlayFab.ClientModels;
using System;


public class ClubManager : MonoBehaviour
{
    public Panel panel;
    //create club
    public TMP_Text textClub;
    public GameObject createClubScene;
    public TMP_InputField nameInputField;
    public TMP_InputField descriptionInputField;
    //club main
    public GameObject clubScene;
    public ClubInfo clubInfo;
    public ClubLobby clubLobby;
    //setting club
    

    // Thực hiện khi script bắt đầu
    void Awake()
    {

    }

    // Phương thức kiểm tra vai trò và xử lý nút bấm
    public void CheckRoleAndHandleButton()
    {
        var request = new PlayFab.ClientModels.GetAccountInfoRequest();
        PlayFabClientAPI.GetAccountInfo(request, result =>
        {
            string titlePlayerAccountId = result.AccountInfo.TitleInfo.TitlePlayerAccount.Id;
            Debug.Log("ID người chơi: " + titlePlayerAccountId);
            ListMembership(titlePlayerAccountId);
        }, OnError);
        Debug.Log("Check Role And Handle Button");
    }

    private void ListMembership(string playFabId)
    {
        var request = new ListMembershipRequest
        {
            Entity = new PlayFab.GroupsModels.EntityKey
            {
                Id = playFabId,
                Type = "title_player_account"
            }
        };

        PlayFabGroupsAPI.ListMembership(request, OnListMembership, OnError);
        Debug.Log("List Member ship");
    }

    private void OnListMembership(ListMembershipResponse response)
    {
        foreach (var group in response.Groups)
        {
            foreach (var role in group.Roles)
            {
                if (role.RoleName == "Administrators")
                {
                    Debug.Log("User is an Administrator of group: " + group.GroupName);
                    Debug.Log("Navigate To Group: " + group.Group.Id);
                    NavigateToGroup(group.Group.Id);
                    
                    return;
                }
            }
        }

        // Player is not an administrator of any group, prompt to create a new club
        createClubScene.SetActive(true);
        Debug.Log("Player is not an administrator of any group.");
    }

    private void NavigateToGroup(string groupId)
    {
        panel.TurnOnScene(clubScene);
        clubInfo.GetClubDetails(groupId);
        Debug.Log("Navigating to group with ID: " + groupId);
        // clubLobby.FetchRoomList();
    }

    // Call this method when you want to create a new club
    public void CreateClub()
    {
        var createGroupRequest = new CreateGroupRequest
        {
            GroupName = nameInputField.text,
            // You can pass additional data such as a description using Entity Data
        };

        PlayFabGroupsAPI.CreateGroup(createGroupRequest, OnClubCreated, OnError);
        Debug.Log("Create Club");
    }

    private void OnClubCreated(CreateGroupResponse response)
    {
        // Club was created successfully
        Debug.Log("Club created with ID: " + response.Group.Id);
        string groupId = response.Group.Id;
        // Create a shared group for the new club
        var createSharedGroupRequest = new CreateSharedGroupRequest
        {
            SharedGroupId = groupId,
        };
        
        PlayFabClientAPI.CreateSharedGroup(createSharedGroupRequest, result =>
        {
            // Shared group created successfully, save the SharedGroup ID
            Debug.Log("Shared group created with ID: " + result.SharedGroupId);
            SaveGroupDataToSharedGroupId(groupId, result.SharedGroupId);
            
            // Continue with the rest of the club setup
            panel.TurnOnScene(clubScene);
            clubInfo.SetClubInfo(response.Group.Id, response.GroupName, null, "");
            createClubScene.SetActive(false);
        }, OnError);
    }

    // Implement this method to save the SharedGroup ID to your club's data
    private void SaveGroupDataToSharedGroupId(string groupId, string sharedGroupId)
    {
        // Save the SharedGroup ID using UpdateGroupData
        var updateGroupDataRequest = new UpdateSharedGroupDataRequest
        {
            SharedGroupId = sharedGroupId,
            Data = new Dictionary<string, string>
            {
                { "GroupId", groupId }
            },
            Permission = UserDataPermission.Public // or OperationTypes.Group
        };

        PlayFabClientAPI.UpdateSharedGroupData(updateGroupDataRequest, result =>
        {
            Debug.Log("Save Group Data To Shared GroupId successfully.");
        }, OnError);
    }

    private void OnError(PlayFabError error)
    {
        // Handle error scenario
        Debug.LogError("Error creating club: " + error.GenerateErrorReport());
    }


    // Tham gia vào một club
    public void JoinClub(string clubId)
    {
        // Logic để tham gia vào club sử dụng PlayFab
        // Cần thêm các tham số cần thiết và gọi API PlayFab để tham gia vào club
    }

    // Rời khỏi club
    public void LeaveClub(string clubId)
    {
        // Logic để rời khỏi club sử dụng PlayFab
        // Cần thêm các tham số cần thiết và gọi API PlayFab để rời khỏi club
    }

    // Mời người chơi vào club
    public void InvitePlayerToClub(string playerId)
    {
        // Logic để mời một người chơi vào club sử dụng PlayFab và/hoặc Photon
        // Cần thêm các tham số cần thiết và gọi API PlayFab/Photon để gửi lời mời
    }

    // Cập nhật danh sách thành viên trong club
    private void UpdateClubMemberList()
    {
        
    }

    // Cập nhật thông tin club
    private void UpdateClubInfo()
    {
        
    }

    // Làm mới thông tin club và danh sách thành viên
    public void RefreshClubData()
    {
        UpdateClubInfo();
        UpdateClubMemberList();
    }
}