using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerWeaponVisual : MonoBehaviour
{
    private Rig rig;
    private Animator anim;
    private bool isGrabbingWeapon = false;


    #region Gun transforms region
    [SerializeField] private Transform[] gunTransforms;

    [SerializeField] private Transform pistol;
    [SerializeField] private Transform revolver;
    [SerializeField] private Transform autoRifle;
    [SerializeField] private Transform shotgun;
    [SerializeField] private Transform rifle;

    private Transform currentGun;
    #endregion

    [Header("Left Hand IK")]
    [SerializeField] private float leftHandIKWeightIncreaseRate;
    [SerializeField] private TwoBoneIKConstraint leftHandIK;
    [SerializeField] private Transform leftHandIK_Target;
    private bool shouldIncrease_LeftHandIKWeight;

    [Header("Rig")]
    [SerializeField] private float rigWeightIncreaseRate;
    private bool shouldIncrease_RigWeight = false;


    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rig = GetComponentInChildren<Rig>();

        SwitchOn(pistol);
    }

    private void Update()
    {
        CheckWeaponSwitch();

        if (Input.GetKeyDown(KeyCode.R) && isGrabbingWeapon == false)
        {
            anim.SetTrigger("Reload");
            ReduceRigWeight();
        }

        UpdateRigWeight();
        UpdateLeftHandIKWeight();
    }

    private void UpdateLeftHandIKWeight()
    {
        if (shouldIncrease_LeftHandIKWeight)
        {
            leftHandIK.weight += leftHandIKWeightIncreaseRate * Time.deltaTime;
            if (leftHandIK.weight >= 1)
            {
                leftHandIK.weight = 1;
                shouldIncrease_LeftHandIKWeight = false;
            }
        }
    }

    private void UpdateRigWeight()
    {
        if (shouldIncrease_RigWeight)
        {
            rig.weight += rigWeightIncreaseRate * Time.deltaTime;
            if (rig.weight >= 1)
            {
                rig.weight = 1;
                shouldIncrease_RigWeight = false;
            }
        }
    }

    private void ReduceRigWeight()
    {
        rig.weight = 0.2f;
    }

    private void PlayeWeaponGrabAnimation(GrabType grabType)
    {
        leftHandIK.weight = 0;
        ReduceRigWeight();
        anim.SetFloat("WeaponGrabType", (float)grabType);
        anim.SetTrigger("WeaponGrab");
        SetBusyGrabbingWeaponTo(true);
    }

    public void SetBusyGrabbingWeaponTo(bool busy)
    {
        isGrabbingWeapon = busy;
        anim.SetBool("BusyGrabbingWeapon", isGrabbingWeapon);
    }

    public void MaximizeRigWeight() 
    {
        shouldIncrease_RigWeight = true;
    }

    public void MaximizeLeftHandWeight() 
    {
        shouldIncrease_LeftHandIKWeight = true;
    }

    private void SwitchOn(Transform gunTransform) 
    {
        SwitchOffGuns();
        gunTransform.gameObject.SetActive(true);
        currentGun = gunTransform;

        AttachLeftHand();
    }

    private void SwitchOffGuns() 
    {
        for(int i = 0; i < gunTransforms.Length; i++) 
        {
            gunTransforms[i].gameObject.SetActive(false);
        }
    }

    private void AttachLeftHand() 
    {
        Transform targetTransform = currentGun.GetComponentInChildren<LeftHandTargetTransform>().transform;
        leftHandIK_Target.localPosition = targetTransform.localPosition;
        leftHandIK_Target.localRotation = targetTransform.localRotation;
    }

    private void SwitchAnimationlayer(int layerIndex) 
    {
        for (int i = 1; i < anim.layerCount; i++)
        {
            anim.SetLayerWeight(i, 0);
        }
        anim.SetLayerWeight(layerIndex, 1);
    }

    private void CheckWeaponSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchOn(pistol);
            SwitchAnimationlayer(1);
            PlayeWeaponGrabAnimation(GrabType.SideGrab);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchOn(revolver);
            SwitchAnimationlayer(1);
            PlayeWeaponGrabAnimation(GrabType.SideGrab);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchOn(autoRifle);
            SwitchAnimationlayer(1);
            PlayeWeaponGrabAnimation(GrabType.BackGrab);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SwitchOn(shotgun);
            SwitchAnimationlayer(2);
            PlayeWeaponGrabAnimation(GrabType.BackGrab);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SwitchOn(rifle);
            SwitchAnimationlayer(3);
            PlayeWeaponGrabAnimation(GrabType.BackGrab);
        }
    }
}

public enum GrabType { SideGrab, BackGrab };
