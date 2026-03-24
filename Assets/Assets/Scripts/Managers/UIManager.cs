using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public WarehouseUI warehouseUI;
    //public InventoryUI inventoryUI;
    //public LootBoxUI lootBoxUI;
    //public DialogueUI dialogueUI;
    


    public static UIManager instance;


    private void Awake()
    {
        instance = this;
    }

    public void ToggleUI(bool value, GameObject UI) 
    {
        UI.gameObject.SetActive(value);
    }
}
