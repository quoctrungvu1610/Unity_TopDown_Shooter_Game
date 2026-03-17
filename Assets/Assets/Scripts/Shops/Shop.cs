using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Shop : MonoBehaviour
{
    public class ShopItem
    {
        InventoryItem item;
        int availability;
        float price;
        int quantityInTransaction;
    }

    public event Action onChange;

    public IEnumerable<ShopItem> GetFilteredItems() { return null; }
    public void SelectFilter(ItemCategory category) { }
    public ItemCategory GetFilter() { return ItemCategory.None; }
    public void SelectMode(bool isBuying) { }
    public bool IsBuyingMode() { return true; }
    public bool CanTransact() { return true; }
    public void ConfirmTransaction() { }
    public float TransactionTotal() { return 0f; }
    public void AddToTransaction(Inventory item, int quantity) { }
}
