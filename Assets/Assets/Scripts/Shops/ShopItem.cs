using System;
using UnityEngine;

public class ShopItem
{
    InventoryItem item;
    int availability;
    float price;
    int quantityInTransaction;

    public ShopItem(InventoryItem item, int availability, float price, int quantityInTransaction)
    {
        this.item = item;
        this.availability = availability;
        this.price = price;
        this.quantityInTransaction = quantityInTransaction;
    }

    public string GetName()
    {
        return item.GetDisplayName();
    }

    public Sprite GetIcon()
    {
        return item.GetIcon();
    }

    public int GetAvailability()
    {
        return availability;
    }

    public float GetPrice()
    {
        return price;
    }

    public InventoryItem GetInventoryItem()
    {
        return item;
    }

    public int GetQuantityInTransaction() 
    {
        return quantityInTransaction;
    }
}