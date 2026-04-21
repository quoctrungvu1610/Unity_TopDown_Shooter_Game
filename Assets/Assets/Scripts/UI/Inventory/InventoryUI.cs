using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// To be placed on the root of the inventory UI. Handles spawning all the
/// inventory slot prefabs.
/// </summary>
public class InventoryUI : MonoBehaviour
{
    // CONFIG DATA
    [SerializeField] InventorySlotUI InventoryItemPrefab = null;
    [SerializeField] private Button quitButton;
    [SerializeField] private Transform uISLotsHolder;

    // CACHE
    Inventory playerInventory;

    // LIFECYCLE METHODS

    private void Awake()
    {
        playerInventory = Inventory.GetPlayerInventory();
        playerInventory.inventoryUpdated += Redraw;
        quitButton.onClick.AddListener(Close);
    }

    private void Start()
    {
        Redraw();
    }

    // PRIVATE

    private void Redraw()
    {
        foreach (Transform child in uISLotsHolder)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < playerInventory.GetSize(); i++)
        {
            var itemUI = Instantiate(InventoryItemPrefab, uISLotsHolder);
            itemUI.Setup(playerInventory, i);
        }
    }

    private void Close() 
    {
        this.gameObject.SetActive(false);
    }
}
