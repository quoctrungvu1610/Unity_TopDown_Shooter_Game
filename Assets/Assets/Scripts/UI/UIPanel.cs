using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PanelName 
{
    Inventory,
    HUD,
    Loot,
    Dialogue,
    Quest,
    Stat,
    Shop,
    Warehouse,
    BuildingPanel,
    PlayerEquipment

}

public class UIPanel : MonoBehaviour
{
    public PanelName panelName;
    public List<PanelName> panelsActiveWith = new List<PanelName>();

    public void Show() 
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
