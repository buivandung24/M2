using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;

public class UpdateCurrency : MonoBehaviour
{
    public GameObject nameGameobject; // Thêm biến tham chiếu đến Text_Coin
    public TMP_Text value;

    private void Update() {
        UpdateCurrencyValue();
    }

    // private void Start() {
    //     FetchCurrency();
    // }

    // private void OnEnable() {
    //     FetchCurrency();
    // }

    // // Hàm lấy thông tin tiền tệ từ PlayFab
    // public void FetchCurrency()
    // {
    //     // Yêu cầu thông tin tiền tệ của người chơi
    //     PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), OnGetUserInventorySuccess, OnGetUserInventoryFailure);
    // }

    // // Hàm callback khi lấy thông tin thành công
    // private void OnGetUserInventorySuccess(GetUserInventoryResult result)
    // {
    //     // Lưu trữ số lượng coin và diamond
    //     int coins = 0;
    //     int diamonds = 0;

    //     // Lặp qua danh sách tiền tệ và lấy số lượng theo code tương ứng
    //     foreach (var item in result.VirtualCurrency)
    //     {
    //         if (item.Key == "CN")
    //         {
    //             coins = item.Value;
    //         }
    //         else if (item.Key == "DM")
    //         {
    //             diamonds = item.Value;
    //         }
    //     }

    //     // Cập nhật thông tin trên UI
    //     if (nameGameobject != null && nameGameobject.name == "Text_Coin")
    //     {
    //         value.text = coins.ToString();
    //     }
    //     if (nameGameobject != null && nameGameobject.name == "Text_Diamond")
    //     {
    //         value.text = diamonds.ToString();
    //     }

    //     // In thông tin ra console (hoặc xử lý tùy theo nhu cầu của bạn)
    //     Console.WriteLine("Coins: " + coins);
    //     Console.WriteLine("Diamonds: " + diamonds);
    //     // Debug.Log("Done FetchCurrency");
    // }

    // // Hàm callback khi lấy thông tin thất bại
    // private void OnGetUserInventoryFailure(PlayFabError error)
    // {
    //     Console.WriteLine("Error while fetching user inventory: " + error.ErrorMessage);
    // }


    public void UpdateCurrencyValue(){
        if (nameGameobject != null && nameGameobject.name == "Text_Coin")
        {
            value.text = PlayFabManager.instance.userCoins.ToString();
        }
        if (nameGameobject != null && nameGameobject.name == "Text_Diamond")
        {
            value.text = PlayFabManager.instance.userDiamonds.ToString();
        }
    }
}