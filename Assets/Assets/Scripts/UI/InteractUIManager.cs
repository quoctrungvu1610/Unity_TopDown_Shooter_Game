using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class InteractUIManager : MonoBehaviour
{
    [SerializeField] private RectTransform canvasTransformParent;
    public static InteractUIManager instance;

    private GameObject currentUIObject;
    private GameObject currentInteractObject;
    private Camera mainCamera;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (currentUIObject != null && currentInteractObject != null)
        {
            Vector3 screenPos = mainCamera.WorldToScreenPoint(currentInteractObject.transform.position);
            currentUIObject.transform.position = screenPos;
        }
    }

    public void SpawnInteractUI(GameObject interactObject, GameObject UI)
    {
        currentInteractObject = interactObject;
        currentUIObject = Instantiate(UI, canvasTransformParent);
    }

    public void DestroyInteractUI()
    {
        if (currentUIObject != null)
            Destroy(currentUIObject);

        currentUIObject = null;
        currentInteractObject = null;
    }
}
