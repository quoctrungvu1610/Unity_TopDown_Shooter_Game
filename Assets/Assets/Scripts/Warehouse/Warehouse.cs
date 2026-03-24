using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Warehouse : Interactable, ISaveable
{

    [SerializeField] private int warehouseSize = 100;
    private InventorySlot[] slots;

    public event Action warehouseUpdated;
    public event Action filteredUpdated;

    public ItemCategory currentCategory = ItemCategory.None;

    public static Warehouse GetPlayerWarehouse()
    {
        var warehouse = GameObject.FindWithTag("Warehouse");
        return warehouse.GetComponent<Warehouse>();
    }

    protected override void Awake()
    {
        base.Awake();
        slots = new InventorySlot[warehouseSize];
    }

    public bool HasSpaceFor(InventoryItem item) 
    {
        return FindSlot(item) >= 0;
    }

    private int FindSlot(InventoryItem item)
    {
        int i = FindStack(item);
        if (i < 0) 
        {
            i = FindEmptySlot();
        }
        return i;
    }

    private int FindEmptySlot()
    {
        for (int i = 0; i < GetSize(); i++) 
        {
            if (slots[i].item == null) 
            {
                return i;
            }
        }
        return -1;
    }

    public int GetSize() 
    {
        return slots.Length;
    }

    public bool AddToFirstEmptySlot(InventoryItem item, int number) 
    {
        int i = FindSlot(item);

        if (i < 0)
        {
            return false;
        }

        slots[i].item = item;
        slots[i].number += number;

        Debug.Log("Added " + item.name + " to inventory slot " + i + ":" + slots[i].number);

        if (warehouseUpdated != null)
        {
            warehouseUpdated();
        }
        return true;
    }

    public bool HasItem(InventoryItem item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (object.ReferenceEquals(slots[i].item, item))
            {
                return true;
            }
        }
        return false;
    }

    public InventoryItem GetItemInSlot(int slot)
    {
        return slots[slot].item;
    }

    public int GetNumberInSlot(int slot)
    {
        return slots[slot].number;
    }

    public void RemoveFromSlot(int slot, int number)
    {
        slots[slot].number -= number;
        if (slots[slot].number <= 0)
        {
            slots[slot].number = 0;
            slots[slot].item = null;
        }
        if (warehouseUpdated != null)
        {
            warehouseUpdated();
        }
    }

    public bool AddItemToSlot(int slot, InventoryItem item, int number)
    {
        if (slots[slot].item != null)
        {
            return AddToFirstEmptySlot(item, number); ;
        }
        var i = FindStack(item);
        if (i >= 0)
        {
            slot = i;
        }

        slots[slot].item = item;
        slots[slot].number += number;
        if (warehouseUpdated != null)
        {
            warehouseUpdated();
        }
        return true;
    }

    private int FindStack(InventoryItem item)
    {
        if (!item.IsStackable())
        {
            return -1;
        }

        for (int i = 0; i < GetSize(); i++)
        {
            if (object.ReferenceEquals(slots[i].item, item))
            {
                return i;
            }
        }
        return -1;
    }

    [System.Serializable]
    private struct WarehouseSlotRecord
    {
        public string itemID;
        public int number;
    }


    public object CaptureState()
    {
        var slotStrings = new WarehouseSlotRecord[warehouseSize];
        for (int i = 0; i < warehouseSize; i++)
        {
            if (slots[i].item != null)
            {
                slotStrings[i].itemID = slots[i].item.GetItemID();
                slotStrings[i].number = slots[i].number;

            }
        }
        return slotStrings;
    }

    public void RestoreState(object state)
    {
        var slotStrings = (WarehouseSlotRecord[])state;
        for (int i = 0; i < warehouseSize; i++)
        {
            slots[i].item = InventoryItem.GetFromID(slotStrings[i].itemID);
            slots[i].number = slotStrings[i].number;
        }
        if (warehouseUpdated != null)
        {
            warehouseUpdated();
        }
    }

    public int GetSizeByCategoty(ItemCategory category) 
    {
        int size = 0;
        foreach (var item in slots)
        {
            if (item.item.GetCategoty() == category)
            {
                size++;
            }
        }
        return size;
    }

    public IEnumerable<InventorySlot> GetFilteredItem(ItemCategory category) 
    {
        foreach (var item in slots) 
        {
            if (item.item != null) 
            {
                if (item.item.GetCategoty() == category) 
                {
                    yield return item;
                }
            }
        }
    }

    public void SetCurrentCategory(ItemCategory category) 
    {
        Debug.Log(category);
        if (category == ItemCategory.None) 
        {
            warehouseUpdated();
            return;
        }
        currentCategory = category;
        filteredUpdated();
    }

    public override void Interaction()
    {
        base.Interaction();
        UIManager.instance.ToggleUI(true, UIManager.instance.warehouseUI.gameObject);
    }
}
