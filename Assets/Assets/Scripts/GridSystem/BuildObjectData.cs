using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

[System.Serializable]
public struct ObjectIngredient
{
    public InventoryItem item;
    public int quantity;
}

[CreateAssetMenu(menuName = ("Build/Build Object"))]
public class BuildObjectData : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField] private string objectID;
    [SerializeField] private string objectName;
    [SerializeField] private string objectDescription;
    [SerializeField] private float objectPrice;
    [SerializeField] private Sprite objectIcon;
    [SerializeField] private Vector2Int objectSize;
    [SerializeField] private GameObject objectPrefab;
    [SerializeField] private List<ObjectIngredient> ingredients;

    static Dictionary<string, BuildObjectData> buildObjectLookupCache;

    public static BuildObjectData GetFromID(string ID) 
    {
        if (buildObjectLookupCache == null) 
        {
            buildObjectLookupCache = new Dictionary<string, BuildObjectData>();
            var buildItemList = Resources.LoadAll<BuildObjectData>("");

            foreach (var buildItem in buildItemList) 
            {
                if (buildObjectLookupCache.ContainsKey(buildItem.objectID)) 
                {
                    continue;
                }
                buildObjectLookupCache[buildItem.objectID] = buildItem;
            }
        }

        if (ID == null || !buildObjectLookupCache.ContainsKey(ID)) return null;
        return buildObjectLookupCache[ID];

    }

    public string GetObjectID() 
    {
        return objectID;
    }

    public string GetObjectName() 
    {
        return objectName;
    }

    public string GetObjectDescription() 
    {
        return objectDescription;
    }

    public float GetObjectPrice() 
    {
        return objectPrice;
    }

    public Sprite GetObjectIcon() 
    {
        return objectIcon;
    }

    public Vector2Int GetObjectSize() 
    {
        return objectSize;
    }

    public GameObject GetObjectPrefab() 
    {
        return objectPrefab;
    }

    public IEnumerable<ObjectIngredient> GetObjectIngredients() 
    {
        return ingredients;
    }


    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        if (string.IsNullOrWhiteSpace(objectID))
        {
            objectID = System.Guid.NewGuid().ToString();
        }
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {

    }
}
