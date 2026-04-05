using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewSystem : MonoBehaviour
{
    [SerializeField] private float previewYOffset = 0.06f;
    [SerializeField] private GameObject cellIndicator;
    [SerializeField] private Material previewMaterialsPrefab;

    private GameObject previewObject;
    private Material previewMaterialInstance;
    private Renderer cellIndicatorRenderer;

    private Renderer[] previewRenderers;
    private Vector2Int currentPreviewSize;

    private void Start()
    {
        previewMaterialInstance = new Material(previewMaterialsPrefab);
        cellIndicator.SetActive(false);
        cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();
    }

    public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size)
    {
        previewObject = Instantiate(prefab);
        PreparePreview(previewObject);
        PrepareCursor(size);
        cellIndicator.SetActive(true);

        currentPreviewSize = size;
        previewRenderers = previewObject.GetComponentsInChildren<Renderer>();
        AlignMeshToGridCenter(size);
    }

    public void PrepareCursor(Vector2Int size)
    {
        if (size.x > 0 && size.y > 0)
        {
            cellIndicator.transform.localScale = new Vector3(size.x, 1, size.y);
            cellIndicatorRenderer.material.mainTextureScale = size;
        }
    }

    private void PreparePreview(GameObject previewObject)
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = previewMaterialInstance;
            }
            renderer.materials = materials;
        }

        foreach (Transform child in previewObject.transform) 
        {
            if (child.GetComponent<Collider>() != null) 
            {
                child.GetComponent<Collider>().enabled = false;
            }
        }
    }

    public void StopShowingPreview()
    {
        cellIndicator.SetActive(false);
        if (previewObject != null)
        {
            Destroy(previewObject);
        }
        previewRenderers = null;
    }

    public void UpdatePosition(Vector3 position, bool validity)
    {
        MoveCursor(position);
        if (previewObject != null)
        {
            MovePreview(position);
            ApplyFeedbackToPreview(validity);

            Vector2Int currentSize = new Vector2Int(
                Mathf.RoundToInt(cellIndicator.transform.localScale.x),
                Mathf.RoundToInt(cellIndicator.transform.localScale.z)
            );
            AlignMeshToGridCenter(currentSize);
        }
        ApplyFeedbackToCursor(validity);
    }

    private void ApplyFeedbackToPreview(bool validity)
    {
        Color c = validity ? Color.white : Color.red;

        c.a = 0.5f;
        previewMaterialInstance.SetColor("_BaseColor", c);
    }

    private void ApplyFeedbackToCursor(bool validity)
    {
        Color c = validity ? Color.green : Color.red;

        c.a = 0.8f;
        cellIndicatorRenderer.material.color = c;
    }

    private void MoveCursor(Vector3 position)
    {
        cellIndicator.transform.position = position;
    }

    private void MovePreview(Vector3 position)
    {
        previewObject.transform.position = new Vector3(
            position.x,
            position.y + previewYOffset,
            position.z);
    }

    public void StartShowingRemovePreview()
    {
        cellIndicator.SetActive(true);
        PrepareCursor(Vector2Int.one);
        ApplyFeedbackToCursor(false);
    }

    public void UpdateRemovePreview(Vector2Int size)
    {
        PrepareCursor(size);
    }

    public void UpdateRotation(int angle, Vector2Int activeSize)
    {
        if (previewObject != null)
        {
            previewObject.transform.rotation = Quaternion.Euler(0, angle, 0);

            PrepareCursor(activeSize);

            AlignMeshToGridCenter(activeSize);
        }
    }

    private void AlignMeshToGridCenter(Vector2Int activeSize)
    {
        if (previewObject == null || previewRenderers == null || previewRenderers.Length == 0)
            return;

        Vector3 gridCenterWorld = cellIndicator.transform.position + new Vector3(activeSize.x * 0.5f, 0, activeSize.y * 0.5f);

        Bounds combinedBounds = previewRenderers[0].bounds;
        foreach (var r in previewRenderers)
        {
            if (r != null) combinedBounds.Encapsulate(r.bounds);
        }

        Vector3 moveVector = gridCenterWorld - combinedBounds.center;
        moveVector.y = 0;

        for (int i = 0; i < previewObject.transform.childCount; i++)
        {
            previewObject.transform.GetChild(i).position += moveVector;
        }
    }

    public void AlignAnyObjectToGridCenter(GameObject obj, Vector2Int activeSize, Vector3 gridPosition)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers == null || renderers.Length == 0) return;

        Vector3 gridCenterWorld = gridPosition + new Vector3(activeSize.x * 0.5f, 0, activeSize.y * 0.5f);

        Bounds combinedBounds = renderers[0].bounds;
        foreach (var r in renderers)
        {
            if (r != null) combinedBounds.Encapsulate(r.bounds);
        }

        Vector3 moveVector = gridCenterWorld - combinedBounds.center;
        moveVector.y = 0;

        for (int i = 0; i < obj.transform.childCount; i++)
        {
            obj.transform.GetChild(i).position += moveVector;
        }
    }
}
