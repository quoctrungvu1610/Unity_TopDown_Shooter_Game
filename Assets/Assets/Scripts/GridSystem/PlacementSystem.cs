using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
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
        floorData = new GridData();
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
}
