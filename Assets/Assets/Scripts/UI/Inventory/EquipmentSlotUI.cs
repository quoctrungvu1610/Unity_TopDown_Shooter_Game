using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An slot for the players equipment.
/// </summary>
public class EquipmentSlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>
{
    // CONFIG DATA

    [SerializeField] InventoryItemIcon icon = null;
    [SerializeField] EquipLocation equipLocation = EquipLocation.Weapon;

    // CACHE
    Equipment playerEquipment;

    // LIFECYCLE METHODS

    private void Awake()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        playerEquipment = player.GetComponent<Equipment>();
        playerEquipment.equipmentUpdated += RedrawUI;
    }

    private void Start()
    {
        RedrawUI();
    }

    // PUBLIC

    public int MaxAcceptable(InventoryItem item)
    {
        EquipableItem equipableItem = item as EquipableItem;
        if (equipableItem == null) return 0;
        if (equipableItem.GetAllowedEquipLocation() == EquipLocation.Weapon && equipLocation == EquipLocation.BackupWeapon) return 1;
        if (equipableItem.GetAllowedEquipLocation() != equipLocation) return 0;
        if (GetItem() != null) return 0;
        

        return int.MaxValue;
    }

    public void AddItems(InventoryItem item, int number)
    {
        playerEquipment.AddItem(equipLocation, (EquipableItem)item, number);
    }

    public InventoryItem GetItem()
    {
        return playerEquipment.GetItemInSlot(equipLocation);
    }

    public int GetNumber()
    {
        if (GetItem() != null)
        {
            return playerEquipment.GetNumberInSlot(equipLocation);
        }
        else
        {
            return 0;
        }
    }

    public void RemoveItems(int number)
    {
        playerEquipment.RemoveItem(equipLocation);
    }

    // PRIVATE

    void RedrawUI()
    {
        icon.SetItem(playerEquipment.GetItemInSlot(equipLocation), playerEquipment.GetNumberInSlot(equipLocation));
    }
}
