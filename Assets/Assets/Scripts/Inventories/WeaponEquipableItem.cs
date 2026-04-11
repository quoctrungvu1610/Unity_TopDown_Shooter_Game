using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Inventory/Equipable Weapon"))]
public class WeaponEquipableItem : EquipableItem
{
    [SerializeField] private WeaponData weaponData;

    public WeaponData GetWeaponData()
    {
        return weaponData;
    }
}
