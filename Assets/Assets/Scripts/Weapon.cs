using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]// This attribute makes the class serializable in Unity

public class Weapon
{
    public ShootType shootType;
    public int bulletsPerShot { get; private set; }

    #region Regular Mode Variables
    private float defaultFireRate = 2f;
    public float fireRate = 1f;//bullets per second
    private float lastShootTime = 0f;
    #endregion

    #region Burst Mode Variables
    private bool burstAvailable = false;
    public bool burstActive;
    private float burstFireRate;
    private int burstBulletPerShot;
    public float burstFireDelay { get; private set; }
    #endregion

    [Header("Magazine Details")]
    public int bulletsInMagazine;
    public int magazineCapacity;
    public int totalReserveAmmo;

    #region Weapon Spread Variables
    [Header("Spread Amount")]
    private float baseSpread = .5f;
    private float maximumSpread = 5f;
    private float spreadIncreaseRate = 0.15f;
    private float lastSpreadUpdateTime;
    private float currentSpread = .5f;
    private float spreadCooldown = 1f;
    #endregion

    #region Weapion Generics Info Variables
    public WeaponType weaponType;
    public float reloadSpeed { get; private set; }
    public float equipmentSpeed { get; private set; }
    public float gunDistance { get; private set; }
    public float cameraDistance { get; private set; }
    #endregion

    public Weapon(Weapon_Data weapnData) 
    {
        bulletsInMagazine = weapnData.bulletsInMagazine;
        magazineCapacity = weapnData.magazineCapacity;
        totalReserveAmmo = weapnData.totalReserveAmmo;

        fireRate = weapnData.fireRate;
        weaponType = weapnData.weaponType;

        bulletsPerShot = weapnData.bulletPerShot;
        shootType = weapnData.shootType;

        burstAvailable = weapnData.burstAvailable;
        burstActive = weapnData.burstActive;
        burstBulletPerShot = weapnData.burstBulletPerShot;
        burstFireRate = weapnData.burstFireRate;
        burstFireDelay = weapnData.burstFireDelay;

        baseSpread = weapnData.baseSpread;
        maximumSpread = weapnData.maxSpread;
        spreadIncreaseRate = weapnData.spreadIncreaseRate;

        reloadSpeed = weapnData.reloadSpeed;
        equipmentSpeed = weapnData.equipmentSpeed;
        gunDistance = weapnData.gunDistance;
        cameraDistance = weapnData.cameraDistance;

        defaultFireRate = fireRate;
    }


    #region Burst method

    public bool BurstActivated() 
    {
        if(weaponType == WeaponType.Shotgun) 
        {
            burstFireDelay = 0;
            return true;
        }
        return burstActive;
    } 

    public void ToggleBurstMode() 
    {
        if (burstAvailable)
        {
            burstActive = !burstActive;
        }
        else return;

        if (burstActive)
        {
            bulletsPerShot = burstBulletPerShot;
            fireRate = burstFireRate;
        }
        else 
        {
            bulletsPerShot = 1;
            fireRate = defaultFireRate;
        }
    }

    #endregion

    public bool CanShoot() 
    {
        return HaveEnoughBullets() && ReadyToFire();
    }

    private bool ReadyToFire() 
    {
        if(Time.time >= lastShootTime + (1f / fireRate)) 
        {
            lastShootTime = Time.time;
            return true;
        }
        return false;
    }

    private bool HaveEnoughBullets()
    {
        return bulletsInMagazine > 0;
    }

    public bool CanReload() 
    {
        if(bulletsInMagazine == magazineCapacity) 
        {
            return false;
        }

        if (totalReserveAmmo > 0) 
        {
            return true;
        }

        return false;
    }
    public void RefillBullets() 
    {
        int bulletToReload = magazineCapacity;

        if (bulletToReload > totalReserveAmmo) 
        {
            bulletToReload = totalReserveAmmo;
        }

        totalReserveAmmo -= bulletToReload;
        bulletsInMagazine = bulletToReload;

        if(totalReserveAmmo < 0) 
        {
            totalReserveAmmo = 0;
        }
    }

    #region Spread Methods

    public Vector3 ApplySpread(Vector3 originalDirection)
    {
        UpdateSpread();

        float randomizedValue = Random.Range(-currentSpread,currentSpread);

        Quaternion spreadRotation = Quaternion.Euler(randomizedValue, randomizedValue, randomizedValue);

        return spreadRotation * originalDirection;
    }

    private void UpdateSpread() 
    {
        if (Time.time > lastSpreadUpdateTime + spreadCooldown) 
        {
            currentSpread = baseSpread;
        }
        else
        {
            IncreaseSpread();
        }

        lastSpreadUpdateTime = Time.time;
    }

    private void IncreaseSpread() 
    {
        currentSpread = Mathf.Clamp(currentSpread + spreadIncreaseRate, baseSpread, maximumSpread);
    }

    #endregion
}


public enum WeaponType
{
    Pistol,
    Revolver,
    AutoRifle,
    Shotgun,
    Rifle
}


public enum ShootType
{
    Single,
    Auto
}
