using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletType
{
    Regular,
    Explosive,
    Piercing,
    Buckshot,
}


[CreateAssetMenu(fileName = "New Bullet Data", menuName = "Weapon System/Bullet Data")]
public class BulletData : ScriptableObject
{
    [SerializeField] private string bulletName;
    [SerializeField] private BulletType bulletType;
    [SerializeField] private string caliber;
    [SerializeField] private int bulletDamage;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float imactForce;
    [SerializeField] private float flyDistance;
    [SerializeField] private GameObject bulletPrefab;

    public BulletType GetBulletType() 
    {
        return bulletType;
    }

    public int GetBulletDamage() 
    {
        return bulletDamage;
    }

    public float GetBulletSpeed() 
    {
        return bulletSpeed;
    }

    public float GetImpactForce() 
    {
        return imactForce;
    }

    public string GetBulletName() 
    {
        return bulletName;
    }

    public float GetFlyDistance() 
    {
        return flyDistance;
    }

    public GameObject GetBulletPrefab() 
    {
        return bulletPrefab;
    }
}
