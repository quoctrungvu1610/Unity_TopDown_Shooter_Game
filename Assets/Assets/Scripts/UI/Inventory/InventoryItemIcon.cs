using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

/// <summary>
/// To be put on the icon representing an inventory item. Allows the slot to
/// update the icon and number.
/// </summary>
[RequireComponent(typeof(Image))]
public class InventoryItemIcon : MonoBehaviour
{
    //CONFIG DATA
    [SerializeField] private GameObject textContainer = null;
    [SerializeField] private GameObject itemNameTextContainer = null;
    [SerializeField] private TextMeshProUGUI itemNumber = null;
    [SerializeField] private TextMeshProUGUI itemName = null;
    [SerializeField] private GameObject backImage = null;
    // PUBLIC
    //public void SetItem(InventoryItem item, int number)
    //{
    //    SetItem(item, number);
    //}

    public void SetItem(InventoryItem item, int number)
    {
        var iconImage = GetComponent<Image>();
        if (item == null)
        {
            if(backImage != null)
                backImage.SetActive(true);
            iconImage.enabled = false;
            itemNameTextContainer.SetActive(false);
        }
        else
        {
            if(backImage != null)
                backImage.SetActive(false);
            iconImage.enabled = true;
            iconImage.sprite = item.GetIcon();
            itemNameTextContainer.SetActive(true);
            itemName.text = item.GetDisplayName();
        }

        if (itemNumber) 
        {
            if(number <= 1)
            {
                textContainer.SetActive(false);
            }
            else
            {
                textContainer.SetActive(true);
                itemNumber.text = number.ToString();
            }
        }

    }
}