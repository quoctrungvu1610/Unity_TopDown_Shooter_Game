using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LootBox : Interactable 
{
    [SerializeField] private LootBoxSlot[] slots;
    private Player player;

    protected override void Awake()
    {
        base.Awake();
        player = FindObjectOfType<Player>();
    }
    [System.Serializable]
    public struct LootBoxSlot
    {
        public InventoryItem item;
        public int number;
    }

    public event Action lootBoxUpdated;

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

    public int GetSize() 
    {
        return slots.Length;
    }

    public InventoryItem GetItemInSlot(int slot)
    {
        return slots[slot].item;
    }

    public int GetNumberInSlot(int slot)
    {
        return slots[slot].number;
    }

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
        Debug.Log("Added " + item.name + " to inventory slot " + i + ":" + slots[i].number);
        if (lootBoxUpdated != null)
        {
            lootBoxUpdated();
        }
        return true;
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
        if (lootBoxUpdated != null)
        {
            lootBoxUpdated();
        }
        return true;
    }

    public void RemoveFromSlot(int slot, int number)
    {
        slots[slot].number -= number;
        if (slots[slot].number <= 0)
        {
            slots[slot].number = 0;
            slots[slot].item = null;
        }
        if (lootBoxUpdated != null)
        {
            lootBoxUpdated();
        }
    }

    public bool HasSpaceFor(InventoryItem item)
    {
        return FindSlot(item) >= 0;
    }

    public LootBox GetLootBox() 
    {
        return GetComponent<LootBox>();
    }

    private int FindSlot(InventoryItem item)
    {
        int i = FindStack(item);
        if (i < 0)
        {
            i = FindEmptySlot();
        }
        return -1;
    }


    private int FindStack(InventoryItem item)
    {
        if (!item.IsStackable())
        {
            return -1;
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (object.ReferenceEquals(slots[i].item, item))
            {
                return i;
            }
        }
        return -1;
    }

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

    public override void Interaction()
    {
        base.Interaction();
        player.GetComponent<PlayerInteraction>().SetCurrentLootBox(this);
        PlayerInteractionUI playerInteractionUI = player.GetComponent<PlayerInteractionUI>();
        playerInteractionUI.ToggleUI(playerInteractionUI.lootBoxUI, true);

    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        PlayerInteractionUI playerInteractionUI = player.GetComponent<PlayerInteractionUI>();
        playerInteractionUI.ToggleUI(playerInteractionUI.lootBoxUI, false);

    }

}
