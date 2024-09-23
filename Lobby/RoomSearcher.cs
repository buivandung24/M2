using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class RoomSearcher : MonoBehaviourPunCallbacks
{
    public TMP_InputField roomIDInputField; // InputField để người dùng nhập ID
    public Button searchButton; // Button để tìm phòng

    private void Awake()
    {
        // Đăng ký sự kiện click cho button
        searchButton.onClick.AddListener(OnSearchButtonClicked);
    }

    private void OnSearchButtonClicked()
    {
        string roomID = roomIDInputField.text;

        // Kiểm tra xem ID có hợp lệ (4 chữ số) không
        if (!string.IsNullOrEmpty(roomID) && roomID.Length == 4)
        {
            // Cố gắng vào phòng có ID đã nhập
            PhotonNetwork.JoinRoom(roomID);
        }
    }

    // Callback khi vào phòng bằng ID không thành công
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        // Không tìm thấy phòng với ID, hiển thị thông báo lỗi hoặc thực hiện hành động khác
        Debug.LogError("Không thể vào phòng có ID: " + roomIDInputField.text + ". Lỗi: " + message);
    }
}