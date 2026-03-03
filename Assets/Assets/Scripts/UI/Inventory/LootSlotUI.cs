using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootSlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>
{
    [SerializeField] InventoryItemIcon icon = null;

    int index;
    InventoryItem item;
    LootBox lootBox;

    // PUBLIC

    public void Setup(LootBox inventory, int index)
    {
        this.lootBox = inventory;
        this.index = index;
        icon.SetItem(inventory.GetItemInSlot(index), inventory.GetNumberInSlot(index));
    }

    public int MaxAcceptable(InventoryItem item)
    {
        if (lootBox.HasSpaceFor(item))
        {
            Debug.Log("Loot box has space for " + item.name);
            return int.MaxValue;
        }
        Debug.Log("Loot box does not have space for " + item.name);
        return 0;
    }

    public void AddItems(InventoryItem item, int number)
    {
        lootBox.AddItemToSlot(index, item, number);
    }

    public InventoryItem GetItem()
    {
        return lootBox.GetItemInSlot(index);
    }

    public int GetNumber()
    {
        return lootBox.GetNumberInSlot(index);
    }

    public void RemoveItems(int number)
    {
        lootBox.RemoveFromSlot(index, number);
    }
}
