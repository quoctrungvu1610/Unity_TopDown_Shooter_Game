using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    private Player player;
    private PlayerControls controls;

    [Header("Aim Visual - Laser")]
    [SerializeField] private LineRenderer aimLaser;

    [Header("Aim Control")]
    [SerializeField] private Transform aim;
    [SerializeField] private bool isAimingPrecisely = false;
    [SerializeField] private bool isLockingToTarget = false;

    [Header("Camera Control")]
    [SerializeField] private Transform cameraTarget;
    [Range(.5f, 1)]
    [SerializeField] private float minCameraDistance = 1.5f;
    [Range(1, 1.5f)]
    [SerializeField] private float maxCameraDistance = 4f;
    [Range(3f, 5f)]
    [SerializeField] private float cameraSeneitivity = 5f;

    [Space]

    [SerializeField] private LayerMask aimLayerMask;
    
    private Vector2 mouseInput;
    private RaycastHit lastKnownMouseHit;

    private void Start()
    {
        player = GetComponent<Player>();
        AsignInputEvents();
    }

    private void Update()
    {
        //refactor later
        if (Input.GetKeyDown(KeyCode.P))
        {
            isAimingPrecisely = !isAimingPrecisely;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            isLockingToTarget = !isLockingToTarget;
        }

        UpdateAimPosition();
        UpdateCameraPosition();
        UpdateAimLaser();
    }

    private void UpdateAimLaser()
    {
        Transform gunPoint = player.weapon.GunPoint();
        Vector3 laserDirection = player.weapon.BulletDirection();

        float laserTipLength = .5f;
        float gunDistance = 4f;

        Vector3 endPoint = gunPoint.position + laserDirection * gunDistance;

        if(Physics.Raycast(gunPoint.position, laserDirection, out RaycastHit hitInfo, gunDistance))
        {
            endPoint = hitInfo.point;
            laserTipLength = 0;
        }

        aimLaser.SetPosition(0, gunPoint.position);
        aimLaser.SetPosition(1, endPoint);
        aimLaser.SetPosition(2, endPoint + laserDirection * laserTipLength);
    }

    private void UpdateAimPosition()
    {
        Transform target = Target();

        if (isLockingToTarget && target != null)
        {
            if(target.GetComponent<Renderer>() != null)
            {
                aim.position = target.GetComponent<Renderer>().bounds.center;
            }
            else 
            {
                aim.position = target.position;
            }
            return;
        }

        aim.position = GetMouseHitInfo().point;
        if (!isAimingPrecisely)
            aim.position = new Vector3(GetMouseHitInfo().point.x, transform.position.y + 1, GetMouseHitInfo().point.z);
    }

    public bool CanAimPrecisely()
    {
        return isAimingPrecisely;
    }

    public Transform Target() 
    {
        Transform target = null;
        if(GetMouseHitInfo().transform.GetComponent<Target>() != null)
        {
            target = GetMouseHitInfo().transform;
        }
        return target;
    }

    public Transform Aim()
    {
        return aim;
    }

    public RaycastHit GetMouseHitInfo()
    {
        Ray ray = Camera.main.ScreenPointToRay(mouseInput);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, aimLayerMask))
        {
            lastKnownMouseHit = hit;
            return hit;
        }
        return lastKnownMouseHit;
    }

    #region Camera Region

    private Vector3 DesieredCameraPosition() 
    {
        float actualMaxCameraDistance = player.movement.moveInput.y < -.5f ? minCameraDistance : maxCameraDistance;//????
        //Steps to get the desiered camera position

        Vector3 desieredCameraPosition;
        Vector3 aimDirection = (GetMouseHitInfo().point - transform.position).normalized;

        float distanceToDisieredPosition = Vector3.Distance(transform.position, GetMouseHitInfo().point);
        float clampedDistance = Mathf.Clamp(distanceToDisieredPosition, minCameraDistance, maxCameraDistance);//Use Mathf.Clamp to limit the distance between min and max

        desieredCameraPosition = transform.position + aimDirection * clampedDistance;
       
        desieredCameraPosition.y = transform.position.y + 1;

        return desieredCameraPosition;
    }

    private void UpdateCameraPosition() 
    {
        cameraTarget.position = Vector3.Lerp(cameraTarget.position, DesieredCameraPosition(), cameraSeneitivity * Time.deltaTime);
    }

    #endregion

    private void AsignInputEvents()
    {
        controls = player.controls;

        controls.Character.Aim.performed += context => mouseInput = context.ReadValue<Vector2>();
        controls.Character.Aim.canceled += context => mouseInput = Vector2.zero;
    }
}
