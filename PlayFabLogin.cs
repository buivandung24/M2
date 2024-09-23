using System;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.Collections;

public class PlayFabLogin : MonoBehaviour
{
    public TMP_InputField usernameOrEmailInput; // Assign in the inspector
    public TMP_InputField passwordInput; // Assign in the inspector
    public GameObject textFailure;
    public Button loginButton; // Assign in the inspector
    public GameObject loadingScene;

    private void Start()
    {
        // Optional: Add listeners to loginButton
        loginButton.onClick.AddListener(OnLoginButtonClicked);
        loadingScene.SetActive(false);
    }

    // Called when the login button is clicked
    public void OnLoginButtonClicked()
    {
    // Make sure that both username/email and password fields are not empty
        if (string.IsNullOrEmpty(usernameOrEmailInput.text) || string.IsNullOrEmpty(passwordInput.text))
        {
            Debug.LogError("Username/Email and Password fields must not be empty.");
            return;
        }

        // Determine if the input is an email or username
        string emailAddressPattern = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$";
        if (System.Text.RegularExpressions.Regex.IsMatch(usernameOrEmailInput.text, emailAddressPattern))
        {
            // Email address provided
            LoginWithEmailAddress(usernameOrEmailInput.text, passwordInput.text);
        }
        else
        {
            // Username provided
            LoginWithUsername(usernameOrEmailInput.text, passwordInput.text);
        }
    }

    // PlayFab login using email and password
    private void LoginWithEmailAddress(string email, string password)
    {
        var request = new LoginWithEmailAddressRequest { Email = email, Password = password };
        loadingScene.SetActive(true);
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
        
    }

    // PlayFab login using username and password
    private void LoginWithUsername(string username, string password)
    {
        var request = new LoginWithPlayFabRequest { Username = username, Password = password };
        loadingScene.SetActive(true);
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnLoginFailure);
    }

    // Callback on successful login
    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Congratulations, you made your successful login!");
        // Retrieve account info to get the PlayFab username
        GetAccountInfoRequest getAccountInfoRequest = new GetAccountInfoRequest { PlayFabId = result.PlayFabId };
        PlayFabClientAPI.GetAccountInfo(getAccountInfoRequest, OnGetAccountInfoSuccess, OnGetAccountInfoFailure);
    }

    // Callback on successful retrieval of account info
    private void OnGetAccountInfoSuccess(GetAccountInfoResult result)
    {
        GetUsernameAndUpdateDisplayName();
        // Check if the display name is available and not empty
        string displayName = result.AccountInfo.TitleInfo.DisplayName;
        if (!string.IsNullOrEmpty(displayName))
        {
            // Set the PhotonNetwork.NickName to the PlayFab display name
            PhotonNetwork.NickName = displayName;
            Debug.Log("Photon NickName set to PlayFab display name: " + PhotonNetwork.NickName);
        }
        else
        {
            // Fallback to the username if the display name is not set
            PhotonNetwork.NickName = result.AccountInfo.Username;
            Debug.Log("Photon NickName set to PlayFab username (DisplayName was empty): " + PhotonNetwork.NickName);
        }

        // Load the next scene now that we have set the nickname
        //SceneManager.LoadScene("Lobby");
        SceneManager.LoadSceneAsync("Lobby");
    }

    // Callback on failure to retrieve account info
    private void OnGetAccountInfoFailure(PlayFabError error)
    {
        loadingScene.SetActive(false);
        Debug.LogError("Could not retrieve PlayFab account info. Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
        // Handle the failure to get account info here
    }

    // Callback on login failure
    private void OnLoginFailure(PlayFabError error)
    {
        loadingScene.SetActive(false);
        Debug.LogError("Something went wrong with your login. Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
        // Add additional logic here for what happens after a login fails
        //textFailure.gameObject.SetActive(true);
    }

    // Hàm này gọi khi bạn muốn lấy username và cập nhật display name
    public void GetUsernameAndUpdateDisplayName()
    {
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), OnGetUsernameSuccess, OnPlayFabError);
    }

    private void OnGetUsernameSuccess(GetAccountInfoResult result)
    {
        // Lấy username từ Master Player Account
        string username = result.AccountInfo.Username;

        // Kiểm tra nếu DisplayName là null hoặc rỗng
        if (string.IsNullOrEmpty(result.AccountInfo.TitleInfo.DisplayName))
        {
            // Nếu DisplayName không tồn tại, cập nhật DisplayName với username
            UpdateDisplayName(username);
        }
        else
        {
            // Nếu DisplayName đã tồn tại, có thể thực hiện các hành động khác tại đây
            Debug.Log("Display Name is already set to: " + result.AccountInfo.TitleInfo.DisplayName);
        }
    }

    private void UpdateDisplayName(string displayName)
    {
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = displayName
        }, OnUpdateDisplayNameSuccess, OnPlayFabError);
    }

    private void OnUpdateDisplayNameSuccess(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("Display Name updated successfully!");
    }


    // Hàm được gọi khi có lỗi xảy ra
    private void OnPlayFabError(PlayFabError error)
    {
        Debug.LogError("Error while communicating with PlayFab: " + error.ErrorMessage);
    }

}