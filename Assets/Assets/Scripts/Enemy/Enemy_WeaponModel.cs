using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_WeaponModel : MonoBehaviour
{
    public Enemy_MeleeWeaponType weaponType;
    public AnimatorOverrideController overrideController;
    public Enemy_MeleeWeaponData weaponData;

    [Header("Damage Atribute")]
    public Transform[] damagePoints;
    public float attackRadius;

    private void OnDrawGizmos()
    {
        if (damagePoints.Length > 0) 
        {
            foreach (Transform point in damagePoints) 
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(point.position, attackRadius);
            }   
        }
    }
}
