using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    private PlayerWeaponVisual visualController;
    private PlayerWeaponController weaponController;
    private PlayerMovement playerMovement;
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        visualController = GetComponentInParent<PlayerWeaponVisual>();
        weaponController = GetComponentInParent<PlayerWeaponController>();
        playerMovement = GetComponentInParent<PlayerMovement>();
    }

    public void ReloadIsOver()      
    {
        visualController.MaximizeRigWeight();
        weaponController.CurrentWeapon().RefillBullets();
        
        weaponController.UpdateEquipmentData();
        weaponController.SetWeaponReady(true);
        weaponController.SetIsReloading(false);

        CrosshairManager.Instance.FinishReload();
    }

    public void ReturnRig() 
    {
        visualController.MaximizeRigWeight();
        visualController.MaximizeLeftHandWeight();
    }

    public void WeaponEquipingIsOver()      
    {
        weaponController.SetWeaponReady(true);
    }

    public void SwitchOnWeaponModel() 
    {
        visualController.SwitchOnCurrentWeaponModel();
    }
}
