using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlacementSystem : MonoBehaviour, ISaveable
{
    [SerializeField] private InputManager inputManager;

    [SerializeField] private Grid grid;
    [SerializeField] private GameObject gridVisualization;
    [SerializeField] private PreviewSystem preview;
    [SerializeField] private ObjectPlacer objectPlacer;

    private GridData floorData, furnitureData;
    private Vector3Int lastDetectedPosition = Vector3Int.zero;
    IBuildingState buildingState;


    private void Start()
    {
        StopPlacement();
        if(floorData == null)
            floorData = new GridData();
        if(furnitureData == null)
            furnitureData = new GridData();
    }

    public void StartPlacement(BuildObjectData objectData)
    {
        StopPlacement();
        gridVisualization.SetActive(true);

        PlacementState newState = new PlacementState(objectData.GetObjectID(), grid, preview, objectData, floorData, furnitureData, objectPlacer);
        buildingState = newState;

        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;

        inputManager.OnRotateRight += () => newState.Rotate(1);
        inputManager.OnRotateLeft += () => newState.Rotate(-1);
    }

    public void StartRemoving()
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        buildingState = new RemovingState(grid, preview, floorData, furnitureData, objectPlacer);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI())
        {
            return;
        }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        buildingState.OnAction(gridPosition);

        StopPlacement();
    }

    private void StopPlacement()
    {
        if (buildingState == null) return;

        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;

        inputManager.OnRotateRight -= null;
        inputManager.OnRotateLeft -= null;

        gridVisualization.SetActive(false);
        buildingState.EndState();
        buildingState = null;
    }

    private void Update()
    {
        if (buildingState == null)
        {
            return;
        }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        if (lastDetectedPosition != gridPosition)
        {
            buildingState.UpdateState(gridPosition);
            lastDetectedPosition = gridPosition;
        }
    }

    public ObjectPlacer GetObjectPlacer() 
    {
        return objectPlacer;
    }

    [System.Serializable]
    public class PlacementSaveData
    {
        public List<ObjectSaveData> objects = new List<ObjectSaveData>();
    }

    [System.Serializable]
    public class ObjectSaveData
    {
        public string ID;
        public SerializableVector3 position;
        public int rotation;
        public bool isFloor;
    }

    public object CaptureState()
    {
        PlacementSaveData saveData = new PlacementSaveData();

        foreach (var pair in floorData.GetPlacedObjects())
        {
            if (pair.Key == pair.Value.occupiedPositions[0])
            {
                saveData.objects.Add(new ObjectSaveData
                {
                    ID = pair.Value.ID,
                    position = new SerializableVector3(pair.Key),
                    rotation = pair.Value.Rotation,
                    isFloor = true
                });
            }
        }

        foreach (var pair in furnitureData.GetPlacedObjects())
        {
            if (pair.Key == pair.Value.occupiedPositions[0])
            {
                saveData.objects.Add(new ObjectSaveData
                {
                    ID = pair.Value.ID,
                    position = new SerializableVector3(pair.Key),
                    rotation = pair.Value.Rotation,
                    isFloor = false
                });
            }
        }
        return saveData;
    }

    public void RestoreState(object state)
    {
        if (state is PlacementSaveData data)
        {
            ClearCurrentPlacement();

            foreach (var objData in data.objects)
            {
                RestoreObject(objData);
            }
        }
    }

    private void ClearCurrentPlacement()
    {
        objectPlacer.ClearAll();
        floorData = new GridData();
        furnitureData = new GridData();
    }

    private void RestoreObject(ObjectSaveData objData)
    {
        BuildObjectData itemData = BuildObjectData.GetFromID(objData.ID);

        if (itemData == null) return;

        Vector2Int originalSize = itemData.GetObjectSize();
        Vector2Int activeSize = (objData.rotation == 90 || objData.rotation == 270)
            ? new Vector2Int(originalSize.y, originalSize.x)
            : originalSize;

        Vector3 worldPos = grid.CellToWorld(Vector3Int.FloorToInt(objData.position.ToVector()));
        int index = objectPlacer.PlaceObject(
            itemData.GetObjectPrefab(),
            worldPos,
            Quaternion.Euler(0, objData.rotation, 0),
            itemData
        );

        GameObject placedObject = objectPlacer.GetPlacedObject(index);
        preview.AlignAnyObjectToGridCenter(placedObject, activeSize, worldPos);

        GridData targetData = objData.isFloor ? floorData : furnitureData;
        targetData.AddObjectAt(Vector3Int.FloorToInt(objData.position.ToVector()), activeSize, objData.ID, index, objData.rotation);
    }
}
