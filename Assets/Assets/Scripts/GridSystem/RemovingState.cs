using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Quest;

public class RemovingState : IBuildingState
{
    private int gameObjectIndex = -1;
    //int ID;
    Grid grid;
    PreviewSystem previewSystem;
    GridData floorData;
    GridData furnitureData;
    ObjectPlacer objectPlacer;

    public RemovingState(//int iD,
                         Grid grid,
                         PreviewSystem previewSystem,
                         GridData floorData,
                         GridData furnitureData,
                         ObjectPlacer objectPlacer)
    {
        //ID = iD;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.floorData = floorData;
        this.furnitureData = furnitureData;
        this.objectPlacer = objectPlacer;

        previewSystem.StartShowingRemovePreview();
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        GridData selectedData = null;
        if (furnitureData.CanPlaceObjectAt(gridPosition, Vector2Int.one) == false)
        {
            selectedData = furnitureData;
        }
        else if(floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one) == false)
        {
            selectedData = floorData;
        }

        if (selectedData == null)
        {


        }
        else 
        {
            gameObjectIndex = selectedData.GetRepresentationIndex(gridPosition);
            if (gameObjectIndex == -1)
                return;
            selectedData.RemoveObjectAt(gridPosition);
            objectPlacer.RemoveObjectAt(gameObjectIndex);
        }

        Vector3 cellPostion = grid.CellToWorld(gridPosition);
        previewSystem.UpdatePosition(cellPostion, CheckIfSelectionIsValid(gridPosition));
    }

    private bool CheckIfSelectionIsValid(Vector3Int gridPosition)
    {
        return !(furnitureData.CanPlaceObjectAt(gridPosition, Vector2Int.one) && floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one));
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool validity = CheckIfSelectionIsValid(gridPosition);
        Vector2Int objectSize = Vector2Int.one;
        Vector3Int originPos = gridPosition;
        if (validity)
        {
            // 1. Tìm xem data nào chứa object này
            GridData selectedData = null;
            if (furnitureData.CanPlaceObjectAt(gridPosition, Vector2Int.one) == false)
                selectedData = furnitureData;
            else if (floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one) == false)
                selectedData = floorData;

            if (selectedData != null)
            {
                // 2. Lấy vị trí gốc thực sự của object để indicator không bị nhảy lung tung
                originPos = selectedData.GetObjectOrigin(gridPosition);

                // 3. Lấy size (Mày cần cập nhật hàm GetObjectSizeAt như tao nói ở post trước nhé)
                objectSize = selectedData.GetObjectSizeAt(gridPosition);
            }
        }

        // Luôn update vị trí dựa trên originPos thay vì gridPosition của chuột
        previewSystem.UpdatePosition(grid.CellToWorld(originPos), validity);

        // Cập nhật size cho cái khung bao quanh
        // (Trong PreviewSystem.cs, hàm PrepareCursor sẽ lo phần scale)
        previewSystem.PrepareCursor(objectSize);
    }
}
