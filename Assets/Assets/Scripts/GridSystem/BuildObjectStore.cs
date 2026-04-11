using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class BuildObjectStore : MonoBehaviour, ISaveable
{

    private PlacementSystem placementSystem;
    private Inventory inventory;
    private Dictionary<BuildObjectData, bool> unlockedObjects = new Dictionary<BuildObjectData, bool>();


    public event Action buildStoreUpdated;

    private void Awake()
    {
        inventory = GameObject.FindWithTag("Player").GetComponent<Inventory>();
        placementSystem = GameObject.FindWithTag("PlacementSystem").GetComponent<PlacementSystem>();
    }

    private void Start()
    {
        List<string> defaultDatas = new List<string>() 
        {
            "902b4f17-e5d7-4ac7-aaee-900a7fc18178",
            "91ba02b0-c480-492a-9302-2662eee29134",
            "0f79714b-89d5-40b5-b45c-4f0819a40a20",
            "cf3c0593-8374-45eb-8e29-4e051a80a017",
            "defd98f2-8d07-432d-8756-82956200f84a",
            "fdec0758-e02f-4ff2-912e-0c69a82f64bc",
            "869d509d-e650-48f9-8bf6-3f01961d0a51"
        };
        foreach (var data in defaultDatas) 
        {
            BuildObjectData defaultData = BuildObjectData.GetFromID(data);
            AddUnlockedObjects(defaultData, CanUnlockBuildObject(defaultData));
        }
        //BuildObjectData defaultData = BuildObjectData.GetFromID("902b4f17-e5d7-4ac7-aaee-900a7fc18178");
        //AddUnlockedObjects(defaultData, true);
        buildStoreUpdated?.Invoke();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.J)) 
        {
            BuildObjectData defaultData = BuildObjectData.GetFromID("a03704f5-402f-4c7a-b840-03b002c66c70");
            AddUnlockedObjects(defaultData);
        }
    }

    public static BuildObjectStore GetPlayerBuildObjectStore()
    {
        var player = GameObject.FindWithTag("Player");
        return player.GetComponent<BuildObjectStore>();
    }

    public void AddUnlockedObjects(BuildObjectData objectData, bool isUnlocked = false) 
    {
        if (unlockedObjects.ContainsKey(objectData)) 
        {
            return;
        }

        unlockedObjects[objectData] = isUnlocked;

        buildStoreUpdated?.Invoke();
    }

    public bool CanUnlockBuildObject(BuildObjectData objectData) 
    {
        if (objectData.GetObjectIngredients().Count == 0) 
        {
            return true;
        }
        bool canUnlock = false;
        foreach (var ingredient in objectData.GetObjectIngredients()) 
        {
            if (!inventory.HasItem(ingredient.item)) return false;
            if (inventory.HasItem(ingredient.item))
            {
                if (ingredient.quantity <= inventory.GetItemNumber(ingredient.item))
                {
                    canUnlock = true;
                }
                else 
                {
                    return false;
                }
            }

        }
        return canUnlock;
    }

    public void UnlockObject(BuildObjectData objectData) 
    {
        if (!unlockedObjects.ContainsKey(objectData))
        {
            return;
        }

        unlockedObjects[objectData] = CanUnlockBuildObject(objectData);
        if (CanUnlockBuildObject(objectData)) 
        {
            RemoveIngredient(objectData);
        }

        buildStoreUpdated?.Invoke();
    }

    public void AddToPlacedObject(BuildObjectData objectData) 
    {
        buildStoreUpdated?.Invoke();
    }

    public bool CheckIfCanPlaceObject(BuildObjectData objectData) 
    {
        if(!unlockedObjects.ContainsKey(objectData)) 
        {
            return false;
        }

        int placedObjectCount = GetPlacedObjectsCount(objectData);

        if(placedObjectCount < objectData.GetMaxObjectToPlace()) 
        {
            return true;
        }
        return false;

    }


    private void RemoveObject(BuildObjectData objectData) 
    {
        if (unlockedObjects.ContainsKey(objectData)) 
        {
            unlockedObjects.Remove(objectData);
        }
    }

    private void RemoveIngredient(BuildObjectData objectData) 
    {
        if (objectData.GetObjectIngredients() == null) return;

        foreach (var ingredient in objectData.GetObjectIngredients()) 
        {
            if (inventory.CheckValue(ingredient.item, ingredient.quantity)) 
            {
                inventory.RemoveItem(ingredient.item, ingredient.quantity);
            }
        }
        buildStoreUpdated?.Invoke();
    }

    public Dictionary<BuildObjectData, bool> GetUnlockedObjects() 
    {
        return unlockedObjects;
    }

    public Inventory GetPlayerInventory() 
    {
        return inventory;
    }

    public PlacementSystem GetPlacementSystem() 
    {
        return placementSystem;
    }

    public int GetPlacedObjectsCount(BuildObjectData objectData) 
    {
        return placementSystem.GetObjectPlacer().placedGameObject.Where(obj => obj?.GetComponent<BuildObject>().GetBuildObjectType() == objectData.GetBuildObjectType()).Count();
    }

    [System.Serializable]
    private struct BuildObjectRecord 
    {
        public string ID;
        public bool isUnlocked;
    }

    public object CaptureState()
    {
        List<BuildObjectRecord> records = new List<BuildObjectRecord>();
        foreach (var obj in unlockedObjects) 
        {
            BuildObjectRecord record = new BuildObjectRecord();
            record.ID = obj.Key.GetObjectID();
            record.isUnlocked = obj.Value;

            records.Add(record);
        }
        return records;
    }

    public void RestoreState(object state)
    {
        var records = (List<BuildObjectRecord>)state;

        foreach (var record in records) 
        {
            unlockedObjects[BuildObjectData.GetFromID(record.ID)] = record.isUnlocked;
        }
    }
}
