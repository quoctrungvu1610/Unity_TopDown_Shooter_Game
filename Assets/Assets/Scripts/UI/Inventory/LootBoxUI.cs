using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootBoxUI : MonoBehaviour
{
    [SerializeField] LootSlotUI lootSlotPrefab = null;

    [SerializeField] LootBox currentLootBox;


    private void OnEnable()
    {
        
        currentLootBox = FindObjectOfType<PlayerInteraction>().currentActiveLootBox;
        currentLootBox.lootBoxUpdated += Redraw;
        Redraw();
    }

    private void OnDisable()
    {
        DestroyAllChild();
        if(currentLootBox != null)
            currentLootBox.lootBoxUpdated -= Redraw;
    }

    private void Redraw() 
    {
        DestroyAllChild();
        for (int i = 0; i < currentLootBox.GetSize(); i++) 
        {
            var itemUI = Instantiate(lootSlotPrefab, transform);
            itemUI.Setup(currentLootBox, i);
        }

    }

    private void DestroyAllChild() 
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }


}
