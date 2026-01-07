using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HangType { LowBackHang, SideHang, BackHang };

public class BackupWeaponModel : MonoBehaviour
{
    public WeaponType weaponType;
    [SerializeField] private HangType hangType;

    public void Activate(bool activated) 
    {
        gameObject.SetActive(activated);
    }

    public bool HangTypeIs(HangType hangType) 
    {
        return this.hangType == hangType;
    }
}
