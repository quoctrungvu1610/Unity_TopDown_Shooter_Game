using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LootBoxUI : MonoBehaviour
{
    [SerializeField] Transform lootSlotParent;
    [SerializeField] LootSlotUI lootSlotPrefab = null;
    [SerializeField] LootBox currentLootBox = null;
    [SerializeField] private Button closeButton;
    private Player player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        closeButton.onClick.AddListener(Close);
    }

    private void Close()
    {
        this.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (currentLootBox != null) 
        {
            currentLootBox = null;
        }

        if (player != null) 
        {
            if (player.looter == null) return;
            currentLootBox = player.looter.GetCurrentActiveLootBox();
        }

        if (currentLootBox != null)
        {
            currentLootBox.lootBoxUpdated += Redraw;
            Redraw();
        }

        if (currentLootBox == null)
        {
            return;
        }
        
    }

    private void OnDisable()
    {
        DestroyAllChild();
        if (currentLootBox != null)
        {
            currentLootBox.lootBoxUpdated -= Redraw;
        }
    }

    private void Redraw() 
    {
        DestroyAllChild();
        for (int i = 0; i < currentLootBox.GetSize(); i++) 
        {
            var itemUI = Instantiate(lootSlotPrefab, lootSlotParent);
            itemUI.Setup(currentLootBox, i);
        }

    }

    private void DestroyAllChild() 
    {
        foreach (Transform child in lootSlotParent)
        {
            Destroy(child.gameObject);
        }
    }


}
