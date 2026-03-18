using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Shop : Interactable
{
    [System.Serializable]
    class StockItemConfig
    {
        public InventoryItem item;
        public int initialStock;
        [Range(0f, 100f)]
        public float buyingDiscountPercent;
    }

    [SerializeField] private string shopName;
    [SerializeField] private StockItemConfig[] stockConfig;
    private Dictionary<InventoryItem, int> transaction = new Dictionary<InventoryItem, int>();
    private Dictionary<InventoryItem, int> stock = new Dictionary<InventoryItem, int>();
    private Shopper currentShopper = null;
    public event Action onChange;

    protected override void Awake()
    {
        base.Awake();
        foreach (var config in stockConfig) 
        {
            stock[config.item] = config.initialStock;
        }
    }


    public void SetShopper(Shopper shopper)
    {
        currentShopper = shopper;
    }

    public IEnumerable<ShopItem> GetFilteredItems()
    {
        return GetAllItems();
    }

    public IEnumerable<ShopItem> GetAllItems()
    {
        foreach (StockItemConfig config in stockConfig)
        {
            float price = config.item.GetPrice() * (1 - config.buyingDiscountPercent / 100f);
            int quantityInTransaction = 0;
            transaction.TryGetValue(config.item, out quantityInTransaction);
            int currentStock = stock[config.item];
            yield return new ShopItem(config.item, currentStock, price, quantityInTransaction);
        }
    }

    public void SelectFilter(ItemCategory category) { }
    public ItemCategory GetFilter() { return ItemCategory.None; }
    public void SelectMode(bool isBuying) { }
    public bool IsBuyingMode() { return true; }

    public bool CanTransact() 
    {
        if (IsTransactionEmpty()) return false;
        if (!HasSufficientFunds()) return false;
        if (!HasInventorySpace()) return false;

        return true; 
    }

    public bool HasSufficientFunds()
    {
        Purse purse = currentShopper.GetComponent<Player>().purse;
        if (purse == null)
        {
            return false;
        }

        return purse.GetBalance() >= TransactionTotal();
    }
    private bool IsTransactionEmpty()
    {
        return transaction.Count == 0;
    }

    public bool HasInventorySpace()
    {
        Inventory shopperInventory = currentShopper.GetComponent<Player>().inventory;
        if(shopperInventory == null) return false;
        List<InventoryItem> flatItems = new List<InventoryItem> ();
        
        foreach (ShopItem shopItem in GetAllItems()) 
        {
            InventoryItem item = shopItem.GetInventoryItem();
            int quantity = shopItem.GetQuantityInTransaction();
            for (int i = 0; i < quantity; i++) 
            {
                flatItems.Add(item);
            }
        }

        return shopperInventory.HasSpaceFor(flatItems);

    }

    public void ConfirmTransaction()
    {
        Inventory shopperInventory = currentShopper.GetComponent<Player>().inventory;
        Purse shopperPurse = currentShopper.GetComponent<Player>().purse;
        if (shopperInventory == null || shopperPurse == null) return;

        foreach (ShopItem shopItem in GetAllItems()) 
        {
            InventoryItem item = shopItem.GetInventoryItem();
            int quantity = shopItem.GetQuantityInTransaction();
            float price = shopItem.GetPrice();

            for (int i = 0; i < quantity; i++) 
            {
                if (shopperPurse.GetBalance() < price) break;
                bool success = shopperInventory.AddToFirstEmptySlot(item, 1);
                if (success) 
                {
                    AddToTransaction(item, -1);
                    stock[item]--;
                    shopperPurse.UpdateBalance(-price);
                }
            }
        }
        if (onChange != null) 
        {
            onChange();
        }
    }

    public float TransactionTotal() 
    {
        float total = 0;

        foreach (ShopItem item in GetAllItems()) 
        {
            total += item.GetPrice() * item.GetQuantityInTransaction();
        }
        return total;
    }

    public void AddToTransaction(InventoryItem item, int quantity) 
    {
        if (!transaction.ContainsKey(item)) 
        {
            transaction[item] = 0;
        }

        if (transaction[item] + quantity > stock[item])
        {
            transaction[item] = stock[item];
        }
        else 
        {
            transaction[item] += quantity;
        }

        if(transaction[item] <= 0) 
        {
            transaction.Remove(item);
        }

        if(onChange != null) 
        {
            onChange();
        }
    }

    override public void Interaction()
    {
        base.Interaction();
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if (player != null) 
        {
            player.shopper.SetActiveShop(this);

        }
    }

    public string GetShopName()
    {
        return shopName;
    }
}
