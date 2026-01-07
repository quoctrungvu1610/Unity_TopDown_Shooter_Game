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

    public WeaponModel CurrentWeaponModel()
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

    public void PlayFireAnimation()
    {
        anim.SetTrigger("Fire");
    }

    public void PlayReloadAnimation()
    {
        //if(isEquipingWeapon) return;

        float reloadSpeed = player.weapon.CurrentWeapon().reloadSpeed;
        
        anim.SetFloat("ReloadSpeed", reloadSpeed);
        anim.SetTrigger("Reload");
        ReduceRigWeight();
    }


    public void PlayWeaponEquipAnimation()
    {
        EquipType equipType = CurrentWeaponModel().equipAnimationType; 
        
        float equipmentSpeed = player.weapon.CurrentWeapon().equipmentSpeed;

        leftHandIK.weight = 0;
        ReduceRigWeight();

        anim.SetTrigger("EquipWeapon");
        anim.SetFloat("EquipType", (float)equipType);
        anim.SetFloat("EquipSpeed", equipmentSpeed);

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
            backupModel.Activate(false);
        }
    }

    public void SwitchOnBackupWeaponModel() 
    {
        SwitchOffBackupWeaponModels();

        BackupWeaponModel lowHangWeapon = null;
        BackupWeaponModel backHangWeapon = null;
        BackupWeaponModel sideHangWeapon = null;

        foreach (BackupWeaponModel backupModel in backupWeaponModels) 
        {
            if(backupModel.weaponType == player.weapon.CurrentWeapon().weaponType) 
            {
                continue;
            }

            if (player.weapon.WeaponInSlots(backupModel.weaponType) != null) 
            {
                if(backupModel.HangTypeIs(HangType.LowBackHang)) 
                {
                    lowHangWeapon = backupModel;
                }
                else if(backupModel.HangTypeIs(HangType.BackHang)) 
                {
                    backHangWeapon = backupModel;
                }
                else if(backupModel.HangTypeIs(HangType.SideHang)) 
                {
                    sideHangWeapon = backupModel;
                }
            }

            lowHangWeapon?.Activate(true);
            backHangWeapon?.Activate(true);
            sideHangWeapon?.Activate(true);
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


