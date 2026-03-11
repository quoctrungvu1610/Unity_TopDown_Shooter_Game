using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Inventory/Equipable Weapon"))]
public class WeaponEquipableItem : EquipableItem
{
    [SerializeField] private Weapon_Data weapon_Data;

    public Weapon_Data GetWeaponData()
    {
        return weapon_Data;
    }
}
