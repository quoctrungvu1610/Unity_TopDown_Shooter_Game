using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// To be placed on a UI slot to spawn and show the correct item tooltip.
/// </summary>
[RequireComponent(typeof(IItemHolder))]
public class ItemTooltipSpawner : TooltipSpawner
{
    public override bool CanCreateTooltip()
    {
        var item = GetComponent<IItemHolder>().GetItem();
        if (!item) return false;

        return true;
    }

    public override bool IsWeaponTooltip()
    {
        var item = GetComponent<IItemHolder>().GetItem();
        WeaponInventoryItem weaponInventoryItem = item as WeaponInventoryItem;
        if (weaponInventoryItem == null) return false;

        return true;
    }

    public override void UpdateTooltip(GameObject tooltip)
    {
        if (IsWeaponTooltip())
        {
            var weaponTooltip = tooltip.GetComponent<WeaponTooltip>();
            if (!weaponTooltip) return;
            var item = GetComponent<IItemHolder>().GetItem();
            weaponTooltip.Setup(item);

        }
        else 
        {
            var itemTooltip = tooltip.GetComponent<ItemTooltip>();
            if (!itemTooltip) return;
            var item = GetComponent<IItemHolder>().GetItem();
            itemTooltip.Setup(item);

        }
    }
}