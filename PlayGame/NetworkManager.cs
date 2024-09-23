using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;

// Quản lý mạng cho game poker NLH
public class NetworkManager : MonoBehaviourPunCallbacks
{
    // Singleton instance
    public static NetworkManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Khởi tạo Photon Network
        //PhotonNetwork.JoinLobby();
        //PhotonNetwork.AutomaticallySyncScene = true; // Đồng bộ cảnh tự động
    }

    private void Update() {
        ConnectToPhoton();
    }

    public void ConnectToPhoton()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings(); // Kết nối đến Photon sử dụng các thiết lập mặc định
        }
    }

    // Khi đã kết nối đến master server của Photon
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server");
        PhotonNetwork.JoinLobby();
    }

    public void JoinRandomRoom(){
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom(){
        PhotonNetwork.LoadLevel("Game");
    }

    // Callback khi vào phòng ngẫu nhiên không thành công
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Không tìm thấy phòng ngẫu nhiên, đang tạo phòng mới...");
        CreateRandomRoom();
    }

    // Tạo phòng với ID ngẫu nhiên
    private void CreateRandomRoom()
    {
        int randomRoomId = Random.Range(1000, 10000); // Tạo một số ngẫu nhiên từ 1000 đến 9999
        string roomID = randomRoomId.ToString(); // Chuyển đổi số ngẫu nhiên thành chuỗi

        // Tạo phòng với ID đã tạo và tối đa 9 người chơi
        PhotonNetwork.CreateRoom(roomID, new RoomOptions { MaxPlayers = 9 });
    }

    // Callback khi tạo phòng không thành công
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Tạo phòng không thành công: " + message + ", đang thử lại...");
        CreateRandomRoom(); // Thử tạo một phòng khác nếu phòng đầu tiên đã tồn tại
    }
}