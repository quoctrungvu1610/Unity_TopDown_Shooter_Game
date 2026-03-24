using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarehouseFilteredButton : MonoBehaviour
{
    [SerializeField] private ItemCategory category;
    private Button button;
    private Warehouse warehouse;

    private void Awake()
    {
        warehouse = Warehouse.GetPlayerWarehouse();
        button = GetComponent<Button>();

        button.onClick.AddListener(SetWarehouseFiltered);
    }

    private void SetWarehouseFiltered() 
    {
        if (warehouse != null) 
        {
            warehouse.SetCurrentCategory(category);
        } 
    }
}
