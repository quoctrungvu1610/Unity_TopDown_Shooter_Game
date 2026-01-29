using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Enemy Data/Range Weapon Data")]
public class Enemy_RangeWeaponData : ScriptableObject
{
    [Header("Weapon Details")]
    public Enemy_RangeWeaponType weaponType;
    public float fireRate = 1f;

    public int minBulletsPerAttack = 1;
    public int maxBulletsPerAttack = 1;

    public float minWeaponCooldown = 2f;
    public float maxWeaponCooldown = 3f;

    [Header("Bullet Details")]
    public float bulletSpeed = 20f;
    public float weaponSpread = 0.1f;

    public int GetBulletsPerAttack()
    {
        return Random.Range(minBulletsPerAttack, maxBulletsPerAttack);
    }

    public float GetWeaponCooldown()
    {
        return Random.Range(minWeaponCooldown, maxWeaponCooldown);
    }

    public Vector3 ApplyWeaponSpread(Vector3 originalDirection)
    {
        float randomizedValue = Random.Range(-weaponSpread, weaponSpread);

        Quaternion spreadRotation = Quaternion.Euler(0, randomizedValue, randomizedValue);

        return spreadRotation * originalDirection;
    }

}
