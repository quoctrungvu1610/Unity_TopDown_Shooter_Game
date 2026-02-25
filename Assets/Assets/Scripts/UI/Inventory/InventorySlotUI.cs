using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlotUI : MonoBehaviour, IDragContainer<Sprite>
{
    // CONFIG DATA
    [SerializeField] InventoryItemIcon icon = null;

    // PUBLIC

    public int MaxAcceptable(Sprite item)
    {
        if (GetItem() == null)
        {
            return int.MaxValue;
        }
        return 0;
    }

    public void AddItems(Sprite item, int number)
    {
        icon.SetItem(item);
    }

    public Sprite GetItem()
    {
        return icon.GetItem();
    }

    public int GetNumber()
    {
        return 1;
    }

    public void RemoveItems(int number)
    {
        icon.SetItem(null);
    }
}
