using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionUI : MonoBehaviour
{
    public GameObject inventoryUI;
    public GameObject playerUI;
    public GameObject lootBoxUI;
    public GameObject equipmentUI;

    public void ToggleUI(GameObject UI, bool value) 
    {
        if(value == true && UI.gameObject.activeSelf == false)
        {
            UI.SetActive(true);
            return;
        }
        else if(value == false && UI.gameObject.activeSelf == true)
        {
            UI.SetActive(false);
            return;
        }

    }
}
