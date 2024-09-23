using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayFabManager : MonoBehaviour
{
    public static PlayFabManager instance;
    public int userCoins;
    public int userDiamonds;

    // Start is called before the first frame update
    void Awake()
    {
        // Có thể gọi hàm lấy thông tin ngay khi bắt đầu nếu bạn muốn
        instance = this;
        //GetUsernameAndUpdateDisplayName();
        GetCurrency();
        DontDestroyOnLoad(gameObject);
    }

    // Hàm lấy thông tin về số tiền và kim cương từ PlayFab
    public void GetCurrency()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), OnGetUserInventorySuccess, OnError);
    }

    void OnGetUserInventorySuccess(GetUserInventoryResult result)
    {
        foreach (var item in result.VirtualCurrency)
        {
            switch (item.Key)
            {
                case "CN": 
                    userCoins = item.Value;
                    break;
                case "DM": 
                    userDiamonds = item.Value;
                    break;
                default:
                    break;
            }
        }
        // Cập nhật UI hoặc thực hiện hành động sau khi có thông tin
    }

    void OnError(PlayFabError error)
    {
        Debug.LogError("Error while communicating with PlayFab: " + error.GenerateErrorReport());
    }

    // // Hàm này gọi khi bạn muốn lấy username và cập nhật display name
    // public void GetUsernameAndUpdateDisplayName()
    // {
    //     PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), OnGetAccountInfoSuccess, OnPlayFabError);
    // }

    // private void OnGetAccountInfoSuccess(GetAccountInfoResult result)
    // {
    //     // Lấy username từ Master Player Account
    //     string username = result.AccountInfo.Username;

    //     // Kiểm tra nếu DisplayName là null hoặc rỗng
    //     if (string.IsNullOrEmpty(result.AccountInfo.TitleInfo.DisplayName))
    //     {
    //         // Nếu DisplayName không tồn tại, cập nhật DisplayName với username
    //         UpdateDisplayName(username);
    //     }
    //     else
    //     {
    //         // Nếu DisplayName đã tồn tại, có thể thực hiện các hành động khác tại đây
    //         Debug.Log("Display Name is already set to: " + result.AccountInfo.TitleInfo.DisplayName);
    //     }
    // }

    // private void UpdateDisplayName(string displayName)
    // {
    //     PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
    //     {
    //         DisplayName = displayName
    //     }, OnUpdateDisplayNameSuccess, OnPlayFabError);
    // }

    // private void OnUpdateDisplayNameSuccess(UpdateUserTitleDisplayNameResult result)
    // {
    //     Debug.Log("Display Name updated successfully!");
    // }


    // // Hàm được gọi khi có lỗi xảy ra
    // private void OnPlayFabError(PlayFabError error)
    // {
    //     Debug.LogError("Error while communicating with PlayFab: " + error.ErrorMessage);
    // }
}