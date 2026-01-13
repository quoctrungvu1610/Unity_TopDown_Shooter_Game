using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Weapon System/Weapon Data")]
public class Weapon_Data : ScriptableObject
{
    public string weaponName;

    [Header("Magazine Details")]
    public int bulletsInMagazine;
    public int magazineCapacity;
    public int totalReserveAmmo;

    [Header("Regular Shot")]
    public ShootType shootType;
    public int bulletPerShot = 1;
    public float fireRate;

    [Header("Burst Shot")]
    public bool burstAvailable = false;
    public bool burstActive;
    public int burstBulletPerShot = 3;
    public float burstFireRate = 0.5f;
    public float burstFireDelay = 0.1f;

    [Header("Weapon Spread")]
    public float baseSpread;
    public float maxSpread;
    public float spreadIncreaseRate = 0.15f;

    [Header("Weapon Generics")]
    public WeaponType weaponType;
    [Range(1, 3)]
    public float reloadSpeed = 1f;
    [Range(1, 3)]
    public float equipmentSpeed = 1f;
    [Range(4, 8)]
    public float gunDistance = 4f;
    [Range(6, 10)]
    public float cameraDistance = 6f;
}
