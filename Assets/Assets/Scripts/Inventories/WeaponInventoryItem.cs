using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Inventory/Weapon"))]
public class WeaponInventoryItem : InventoryItem
{
    [SerializeField] private Weapon_Data weapon_Data;

    public Weapon_Data GetWeaponData()
    {
        return weapon_Data;
    }
}
