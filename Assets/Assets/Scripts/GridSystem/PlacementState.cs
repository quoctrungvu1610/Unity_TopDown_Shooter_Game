using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementState : IBuildingState
{
    private int currentRotationAngle = 0;
    private int selectedObjectIndex = -1;
    private Vector2Int originalSize;
    private Vector2Int activeSize;

    private string ID;
    private Grid grid;
    private PreviewSystem previewSystem;
    private BuildObjectData data;
    private GridData floorData;
    private GridData furnitureData;
    private ObjectPlacer objectPlacer;

    public PlacementState(string ID,
                          Grid grid,
                          PreviewSystem previewSystem,
                          BuildObjectData data,
                          GridData floorData,
                          GridData furnitureData,
                          ObjectPlacer objectPlacer)
    {
        this.ID = ID;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.data = data;
        this.floorData = floorData;
        this.furnitureData = furnitureData;
        this.objectPlacer = objectPlacer;

        //selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
        if (data != null)
        {
            originalSize = data.GetObjectSize();
            activeSize = originalSize;
            previewSystem.StartShowingPlacementPreview(data.GetObjectPrefab(), originalSize);
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
        bool placementValidity = CheckPlacementValidity(gridPosition);
        if (placementValidity == false) return;

        Vector3 worldPosition = grid.CellToWorld(gridPosition);
        int index = objectPlacer.PlaceObject(
            data.GetObjectPrefab(),
            worldPosition,
            Quaternion.Euler(0, currentRotationAngle, 0)
        );

        GameObject placedObject = objectPlacer.GetPlacedObject(index);
        previewSystem.AlignAnyObjectToGridCenter(placedObject, activeSize, worldPosition);

        GridData selectedData = data.GetObjectID() == "0" ? floorData : furnitureData;
        selectedData.AddObjectAt(
            gridPosition,
            activeSize,
            data.GetObjectID(),
            index
        );
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition)
    {
        GridData selectedData = ID == "0" ? floorData : furnitureData;
        return selectedData.CanPlaceObjectAt(gridPosition, activeSize);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), CheckPlacementValidity(gridPosition));
    }
}
