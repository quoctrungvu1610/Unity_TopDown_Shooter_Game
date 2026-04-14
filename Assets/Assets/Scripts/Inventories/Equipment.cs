using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EquipmentSlot
{
    public EquipableItem item;
    public int number;

    public void SetItem(EquipableItem item, int number)
    {
        this.item = item;
        this.number = number;
    }
}


/// <summary>
/// Provides a store for the items equipped to a player. Items are stored by
/// their equip locations.
/// 
/// This component should be placed on the GameObject tagged "Player".
/// </summary>
public class Equipment : MonoBehaviour, ISaveable
{
    // STATE
    Dictionary<EquipLocation, EquipmentSlot> equippedItems = new Dictionary<EquipLocation, EquipmentSlot>();

    // PUBLIC

    /// <summary>
    /// Broadcasts when the items in the slots are added/removed.
    /// </summary>
    public event Action equipmentUpdated;

    private Player player;


    private void Start()
    {
        player = GetComponent<Player>();
    }

    /// <summary>
    /// Return the item in the given equip location.
    /// </summary>
    public EquipableItem GetItemInSlot(EquipLocation equipLocation)
    {
        if (!equippedItems.ContainsKey(equipLocation))
        {
            return null;
        }

        return equippedItems[equipLocation].item;
    }

    public int GetNumberInSlot(EquipLocation equipLocation)
    {
        if (!equippedItems.ContainsKey(equipLocation))
        {
            return 0;
        }
        return equippedItems[equipLocation].number;
    }

    //Get the bullet data in the given equip location. Return null if there is no item or the item is not a bullet item.
    public BulletItem GetBulletItemInSlot(EquipLocation equipLocation)
    {
        if (!equippedItems.ContainsKey(equipLocation))
        {
            return null;
        }
        var equipableItem = equippedItems[equipLocation].item as BulletItem;
        if (equipableItem == null) return null;
        return equipableItem;
    }

    /// <summary>
    /// Add an item to the given equip location. Do not attempt to equip to
    /// an incompatible slot.
    /// </summary>
    public void AddItem(EquipLocation slot, EquipableItem item, int number)
    {
        Debug.Log($"Adding item {item} to slot {slot} with number {number}");
        var equipmentSlot = new EquipmentSlot();
        equipmentSlot.item = item;
        equipmentSlot.number = number;
        equippedItems[slot] = equipmentSlot;

        //if (slot != EquipLocation.Weapon && slot != EquipLocation.BackupWeapon)
        //{
        //    player.health.UpdateCurrentEquipmentData(item as StatEquipableItem);
        //    player.health.OnAddEquipment();
        //}
        equipmentUpdated?.Invoke();
    }

    public void UpdateNumberInSlot(EquipLocation slot, int number)
    {
        if (!equippedItems.ContainsKey(slot)) return;
        var equipmentSlot = equippedItems[slot];
        equipmentSlot.number = number;
        equipmentSlot.item = equippedItems[slot].item;
        equippedItems[slot] = equipmentSlot;
    }

    /// <summary>
    /// Remove the item for the given slot.
    /// </summary>
    public void RemoveItem(EquipLocation slot)
    {
        //if (slot != EquipLocation.Weapon && slot != EquipLocation.BackupWeapon) 
        //{
        //    StatEquipableItem item = GetItemInSlot(slot) as StatEquipableItem;
        //    player.health.UpdateCurrentEquipmentData(item);
        //    player.health.OnRemoveEquipment();
        //}
        Debug.Log($"Removing item in slot {slot}");
        equippedItems.Remove(slot);
        equipmentUpdated?.Invoke();
    }

    /// <summary>
    /// Enumerate through all the slots that currently contain items.
    /// </summary>
    public IEnumerable<EquipLocation> GetAllPopulatedSlots()
    {
        return equippedItems.Keys;
    }

    // PRIVATE

    [System.Serializable]
    private struct EquipmentSlotRecord
    {
        public string itemID;
        public int number;
    }

    object ISaveable.CaptureState()
    {
        var equippedItemsForSerialization = new Dictionary<EquipLocation, EquipmentSlotRecord>();
        foreach (var pair in equippedItems)
        {
            var equipmentRecord = new EquipmentSlotRecord();
            equipmentRecord.itemID = pair.Value.item.GetItemID();
            equipmentRecord.number = pair.Value.number;
            equippedItemsForSerialization[pair.Key] = equipmentRecord;
        }
        return equippedItemsForSerialization;
    }

    void ISaveable.RestoreState(object state)
    {
        equippedItems = new Dictionary<EquipLocation, EquipmentSlot>();

        var equippedItemsForSerialization = (Dictionary<EquipLocation, EquipmentSlotRecord>)state;

        foreach (var pair in equippedItemsForSerialization)
        {
            var item = (EquipableItem)InventoryItem.GetFromID(pair.Value.itemID);
            var number = pair.Value.number;
            if (item != null && number > 0)
            {
                var equipmentSlot = new EquipmentSlot();
                equipmentSlot.SetItem(item, number);
                equippedItems[pair.Key] = equipmentSlot;
            }
        }
    }
}
