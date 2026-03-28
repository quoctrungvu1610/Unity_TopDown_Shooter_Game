using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHideUI : MonoBehaviour
{
    [SerializeField] KeyCode inventoryToggleKey = KeyCode.Escape;
    [SerializeField] KeyCode playerEquipmentToggleKey = KeyCode.F2;

    void Update()
    {
        if (Input.GetKeyDown(inventoryToggleKey))
        {
            Toggle(PanelName.Inventory);
        }
        if (Input.GetKeyDown(playerEquipmentToggleKey))
        {
            Toggle(PanelName.PlayerEquipment);
        }
    }

    public void Toggle(PanelName panelName)
    {
        Debug.Log(panelName);
        UIManager.Instance.TogglePanel(panelName, false);
    }
}
