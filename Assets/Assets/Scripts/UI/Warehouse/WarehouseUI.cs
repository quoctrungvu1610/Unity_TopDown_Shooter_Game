using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarehouseUI : MonoBehaviour
{
    [SerializeField] private WarehouseSlotUI wareHouseItemPrefab = null;
    [SerializeField] private Transform slotsHolder;
    [SerializeField] private Button quitButton;

    // CACHE
    Warehouse playerWarehouse;

    // LIFECYCLE METHODS

    private void Awake()
    {
        playerWarehouse = Warehouse.GetPlayerWarehouse();
        playerWarehouse.warehouseUpdated += Redraw;
        playerWarehouse.filteredUpdated += RedrawFilteredItem;

        quitButton.onClick.AddListener(Close);
    }

    private void Start()
    {
        Redraw();
        this.gameObject.SetActive(false);
    }

    private void Close() 
    {
        this.gameObject.SetActive(false);
    }

    private void Redraw()
    {
        foreach (Transform child in slotsHolder)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < playerWarehouse.GetSize(); i++)
        {
            var itemUI = Instantiate(wareHouseItemPrefab, slotsHolder);
            itemUI.Setup(playerWarehouse, i);
        }
    }

    private void RedrawFilteredItem() 
    {
        foreach (Transform child in slotsHolder)
        {
            InventoryItem item = child.gameObject.GetComponent<WarehouseSlotUI>().item;
            if (item != null)
            {
                if (item.GetCategoty() == playerWarehouse.currentCategory)
                {
                    child.gameObject.SetActive(true);
                }
                else
                {
                    child.gameObject.SetActive(false);
                }
            }
            else 
            {
                child.gameObject.SetActive(false);
            }
  
        }
        
    }
}
