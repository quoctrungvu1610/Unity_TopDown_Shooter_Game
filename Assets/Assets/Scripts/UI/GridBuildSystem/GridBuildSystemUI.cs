using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridBuildSystemUI : MonoBehaviour
{
    [SerializeField] private Transform slotsParent;
    [SerializeField] private GridBuildSystemSlotUI slotPrefab;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button removeStationButton;

    [SerializeField] private BuildObjectStore buildObjectStore;
    private Inventory inventory;
    private void Awake()
    {
        inventory = GameObject.FindWithTag("Player").GetComponent<Inventory>();

        quitButton.onClick.AddListener(Close);
        removeStationButton.onClick.AddListener(RemoveStation);

        if(inventory != null) 
        {
            inventory.inventoryUpdated += Redraw;
        }
  
        if (buildObjectStore != null) 
        {
            buildObjectStore.buildStoreUpdated += Redraw;
            buildObjectStore.GetPlacementSystem().GetObjectPlacer().objectPlacerUpdated += Redraw;
        }
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
            slot.Setup(data.Key, data.Value, buildObjectStore.CanUnlockBuildObject(data.Key), buildObjectStore, buildObjectStore.CheckIfCanPlaceObject(data.Key));
        }
    }

    private void RemoveStation() 
    {
        buildObjectStore.GetPlacementSystem().StartRemoving();
    }

    private void Close() 
    {
        this.gameObject.SetActive(false);
    }
}
