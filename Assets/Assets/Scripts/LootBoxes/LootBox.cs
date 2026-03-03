using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Inventory;

public class LootBox : Interactable 
{
    [SerializeField] private LootBoxSlot[] slots;

    private LootBoxSlot[] interactSlots;


    private Player player;

    protected override void Awake()
    {
        base.Awake();
        interactSlots = new LootBoxSlot[10];
        player = FindObjectOfType<Player>();
    }

    private void Start()
    {
        interactSlots = slots;
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
        for (int i = 0; i < interactSlots.Length; i++)
        {
            if (object.ReferenceEquals(interactSlots[i].item, item))
            {
                return true;
            }
        }
        return false;
    }

    public int GetSize() 
    {
        return interactSlots.Length;
    }

    public InventoryItem GetItemInSlot(int slot)
    {
        return interactSlots[slot].item;
    }

    public int GetNumberInSlot(int slot)
    {
        return interactSlots[slot].number;
    }

    public bool AddToFirstEmptySlot(InventoryItem item, int number)
    {
        int i = FindSlot(item);

        if (i < 0)
        {
            return false;
        }
        interactSlots[i].item = item;
        interactSlots[i].number += number;

        if (lootBoxUpdated != null)
        {
            lootBoxUpdated();
        }
        return true;
    }

    public bool AddItemToSlot(int slot, InventoryItem item, int number)
    {
        if (interactSlots[slot].item != null)
        {
            return AddToFirstEmptySlot(item, number); ;
        }
        var i = FindStack(item);
        if (i >= 0)
        {
            slot = i;
        }

        interactSlots[slot].item = item;
        interactSlots[slot].number += number;
        if (lootBoxUpdated != null)
        {
            lootBoxUpdated();
        }
        return true;
    }

    public void RemoveFromSlot(int slot, int number)
    {
        interactSlots[slot].number -= number;
        if (interactSlots[slot].number <= 0)
        {
            interactSlots[slot].number = 0;
            interactSlots[slot].item = null;
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
            Debug.Log(FindEmptySlot());
        }
        return i;
    }


    private int FindStack(InventoryItem item)
    {
        if (!item.IsStackable())
        {
            return -1;
        }

        for (int i = 0; i < interactSlots.Length; i++)
        {
            if (object.ReferenceEquals(interactSlots[i].item, item))
            {
                return i;
            }
        }
        return -1;
    }

    private int FindEmptySlot()
    {
        for (int i = 0; i < interactSlots.Length; i++)
        {
            if (interactSlots[i].item == null)
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
