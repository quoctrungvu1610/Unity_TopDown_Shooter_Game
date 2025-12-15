using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerWeaponVisual : MonoBehaviour
{
    private Rig rig;
    private Animator anim;
    private bool isGrabbingWeapon = false;
    private Player player;

    [SerializeField] private WeaponModel[] weaponModels;
    [SerializeField] private BackupWeaponModel[] backupWeaponModels;

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
        player = GetComponent<Player>();
        anim = GetComponentInChildren<Animator>();
        rig = GetComponentInChildren<Rig>();
        weaponModels = GetComponentsInChildren<WeaponModel>(true);
        backupWeaponModels = GetComponentsInChildren<BackupWeaponModel>(true);
    }

    private void Update()
    {
        UpdateRigWeight();
        UpdateLeftHandIKWeight();
    }

    private WeaponModel CurrentWeaponModel()
    {
        WeaponModel weaponModel = null;

        WeaponType weaponType = player.weapon.CurrentWeapon().weaponType;

        for (int i = 0; i < weaponModels.Length; i++)
        {
            if (weaponModels[i].weaponType == weaponType)
            {
                weaponModel = weaponModels[i];
            }
        }

        return weaponModel;

    }

    public void PlayReloadAnimation()
    {
        if(isGrabbingWeapon) return;

        anim.SetTrigger("Reload");
        ReduceRigWeight();
    }



    public void PlayWeaponEquipAnimation()
    {
        GrabType grabType = CurrentWeaponModel().grabType; 
        
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


    public void SwitchOnCurrentWeaponModel() 
    {
        int animationIndex = (int)CurrentWeaponModel().holdType;

        SwitchOffBackupWeaponModels();
        if (player.weapon.HasOnlyOneWeapon() == false) 
        {
            SwitchOnBackupWeaponModel();
        }   
        SwitchOffWeaponModels();

        SwitchAnimationlayer(animationIndex);
        CurrentWeaponModel().gameObject.SetActive(true);
        AttachLeftHand();
    }

    private void SwitchOffBackupWeaponModels() 
    {
        foreach(BackupWeaponModel backupModel in backupWeaponModels) 
        {
            backupModel.gameObject.SetActive(false);
        }
    }

    public void SwitchOnBackupWeaponModel() 
    {
        if(player.weapon.BackupWeapon() == null) 
        {
            return;
        }
        WeaponType weaponType = player.weapon.BackupWeapon().weaponType;
        foreach(BackupWeaponModel backupModel in backupWeaponModels) 
        {
            if(backupModel.weaponType == weaponType) 
            {
                backupModel.gameObject.SetActive(true);
            }
        }   
    }

    public void SwitchOffWeaponModels() 
    {
        for(int i = 0; i < weaponModels.Length; i++) 
        {
            weaponModels[i].gameObject.SetActive(false);
        }
    }

    private void AttachLeftHand() 
    {
        Transform targetTransform = CurrentWeaponModel().holdPoint;
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

    #region Animation Rigging Methods
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
    public void MaximizeRigWeight() 
    {
        shouldIncrease_RigWeight = true;
    }
    public void MaximizeLeftHandWeight() 
    {
        shouldIncrease_LeftHandIKWeight = true;
    }

    #endregion
}


