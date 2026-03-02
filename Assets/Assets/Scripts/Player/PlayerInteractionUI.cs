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
        Debug.Log("Call Togge " + UI.name);
        UI.SetActive(value);
    }
}
