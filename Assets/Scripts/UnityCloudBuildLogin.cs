using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class UnityCloudBuildLogin : MonoBehaviour
{
    // Địa chỉ API để đăng nhập vào Unity Cloud Build
    private const string loginUrl = "https://build-api.cloud.unity3d.com/api/v1/oauth2/token";

    // Thay thế với API Access Token của bạn từ Unity Cloud
    private const string apiAccessToken = "YOUR_API_ACCESS_TOKEN";

    // Hàm bắt đầu đăng nhập
    public void Login()
    {
        StartCoroutine(LoginCoroutine());
    }

    // Coroutine thực hiện đăng nhập
    IEnumerator LoginCoroutine()
    {
        // Tạo form để gửi dữ liệu đăng nhập
        WWWForm loginForm = new WWWForm();
        loginForm.AddField("grant_type", "client_credentials");

        // Tạo UnityWebRequest và thiết lập header
        UnityWebRequest www = UnityWebRequest.Post(loginUrl, loginForm);
        www.SetRequestHeader("Authorization", "Basic " + apiAccessToken);

        // Thực hiện yêu cầu và chờ kết quả
        yield return www.SendWebRequest();

        // Xử lý kết quả trả về
        if (www.result == UnityWebRequest.Result.Success)
        {
            // Đăng nhập thành công, xử lý thông tin trả về
            Debug.Log("Login success: " + www.downloadHandler.text);
            // Bạn có thể lưu token để sử dụng cho các yêu cầu tiếp theo tới Unity Cloud Build
        }
        else
        {
            // Đã có lỗi xảy ra, in thông tin lỗi
            Debug.LogError("Login failed: " + www.error);
        }
    }
}