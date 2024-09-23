using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.DataModels;
using System.Collections.Generic;
using UnityEngine;


public class ClubLobby : MonoBehaviourPunCallbacks
{
    // Danh sách các phòng thuộc club này
    public List<RoomData> clubRooms = new List<RoomData>();
    public Transform objectParent;
    public GameObject RoomPlayClubPreFab;
    //script
    public ClubInfo clubInfo;

    // Khởi tạo lobby club và đăng ký các sự kiện
    void Awake()
    {
        FetchRoomListFromSharedGroup();
    }

    public void CreateNewRoom()
    {
        int randomRoomId = Random.Range(1000, 10000); // Tạo một số ngẫu nhiên từ 1000 đến 9999
        string roomID = randomRoomId.ToString(); // Chuyển đổi số ngẫu nhiên thành chuỗi
        Debug.Log("Start Create New Room");
        SaveClubRooms();
        FetchRoomListFromSharedGroup();
    }

    private void SaveClubRooms(){
        if(clubInfo.ClubId.text == "") return;
        var request = new UpdateSharedGroupDataRequest
        {
            SharedGroupId = clubInfo.ClubId.text,
            Data = new Dictionary<string, string> {
                { "RoomName", JsonConvert.SerializeObject(clubRooms) }
            },
        };
        PlayFabClientAPI.UpdateSharedGroupData(request, OnUpdateSharedGroupData, OnPlayFabError);
    }

    private void OnUpdateSharedGroupData(UpdateSharedGroupDataResult response)
    {
        Debug.Log("Room created in PlayFab successfully");
        //FetchRoomList(); // Refresh the room list
    }

    private void OnPlayFabError(PlayFabError error)
    {
        Debug.LogError($"PlayFab Error: {error.ErrorMessage}");
    }

    // Fetch room list from PlayFab's SharedGroup custom data
    public void FetchRoomListFromSharedGroup()
    {
        Debug.Log("Fetching room list from PlayFab's SharedGroup");
        var request = new GetSharedGroupDataRequest
        {
            SharedGroupId = clubInfo.ClubId.text,
        };
        PlayFabClientAPI.GetSharedGroupData(request, OnRoomListFromSharedGroupReceived, OnPlayFabError);
    }

    private void OnRoomListFromSharedGroupReceived(GetSharedGroupDataResult result)
    {
        Debug.Log("Received room list from PlayFab's SharedGroup");

        // Clear existing UI elements
        foreach (Transform child in objectParent)
        {
            Destroy(child.gameObject);
        }

        clubRooms.Clear(); // Clear the current list of rooms

        // Assuming "RoomName" key holds the list of room names
        if (result.Data.TryGetValue("RoomName", out var roomNamesEntry) && roomNamesEntry.Value != null)
        {
            List<string> roomNames = JsonConvert.DeserializeObject<List<string>>(roomNamesEntry.Value);
            foreach (string roomName in roomNames)
            {
                // Create a new room prefab instance
                GameObject roomButtonObj = Instantiate(RoomPlayClubPreFab, objectParent);

                // Set the room name on the RoomData component of the prefab
                RoomData roomButton = roomButtonObj.GetComponent<RoomData>();
                if (roomButton != null)
                {
                    roomButton.SetRoomName(roomName);
                    clubRooms.Add(roomButton); // Add to clubRooms list
                }
            }
        }

        UpdateRoomListUI(); // Update the room list UI
    }

    // Tham gia vào một phòng cụ thể
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    // Callback khi đã tham gia lobby thành công
    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Club Lobby");
    }

    // Callback khi tạo phòng mới thành công
    public override void OnCreatedRoom()
    {
        Debug.Log("Room created successfully");
    }

    // Callback khi có lỗi xảy ra khi tạo phòng
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Failed to create room: {message}");
    }

    private void UpdateRoomListUI()
    {
        // Xóa tất cả các room button hiện có
        foreach (Transform child in objectParent)
        {
            Destroy(child.gameObject);
        }

        // Tạo button mới cho mỗi phòng trong clubRooms
        foreach (RoomData roomData in clubRooms)
        {
            GameObject roomButtonObj = Instantiate(RoomPlayClubPreFab, objectParent);
            RoomData roomButtonData = roomButtonObj.GetComponent<RoomData>();
            if (roomButtonData != null)
            {
                roomButtonData.SetRoomName(roomData.RoomName);
            }
        }
    }
}