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
        public float buyingDiscountPercentage;
    }

    [SerializeField] private string shopName;
    [SerializeField] private float sellingPercentage = 80f;

    [SerializeField] private StockItemConfig[] stockConfig;

    private Dictionary<InventoryItem, int> transaction = new Dictionary<InventoryItem, int>();
    private Dictionary<InventoryItem, int> stock = new Dictionary<InventoryItem, int>();

    private Shopper currentShopper = null;

    private bool isBuyingMode = true;

    private ItemCategory filter = ItemCategory.None;

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
        foreach (ShopItem shopItem in GetAllItems()) 
        {
            InventoryItem item = shopItem.GetInventoryItem();
            if (filter == ItemCategory.None || item.GetCategoty() == filter) 
            {
                yield return shopItem;
            }  
        }
    }

    public IEnumerable<ShopItem> GetAllItems()
    {
        foreach (StockItemConfig config in stockConfig)
        {
            float price = GetPrice(config);
            int quantityInTransaction = 0;
            transaction.TryGetValue(config.item, out quantityInTransaction);
            int availability = GetAvailability(config.item);
            yield return new ShopItem(config.item, availability, price, quantityInTransaction);
        }
    }

    private float GetPrice(StockItemConfig config)
    {
        if (isBuyingMode) 
        {
            return config.item.GetPrice() * (1 - config.buyingDiscountPercentage / 100f);
        }

        return config.item.GetPrice() * (sellingPercentage / 100f); 
    }

    public void SelectFilter(ItemCategory category) 
    {
        filter = category;
        onChange?.Invoke();
    }

    public ItemCategory GetFilter() 
    {
        return filter; 
    }

    public void SelectMode(bool isBuying) 
    {
        isBuyingMode = isBuying;
        onChange?.Invoke();
    }

    public bool IsBuyingMode() 
    {
        return isBuyingMode;
    }

    public bool CanTransact() 
    {
        if (IsTransactionEmpty()) return false;
        if (!HasSufficientFunds()) return false;
        if (!HasInventorySpace()) return false;

        return true; 
    }

    public bool HasSufficientFunds()
    {
        Purse purse = null;
        if (!isBuyingMode) return true;
        if(currentShopper != null)
            purse = currentShopper.GetComponent<Player>().purse;
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
        if (!isBuyingMode) return true;
 
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
                if (isBuyingMode)
                {
                    BuyItem(shopperInventory, shopperPurse, item, price);
                }
                else 
                {
                    SellItem(shopperInventory, shopperPurse, item, price);
                }
            }
        }
        onChange?.Invoke();
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

        int availability = GetAvailability(item);
        if (transaction[item] + quantity > availability)
        {
            transaction[item] = availability;
        }
        else 
        {
            transaction[item] += quantity;
        }

        if(transaction[item] <= 0) 
        {
            transaction.Remove(item);
        }

        onChange?.Invoke();
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

    private int GetAvailability(InventoryItem item)
    {
        if (isBuyingMode)
        {
            return stock[item];
        }

        return CountItemInInventory(item);
    }

    private int CountItemInInventory(InventoryItem item)
    {
        Inventory inventory = null;
        if (currentShopper != null) 
        {
            inventory = currentShopper.GetComponent<Player>().inventory;
        }
        
        if (inventory == null) 
        {
            return 0;
        }

        int total = 0;
        for (int i = 0; i < inventory.GetSize(); i++) 
        {
            if (inventory.GetItemInSlot(i) == item) 
            {
                total += inventory.GetNumberInSlot(i);
            }
        }
        return total;
    }

    private void SellItem(Inventory shopperInventory, Purse shopperPurse, InventoryItem item, float price)
    {
        int slot = FindFirstItemSlot(shopperInventory, item);
        if (slot == -1) return;

        AddToTransaction(item, -1);
        shopperInventory.RemoveFromSlot(slot, 1);
        stock[item]++;
        shopperPurse.UpdateBalance(price);
    }

    private int FindFirstItemSlot(Inventory shopperInventory, InventoryItem item)
    {
        for (int i = 0; i < shopperInventory.GetSize(); i++) 
        {
            if (shopperInventory.GetItemInSlot(i) == item) 
            {
                return i;
            }
        }
        return -1;
    }

    private void BuyItem(Inventory shopperInventory, Purse shopperPurse, InventoryItem item, float price)
    {
        if (shopperPurse.GetBalance() < price) return;
        bool success = shopperInventory.AddToFirstEmptySlot(item, 1);
        if (success)
        {
            AddToTransaction(item, -1);
            stock[item]--;
            shopperPurse.UpdateBalance(-price);
        }
    }
}
