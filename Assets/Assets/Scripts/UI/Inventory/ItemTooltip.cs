using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Root of the tooltip prefab to expose properties to other classes.
/// </summary>
public class ItemTooltip : MonoBehaviour
{
    // CONFIG DATA
    [SerializeField] TextMeshProUGUI titleText = null;
    [SerializeField] TextMeshProUGUI bodyText = null;
    [SerializeField] Image icon;

    // PUBLIC

    public void Setup(InventoryItem item)
    {
        titleText.text = item.GetDisplayName();
        bodyText.text = item.GetDescription();
        icon.sprite = item.GetIcon();
    }
}
