using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    // Mã sản phẩm của vật phẩm bạn muốn mua
    private string _itemProductId = "some_product_id";

    // Hàm mua vật phẩm
    public void PurchaseItem(string itemId)
    {
        // Gọi API purchase item của PlayFab
        var request = new PurchaseItemRequest
        {
            ItemId = itemId,
            VirtualCurrency = "CN", // Sử dụng tiền tệ "CN" để mua
            Price = 100 // Giả sử giá của vật phẩm là 100 CN
        };

        PlayFabClientAPI.PurchaseItem(request, OnPurchaseItemSuccess, OnPlayFabError);
    }

    // Callback khi mua hàng thành công
    private void OnPurchaseItemSuccess(PurchaseItemResult result)
    {
        Debug.Log("Purchase successful!");
        // Cập nhật số dư tiền tệ hoặc xử lý logic game của bạn ở đây
        // ...
    }

    // Hàm trừ tiền
    public void SubtractCurrency(string currencyCode, int amount)
    {
        var request = new SubtractUserVirtualCurrencyRequest
        {
            VirtualCurrency = currencyCode,
            Amount = amount
        };

        PlayFabClientAPI.SubtractUserVirtualCurrency(request, OnSubtractCurrencySuccess, OnPlayFabError);
    }

    // Callback khi trừ tiền thành công
    private void OnSubtractCurrencySuccess(ModifyUserVirtualCurrencyResult result)
    {
        Debug.Log($"Successfully subtracted {result.BalanceChange} from currency {result.VirtualCurrency}");
        // Cập nhật số dư tiền tệ hoặc xử lý logic game của bạn ở đây
        // ...
    }

    // Hàm thêm tiền
    public void AddCurrency(string currencyCode, int amount)
    {
        var request = new AddUserVirtualCurrencyRequest
        {
            VirtualCurrency = currencyCode,
            Amount = amount
        };

        PlayFabClientAPI.AddUserVirtualCurrency(request, OnAddCurrencySuccess, OnPlayFabError);
    }

    // Callback khi thêm tiền thành công
    private void OnAddCurrencySuccess(ModifyUserVirtualCurrencyResult result)
    {
        Debug.Log($"Successfully added {result.BalanceChange} to currency {result.VirtualCurrency}");
        // Cập nhật số dư tiền tệ hoặc xử lý logic game của bạn ở đây
        // ...
    }

    // Callback khi có lỗi từ PlayFab
    private void OnPlayFabError(PlayFabError error)
    {
        Debug.LogError($"Error while communicating with PlayFab: {error.ErrorMessage}");
        // Xử lý lỗi ở đây
        // ...
    }
}