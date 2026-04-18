using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipType { SideEquipAnimation, BackEquipAnimation };

public enum HoldType { CommonHold = 1, LowHold,  HighHoldd, Unarmed};


public class WeaponModel : MonoBehaviour
{
    public WeaponType weaponType;
    public EquipType equipAnimationType;
    public HoldType holdType;

    public Transform gunPoint;
    public Transform holdPoint;

    public ParticleSystem muzzleFlash;
    public GameObject muzzleLight;

    public void AddMuzzleLight() 
    {
        muzzleLight.gameObject.SetActive(true);

    }
}
