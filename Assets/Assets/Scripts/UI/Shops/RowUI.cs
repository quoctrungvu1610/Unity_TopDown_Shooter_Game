using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RowUI : MonoBehaviour, IItemHolder
{
    [SerializeField] private TextMeshProUGUI nameField;
    [SerializeField] private Image iconField;
    [SerializeField] private TextMeshProUGUI availabilityField;
    [SerializeField] private TextMeshProUGUI priceField;
    [SerializeField] private Button addButton;
    [SerializeField] private Button removeButton;
    [SerializeField] private TextMeshProUGUI quantityField;

    private Shop currentShop = null;
    private ShopItem item = null;

    private void Awake()
    {
        addButton.onClick.AddListener(Add);
        removeButton.onClick.AddListener(Remove);
    }

    public void Setup(Shop currentShop, ShopItem item)
    {
        this.currentShop = currentShop;
        this.item = item;

        iconField.sprite = item.GetIcon();
        nameField.text = item.GetName();
        availabilityField.text = item.GetAvailability().ToString();
        priceField.text = "$" + item.GetPrice().ToString("N2");
        quantityField.text = item.GetQuantityInTransaction().ToString();
    }

    public void Add() 
    {
        currentShop.AddToTransaction(item.GetInventoryItem(), 1);
    }

    public void Remove() 
    {

        currentShop.AddToTransaction(item.GetInventoryItem(), -1);
    }

    public InventoryItem GetItem()
    {
        return item.GetInventoryItem();
    }
}
