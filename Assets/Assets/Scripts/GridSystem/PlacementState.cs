using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementState : IBuildingState
{
    private int currentRotationAngle = 0;
    private int selectedObjectIndex = -1;
    private Vector2Int originalSize;
    private Vector2Int activeSize;

    private int ID;
    private Grid grid;
    private PreviewSystem previewSystem;
    private ObjectDatabaseSO database;
    private GridData floorData;
    private GridData furnitureData;
    private ObjectPlacer objectPlacer;

    public PlacementState(int iD,
                          Grid grid,
                          PreviewSystem previewSystem,
                          ObjectDatabaseSO database,
                          GridData floorData,
                          GridData furnitureData,
                          ObjectPlacer objectPlacer)
    {
        this.ID = iD;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.floorData = floorData;
        this.furnitureData = furnitureData;
        this.objectPlacer = objectPlacer;

        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
        if (selectedObjectIndex > -1)
        {
            originalSize = database.objectsData[selectedObjectIndex].Size;
            activeSize = originalSize;
            previewSystem.StartShowingPlacementPreview(database.objectsData[selectedObjectIndex].Prefab, originalSize);
            previewSystem.UpdateRotation(0, activeSize);
        }
    }

    public void RotateRight() => Rotate(1);
    public void RotateLeft() => Rotate(-1);

    public void Rotate(int direction)
    {
        currentRotationAngle = (currentRotationAngle + direction * 90) % 360;
        if (currentRotationAngle < 0) currentRotationAngle += 360;
        activeSize = (currentRotationAngle == 90 || currentRotationAngle == 270)
            ? new Vector2Int(originalSize.y, originalSize.x)
            : originalSize;

        previewSystem.UpdateRotation(currentRotationAngle, activeSize);
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();

    }

    public void OnAction(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        if (placementValidity == false) return;

        Vector3 worldPosition = grid.CellToWorld(gridPosition);
        int index = objectPlacer.PlaceObject(
            database.objectsData[selectedObjectIndex].Prefab,
            worldPosition,
            Quaternion.Euler(0, currentRotationAngle, 0)
        );

        GameObject placedObject = objectPlacer.GetPlacedObject(index);
        previewSystem.AlignAnyObjectToGridCenter(placedObject, activeSize, worldPosition);

        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? floorData : furnitureData;
        selectedData.AddObjectAt(
            gridPosition,
            activeSize,
            database.objectsData[selectedObjectIndex].ID,
            index
        );
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        GridData selectedData = ID == 0 ? floorData : furnitureData;
        return selectedData.CanPlaceObjectAt(gridPosition, activeSize);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), CheckPlacementValidity(gridPosition, selectedObjectIndex));
    }
}
