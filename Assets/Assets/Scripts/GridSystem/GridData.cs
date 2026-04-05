using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GridData
{
    private Vector2Int gridSize = new Vector2Int(20, 20);
    Dictionary<Vector3Int, PlacementData> placedObjects = new();
    private int minBoundary = -10;
    private int maxBoundary = 10;

    public void AddObjectAt(Vector3Int gridPosition, Vector2Int objectSize, string ID, int placeObjectIndex, int rotation) 
    {
        List<Vector3Int> positionToOccupy = CalculatePostions(gridPosition, objectSize);
        PlacementData data = new PlacementData(positionToOccupy, ID, placeObjectIndex, rotation, objectSize);

        foreach (var pos in positionToOccupy) 
        {
            if (placedObjects.ContainsKey(pos)) 
            {
                throw new Exception("Dictionary already contains this cell positon");
            }
            placedObjects[pos] = data;
        
        }
    }

    private List<Vector3Int> CalculatePostions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> returnVal = new List<Vector3Int>();
        for (int x = 0; x < objectSize.x; x++) 
        {
            for (int y = 0; y < objectSize.y; y++) 
            {
                returnVal.Add(gridPosition + new Vector3Int(x, 0, y));
            }
        }
        return returnVal;
    }

    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> positionToOccupy = CalculatePostions(gridPosition, objectSize);
        foreach (var pos in positionToOccupy) 
        {
            if (placedObjects.ContainsKey(pos)) 
            {
                return false;
            }
        }
        return true;
    }

    internal int GetRepresentationIndex(Vector3Int gridPosition)
    {
        if (placedObjects.ContainsKey(gridPosition) == false) 
        {
            return -1;
        }
        return placedObjects[gridPosition].PLacedObjectIndex;
    }

    internal void RemoveObjectAt(Vector3Int gridPosition)
    {
        foreach (var pos in placedObjects[gridPosition].occupiedPositions) 
        {
            placedObjects.Remove(pos);
        }
    }

    public Dictionary<Vector3Int, PlacementData> GetPlacedObjects()
    {
        return placedObjects;
    }

    public Vector2Int GetObjectSizeAt(Vector3Int gridPosition)
    {
        if (placedObjects.ContainsKey(gridPosition))
        {
            return placedObjects[gridPosition].Size;
        }
        return Vector2Int.one;
    }

    public Vector3Int GetObjectOrigin(Vector3Int gridPosition)
    {
        if (placedObjects.ContainsKey(gridPosition))
        {
            return placedObjects[gridPosition].occupiedPositions[0];
        }
        return gridPosition;
    }

    public bool IsInBounds(Vector3Int gridPosition, Vector2Int objectSize)
    {
        // Kiểm tra xem vị trí góc Bottom-Left có âm không
        if (gridPosition.x < minBoundary || gridPosition.z < minBoundary) return false;

        // Kiểm tra xem góc Top-Right của object có vượt quá giới hạn grid không
        if (gridPosition.x + objectSize.x > maxBoundary ||
            gridPosition.z + objectSize.y > maxBoundary)
        {
            return false;
        }

        return true;
    }
}

public class PlacementData 
{
    public List<Vector3Int> occupiedPositions;
    public string ID { get; private set; }
    public int PLacedObjectIndex { get; private set; }
    public int Rotation { get; private set; }
    public Vector2Int Size { get; private set; }

    public PlacementData(List<Vector3Int> occupiedPositions, string ID, int pLacedObjectIndex, int rotation, Vector2Int size)
    {
        this.occupiedPositions = occupiedPositions;
        this.ID = ID;
        PLacedObjectIndex = pLacedObjectIndex;
        this.Rotation = rotation;
        this.Size = size;
    }
}
