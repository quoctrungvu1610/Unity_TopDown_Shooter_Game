using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>
{
    [SerializeField] InventoryItemIcon icon = null;
    [SerializeField] EquipLocation equipLocation = EquipLocation.MainWeaponAmmo;

    Equipment playerEquipment;
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

    public int MaxAcceptable(InventoryItem item)
    {
        var bulletItem = item as BulletItem;
        if (bulletItem == null) return 0;

        var weaponData = GetWeaponDataInSlot(equipLocation);
        if (weaponData == null) return 0;

        var allowedLocation = bulletItem.GetAllowedEquipLocation();
        if (allowedLocation != EquipLocation.MainWeaponAmmo &&
            allowedLocation != EquipLocation.BackupWeaponAmmo)
            return 0;

        var bulletData = bulletItem.GetBulletData();
        if (bulletData == null) return 0;

        return weaponData.GetCompatibleBullets().Contains(bulletData)
            ? int.MaxValue
            : 0;
    }

    private WeaponData GetWeaponDataInSlot(EquipLocation equipLocation) 
    {
        if(equipLocation == EquipLocation.MainWeaponAmmo) 
        {
            var mainWeapon = playerEquipment.GetItemInSlot(EquipLocation.Weapon) as WeaponEquipableItem;
            if(mainWeapon != null) return mainWeapon.GetWeaponData();
        }
        else if(equipLocation == EquipLocation.BackupWeaponAmmo) 
        {
            var backupWeapon = playerEquipment.GetItemInSlot(EquipLocation.BackupWeapon) as WeaponEquipableItem;
            if(backupWeapon != null) return backupWeapon.GetWeaponData();
        }
        return null;
    }

    public void RemoveItems(int number)
    {
        playerEquipment.RemoveItem(equipLocation);
    }

    void RedrawUI()
    {
        icon.SetItem(playerEquipment.GetItemInSlot(equipLocation), playerEquipment.GetNumberInSlot(equipLocation));
    }
}
