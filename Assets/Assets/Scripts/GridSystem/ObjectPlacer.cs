using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    public List<GameObject> placedGameObject = new List<GameObject>();

    public event Action objectPlacerUpdated;

    public int PlaceObject(GameObject prefab, Vector3 position, Quaternion rotation, BuildObjectData buildData)
    {
        GameObject newObject = Instantiate(prefab, position, rotation);
        placedGameObject.Add(newObject);
        objectPlacerUpdated?.Invoke();

        return placedGameObject.Count - 1;
    }

    public void RemoveObjectAt(int gameObjectIndex)
    {
        if (placedGameObject.Count <= gameObjectIndex || placedGameObject[gameObjectIndex] == null)
        {
            return;
        }
        Destroy(placedGameObject[gameObjectIndex]);
        placedGameObject[gameObjectIndex] = null;

        objectPlacerUpdated?.Invoke();
    }

    public GameObject GetPlacedObject(int index)
    {
        if (index >= 0 && index < placedGameObject.Count)
        {
            return placedGameObject[index];
        }
        return null;
    }

    public void ClearAll()
    {
        foreach (var obj in placedGameObject)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        placedGameObject.Clear();
        objectPlacerUpdated?.Invoke();
    }
}
