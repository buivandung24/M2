using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class ItemShop : MonoBehaviour
{
    public enum ItemType
    {
        Coin,
        Diamond,
        Item
    }

    public enum CurrencyType
    {
        Diamond,
        RealMoney
    }

    private const string VIRTUAL_CURRENCY_COIN = "CN";
    private const string VIRTUAL_CURRENCY_DIAMOND = "DM";

    public string nameItem;
    public int priceItem;
    public int itemValue;
    public ItemType itemType;
    public CurrencyType currencyType;

    void Start()
    {
        // Logic xác định loại item và phương thức thanh toán
        // ...
    }

    public void PurchaseItem()
    {
        string virtualCurrency;
        switch (currencyType)
        {
            case CurrencyType.Diamond:
                virtualCurrency = VIRTUAL_CURRENCY_DIAMOND;
                break;
            case CurrencyType.RealMoney:
                // Thực hiện logic xử lý thanh toán bằng tiền thật ở đây
                return;
            default:
                Debug.LogError("Loại tiền thanh toán không hợp lệ!");
                return;
        }

        var request = new PurchaseItemRequest
        {
            ItemId = nameItem,
            VirtualCurrency = virtualCurrency,
            Price = priceItem
        };
        PlayFabClientAPI.PurchaseItem(request, OnPurchaseItemSuccess, OnError);
    }

    public void AddUserVirtualCurrency()
    {
        string virtualCurrencyCode;
        switch (itemType)
        {
            case ItemType.Coin:
                virtualCurrencyCode = VIRTUAL_CURRENCY_COIN;
                break;
            case ItemType.Diamond:
                virtualCurrencyCode = VIRTUAL_CURRENCY_DIAMOND;
                break;
            default:
                Debug.LogError("Loại item không hợp lệ!");
                return;
        }

        int Value = 0;
        if(virtualCurrencyCode == VIRTUAL_CURRENCY_COIN){
            Value = PlayFabManager.instance.userDiamonds;
        } else if(virtualCurrencyCode == VIRTUAL_CURRENCY_DIAMOND){
            Value = priceItem;
        }

        if(Value >= priceItem){
            var request = new AddUserVirtualCurrencyRequest
            {
                VirtualCurrency = virtualCurrencyCode,
                Amount = itemValue
            };
            PlayFabClientAPI.AddUserVirtualCurrency(request, OnAddUserVirtualCurrencySuccess, OnError);
        }
        
    }

    public void SubtractUserVirtualCurrency()
    {
        string virtualCurrencyCode;
        switch (itemType)
        {
            case ItemType.Coin:
                virtualCurrencyCode = VIRTUAL_CURRENCY_DIAMOND;
                break;
            case ItemType.Diamond:
                virtualCurrencyCode = VIRTUAL_CURRENCY_COIN;
                break;
            default:
                Debug.LogError("Invalid item type!");
                return;
        }

        int Value = 0;
        if(virtualCurrencyCode == VIRTUAL_CURRENCY_DIAMOND){
            Value = PlayFabManager.instance.userDiamonds;
        } else if(virtualCurrencyCode == VIRTUAL_CURRENCY_COIN){
            Value = priceItem;
        }

        if(Value >= priceItem && priceItem != 0){
            var request = new SubtractUserVirtualCurrencyRequest
            {
                VirtualCurrency = virtualCurrencyCode,
                Amount = priceItem
            };
            PlayFabClientAPI.SubtractUserVirtualCurrency(request, OnSubtractUserVirtualCurrencySuccess, OnError);
        }
        
    }

    private void OnSubtractUserVirtualCurrencySuccess(ModifyUserVirtualCurrencyResult result)
    {
        Debug.Log("Virtual currency subtracted successfully");
        PlayFabManager.instance.GetCurrency();
        // You may want to update the user interface or perform other actions here
    }

    private void OnPurchaseItemSuccess(PurchaseItemResult result)
    {
        Debug.Log("Item purchased successfully");
        // Handle success
    }

    private void OnAddUserVirtualCurrencySuccess(ModifyUserVirtualCurrencyResult result)
    {
        Debug.Log("Virtual currency added successfully");
        PlayFabManager.instance.GetCurrency();
    }

    private void OnError(PlayFabError error)
    {
        Debug.LogError("Error while calling PlayFab API: " + error.ErrorMessage);
        // Handle error
    }

}