using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilterButtonUI : MonoBehaviour
{
    private Button button;
    private Shop currentShop;

    [SerializeField] private ItemCategory category = ItemCategory.None;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(SelectFilter);
    }

    public void SetShop(Shop currentShop) 
    {
        this.currentShop = currentShop;
    }

    public void RefreshUI() 
    {
        button.interactable = currentShop.GetFilter() != category;
    }

    private void SelectFilter() 
    {
        currentShop.SelectFilter(category); 
    }
}
