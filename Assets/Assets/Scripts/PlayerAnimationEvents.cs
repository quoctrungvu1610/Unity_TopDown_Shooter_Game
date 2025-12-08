using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    private PlayerWeaponVisual visualController;

    private void Start()
    {
        visualController = GetComponentInParent<PlayerWeaponVisual>();
    }

    public void ReloadIsOver()      
    {
        visualController.MaximizeRigWeight();

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
}
