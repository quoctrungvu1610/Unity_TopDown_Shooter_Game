using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    private PlayerWeaponVisual visualController;
    private PlayerWeaponController weaponController;

    private void Start()
    {
        visualController = GetComponentInParent<PlayerWeaponVisual>();
        weaponController = GetComponentInParent<PlayerWeaponController>();
    }

    public void ReloadIsOver()      
    {
        visualController.MaximizeRigWeight();
        weaponController.CurrentWeapon().RefillBullets();
        //Refill bullets in the weapon
    }

    public void ReturnRig() 
    {
        visualController.MaximizeRigWeight();
        visualController.MaximizeLeftHandWeight();
    }

    public void WeaponGrabIsOver()      
    {
        visualController.SetBusyGrabbingWeaponTo(false);
    }

    public void SwitchOnWeaponModel() 
    {
        visualController.SwitchOnCurrentWeaponModel();
    }
}
