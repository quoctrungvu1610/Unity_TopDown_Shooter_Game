using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridBuildSystemUI : MonoBehaviour
{
    [SerializeField] private Transform slotsParent;
    [SerializeField] private GridBuildSystemSlotUI slotPrefab;
    [SerializeField] private Button quitButton;

    [SerializeField] private BuildObjectStore buildObjectStore;
    private Inventory inventory;
    private void Awake()
    {
        quitButton.onClick.AddListener(Close);
        inventory = GameObject.FindWithTag("Player").GetComponent<Inventory>();
        inventory.inventoryUpdated += Redraw;
        if(buildObjectStore != null )
            buildObjectStore.buildStoreUpdated += Redraw;
    }

    private void Start()
    {
        Redraw();
    }

    public void Redraw() 
    {
        if (buildObjectStore == null) 
        {
            return;
        }
        foreach (Transform child in slotsParent) 
        {
            Destroy(child.gameObject);
        }
        foreach (var data in buildObjectStore.GetUnlockedObjects()) 
        {
            GridBuildSystemSlotUI slot = Instantiate(slotPrefab, slotsParent);
            slot.Setup(data.Key, data.Value, buildObjectStore.CanUnlockBuildObject(data.Key), buildObjectStore);
        }
    }

    private void Close() 
    {
        this.gameObject.SetActive(false);
    }
}
