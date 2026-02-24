using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class Interactable : MonoBehaviour
{
    protected PlayerWeaponController weaponController;
    protected MeshRenderer mesh;
    [SerializeField] private Material highlightMaterial;
    protected Material defaultMaterial;

    private Outline outline;

    protected void Awake()
    {
        outline = GetComponent<Outline>();
    }

    private void Start()
    {
        if(mesh == null)
        {
            mesh = GetComponentInChildren<MeshRenderer>();
        }

        defaultMaterial = mesh.sharedMaterial;
    }

    protected void UpdateMeshAndMaterial(MeshRenderer newMesh) 
    {
        mesh = newMesh;
        defaultMaterial = newMesh.sharedMaterial;
    }

    public void HighLightActive(bool active)
    {
        if(active)
        {
            //mesh.material = highlightMaterial;
            outline.enabled = true;
        }
        else
        {
            //mesh.material = defaultMaterial;
            outline.enabled = false;
        }
    }

    public virtual void Interaction() 
    {
        Debug.Log("Interacted with " + gameObject.name);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (weaponController == null)
        {
            weaponController = other.GetComponent<PlayerWeaponController>();
        }

        PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();

        if (playerInteraction == null)
        {
            return;
        }

        playerInteraction.GetInteractables().Add(this);
        playerInteraction.UpdateClosestInteractable();
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();

        if (playerInteraction == null)
        {
            return;
        }

        playerInteraction.GetInteractables().Remove(this);
        playerInteraction.UpdateClosestInteractable();
    }
}
