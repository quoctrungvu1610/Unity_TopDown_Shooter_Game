using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private Button quitButton;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button switchButton;

    [SerializeField] private TextMeshProUGUI[] shopNames;
    [SerializeField] private TextMeshProUGUI totalField;

    [SerializeField] private Transform listRoot;
    [SerializeField] private RowUI rowPrefab;

    private Shopper shopper = null;
    private Shop currentShop = null;

    Color originalTotalTextColor;


    private void Awake()
    {
        quitButton.onClick.AddListener(Close);
        confirmButton.onClick.AddListener(ConfirmTransaction);
        switchButton.onClick.AddListener(SwitchMode);
    }

    void Start()
    {
        originalTotalTextColor = totalField.color;
        shopper = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().shopper;
        if (shopper == null) return;
        shopper.activeShopChange += ShopChanged;
        this.gameObject.SetActive(false);
    }

    private void ShopChanged() 
    {
        if(currentShop != null) 
        {
            currentShop.onChange -= RefreshUI;
        }

        currentShop = shopper.GetActiveShop();

        gameObject.SetActive(currentShop != null);

        foreach (FilterButtonUI button in GetComponentsInChildren<FilterButtonUI>()) 
        {
            button.SetShop(currentShop);
        }

        if (currentShop == null) return;
        foreach (TextMeshProUGUI name in shopNames) 
        {
            name.text = currentShop.GetShopName();
        }

        currentShop.onChange += RefreshUI;

        RefreshUI();

    }

    private void RefreshUI()
    {
        foreach (Transform child in listRoot) 
        {
            Destroy(child.gameObject);
        }

        foreach (ShopItem item in currentShop.GetFilteredItems()) 
        {
            RowUI row = Instantiate<RowUI>(rowPrefab, listRoot);
            row.Setup(currentShop, item);
        }

        totalField.text = "Total : " + "$" + currentShop.TransactionTotal().ToString("N2");
        totalField.color = currentShop.HasSufficientFunds() ? originalTotalTextColor : Color.red;
        confirmButton.interactable = currentShop.CanTransact();

        TextMeshProUGUI switchText = switchButton.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI confirmText = confirmButton.GetComponentInChildren<TextMeshProUGUI>();

        if (currentShop.IsBuyingMode())
        {
            switchText.text = "SELL MODE";
            confirmText.text = "PURCHASE";
        }
        else 
        {
            switchText.text = "BUY MODE";
            confirmText.text = "SELL";
        }

        foreach (FilterButtonUI button in GetComponentsInChildren<FilterButtonUI>()) 
        {
            button.RefreshUI();
        }
    }

    private void Close() 
    {
        shopper.SetActiveShop(null);
    }

    public void ConfirmTransaction() 
    {
        currentShop.ConfirmTransaction();
    }

    public void SwitchMode() 
    {
        currentShop.SelectMode(!currentShop.IsBuyingMode());
    }
}
