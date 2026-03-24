using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarehouseSlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>
{
    [SerializeField] InventoryItemIcon icon = null;

    private int index;
    public InventoryItem item;
    private Warehouse warehouse;

    public void Setup(Warehouse warehouse, int index)
    {
        this.warehouse = warehouse;
        this.index = index;
        this.item = warehouse.GetItemInSlot(index);
        icon.SetItem(warehouse.GetItemInSlot(index), warehouse.GetNumberInSlot(index));
    }

    public void Setup(Warehouse warehouse, InventorySlot slot) 
    {
        this.warehouse = warehouse;
        this.index = -1;
        item = slot.item;
        icon.SetItem(slot.item, slot.number);
    }

    public void AddItems(InventoryItem item, int number)
    {
        warehouse.AddItemToSlot(index, item, number);
    }

    public InventoryItem GetItem()
    {
        return warehouse.GetItemInSlot(index);
    }

    public int GetNumber()
    {
        return warehouse.GetNumberInSlot(index);
    }

    public int MaxAcceptable(InventoryItem item)
    {
        if (warehouse.HasSpaceFor(item))
        {
            return int.MaxValue;
        }
        return 0;
    }

    public void RemoveItems(int number)
    {
        warehouse.RemoveFromSlot(index, number);
    }

}
