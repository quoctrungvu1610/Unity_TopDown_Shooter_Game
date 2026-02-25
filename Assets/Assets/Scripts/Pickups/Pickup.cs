using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// To be placed at the root of a Pickup prefab. Contains the data about the
/// pickup such as the type of item and the number.
/// </summary>
public class Pickup : Interactable
{
    // STATE
    InventoryItem item;

    // CACHED REFERENCE
    Inventory inventory;

    // LIFECYCLE METHODS

    protected override void Awake()
    {
        base.Awake();

        var player = GameObject.FindGameObjectWithTag("Player");
        inventory = player.GetComponent<Inventory>();
    }

    // PUBLIC

    /// <summary>
    /// Set the vital data after creating the prefab.
    /// </summary>
    /// <param name="item">The type of item this prefab represents.</param>
    public void Setup(InventoryItem item)
    {
        this.item = item;
    }

    public InventoryItem GetItem()
    {
        return item;
    }

    public void PickupItem()
    {
        bool foundSlot = inventory.AddToFirstEmptySlot(item);
        if (foundSlot)
        {
            Destroy(gameObject);
        }
    }

    public bool CanBePickedUp()
    {
        return inventory.HasSpaceFor(item);
    }

    public override void Interaction()
    {
        base.Interaction();
        PickupItem();
    }
}
