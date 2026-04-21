using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[System.Serializable]
public struct InventorySlot
{
    public InventoryItem item;
    public int number;
}


/// <summary>
/// Provides storage for the player inventory. A configurable number of
/// slots are available.
///
/// This component should be placed on the GameObject tagged "Player".
/// </summary>
public class Inventory : MonoBehaviour, ISaveable, IPredicateEvaluator
{
    // CONFIG DATA
    [Tooltip("Allowed size")]
    [SerializeField] int inventorySize = 16;
    [SerializeField] Color[] rarityColors = null;
    // STATE
    [SerializeField] InventorySlot[] slots;

    // PUBLIC

    /// <summary>
    /// Broadcasts when the items in the slots are added/removed.
    /// </summary>
    public event Action inventoryUpdated;

    /// <summary>
    /// Convenience for getting the player's inventory.
    /// </summary>
    public static Inventory GetPlayerInventory()
    {
        var player = GameObject.FindWithTag("Player");
        return player.GetComponent<Inventory>();
    }

    public Color GetRarityColor(InventoryItem item) 
    {
        return rarityColors[(int)item.GetRarity()];
    }

    /// <summary>
    /// Could this item fit anywhere in the inventory?
    /// </summary>
    public bool HasSpaceFor(InventoryItem item)
    {
        return FindSlot(item) >= 0;
    }

    public bool HasSpaceFor(IEnumerable<InventoryItem> items) 
    {
        int freeSlots = FreeSlots();

        List<InventoryItem> statckedItem = new List<InventoryItem>();

        foreach (var item in items) 
        {
            if (item.IsStackable()) 
            {
                if (HasItem(item)) 
                {
                    continue;
                }

                if (statckedItem.Contains(item)) 
                {
                    continue;
                }

                statckedItem.Add(item);
            }
            if (freeSlots <= 0)
            {
                return false;
            }
            freeSlots--;
        }

        return true;
    }

    public int FreeSlots() 
    {
        int count = 0;
        foreach (var slot in slots) 
        {
            if (slot.number == 0) 
            {
                count++;
            }
        }
        return count;
    }

    /// <summary>
    /// How many slots are in the inventory?
    /// </summary>
    public int GetSize()
    {
        return slots.Length;
    }

    /// <summary>
    /// Attempt to add the items to the first available slot.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <returns>Whether or not the item could be added.</returns>
    public bool AddToFirstEmptySlot(InventoryItem item, int number)
    {
        int i = FindSlot(item);

        if (i < 0)
        {
            return false;
        }
        //TODO
        slots[i].item = item;
        slots[i].number += number;
        inventoryUpdated?.Invoke();
        return true;
    }

    /// <summary>
    /// Is there an instance of the item in the inventory?
    /// </summary>
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

    public int GetItemNumber(InventoryItem item) 
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (object.ReferenceEquals(slots[i].item, item))
            {
                return slots[i].number;
            }
        }
        return 0;
    }

    /// <summary>
    /// Return the item type in the given slot.
    /// </summary>
    public InventoryItem GetItemInSlot(int slot)
    {
        return slots[slot].item;
    }

    public int GetNumberInSlot(int slot)
    {
        return slots[slot].number;
    }

    /// <summary>
    /// Remove the item from the given slot.
    /// </summary>
    public void RemoveFromSlot(int slot, int number)
    {
        slots[slot].number -= number;
        if (slots[slot].number <= 0) 
        {
            slots[slot].number = 0;
            slots[slot].item = null;
        }
        inventoryUpdated?.Invoke();
    }

    public void RemoveItem(InventoryItem item, int number) 
    {
        for (int i = 0; i < slots.Length; i++) 
        {
            if (object.ReferenceEquals(slots[i].item, item)) 
            {
                slots[i].number -= number;
                if (slots[i].number <= 0) 
                {
                    slots[i].number = 0;
                    slots[i].item = null;
                }
            
            }
        }
        inventoryUpdated?.Invoke();
    }

    public bool CheckValue(InventoryItem item, int number) 
    {
        foreach (var slotItem in slots) 
        {
            if (object.ReferenceEquals(slotItem.item, item)) 
            {
                return slotItem.number >= number;
            }
        }
        return false;
    }

    /// <summary>
    /// Will add an item to the given slot if possible. If there is already
    /// a stack of this type, it will add to the existing stack. Otherwise,
    /// it will be added to the first empty slot.
    /// </summary>
    /// <param name="slot">The slot to attempt to add to.</param>
    /// <param name="item">The item type to add.</param>
    /// <returns>True if the item was added anywhere in the inventory.</returns>
    public bool AddItemToSlot(int slot, InventoryItem item, int number)
    {
        Debug.Log($"Add Item To Slot name : {item.name} - number : {number}");
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
        if (inventoryUpdated != null)
        {
            inventoryUpdated();
        }
        return true;
    }

    public WeaponEquipableItem[] GetWeaponInventoryItems() 
    {
        var weaponItems = new List<WeaponEquipableItem>();
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null && slots[i].item is WeaponEquipableItem) 
            {
                weaponItems.Add((WeaponEquipableItem)slots[i].item);
            }
        }
        return weaponItems.ToArray();
    }

    // PRIVATE

    private void Awake()
    {
        slots = new InventorySlot[inventorySize];
    }

    /// <summary>
    /// Find a slot that can accomodate the given item.
    /// </summary>
    /// <returns>-1 if no slot is found.</returns>
    private int FindSlot(InventoryItem item)
    {
        int i = FindStack(item);
        if (i < 0) 
        {
            i = FindEmptySlot();
        }
        return i;
    }

    /// <summary>
    /// Find an empty slot.
    /// </summary>
    /// <returns>-1 if all slots are full.</returns>
    private int FindEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Find an existing stack of this item type.
    /// </summary>
    /// <returns>-1 if all slots are full.</returns>
    private int FindStack(InventoryItem item) 
    {
        if (!item.IsStackable()) 
        {
            return -1;
        }

        for(int i = 0; i < slots.Length; i++)
        {
            if (object.ReferenceEquals(slots[i].item, item))
            {
                return i;
            }
        }
        return -1;
    }

    [System.Serializable]
    private struct InventorySlotRecord
    {
        public string itemID;
        public int number;
    }


    object ISaveable.CaptureState()
    {
        var slotStrings = new InventorySlotRecord[inventorySize];
        for (int i = 0; i < inventorySize; i++)
        {
            if (slots[i].item != null)
            {
                slotStrings[i].itemID = slots[i].item.GetItemID();
                slotStrings[i].number = slots[i].number;

            }
        }
        return slotStrings;
    }

    void ISaveable.RestoreState(object state)
    {
        var slotStrings = (InventorySlotRecord[])state;
        for (int i = 0; i < inventorySize; i++)
        {
            slots[i].item = InventoryItem.GetFromID(slotStrings[i].itemID);
            slots[i].number = slotStrings[i].number;
        }
        if (inventoryUpdated != null)
        {
            inventoryUpdated();
        }
    }

    public bool? Evaluate(string predicate, string[] parameters)
    {
        switch (predicate) 
        {
            case "HasInventoryItem":
                return HasItem(InventoryItem.GetFromID(parameters[0]));
        }
        return null;
    }
}