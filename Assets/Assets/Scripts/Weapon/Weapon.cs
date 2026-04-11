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
    public int damage { get; private set; }
    #endregion

    public WeaponData weaponData { get; private set; } //Serve as default data reference
    public BulletData currentBulletData { get; private set; }

    public Weapon(WeaponData weaponData)
    {
        bulletsInMagazine = weaponData.bulletsInMagazine;
        magazineCapacity = weaponData.magazineCapacity;
        //this.totalReserveAmmo = totalReserveAmmo;

        fireRate = weaponData.fireRate;
        weaponType = weaponData.weaponType;

        bulletsPerShot = weaponData.bulletPerShot;
        shootType = weaponData.shootType;

        burstAvailable = weaponData.burstAvailable;
        burstActive = weaponData.burstActive;
        burstBulletPerShot = weaponData.burstBulletPerShot;
        burstFireRate = weaponData.burstFireRate;
        burstFireDelay = weaponData.burstFireDelay;

        baseSpread = weaponData.baseSpread;
        maximumSpread = weaponData.maxSpread;
        spreadIncreaseRate = weaponData.spreadIncreaseRate;

        reloadSpeed = weaponData.reloadSpeed;
        equipmentSpeed = weaponData.equipmentSpeed;
        gunDistance = weaponData.gunDistance;
        cameraDistance = weaponData.cameraDistance;

        defaultFireRate = fireRate;

        damage = weaponData.damage;

        this.weaponData = weaponData;
        //this.currentBulletData = bulletData;
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
        //Debug.Log($"Checking if {weaponData.name} can shoot. Bullets in magazine: {bulletsInMagazine}, Total reserve ammo: {totalReserveAmmo}");
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

    //public void UpdateCurrentAmmo(int value) 
    //{
    //    //bulletsInMagazine -= value;
    //    Debug.Log($"{weaponData.name} Bullets left in magazine: {bulletsInMagazine}, Bullet in Capacity{totalReserveAmmo}");
    //}

    public BulletData GetCurrentBulletData() 
    {
        return currentBulletData;
    }

    public void SetCurrentBulletData(BulletData bulletData) 
    {
        if (weaponData.compatibleBullets.Contains(bulletData)) 
        {
            currentBulletData = bulletData;
        }
    }

    public int GetTotalReserveAmmo()
    {
        Debug.Log($"Total reserve ammo for {weaponData.name}: {totalReserveAmmo}");
        return totalReserveAmmo;
    }

    public void SetTotalReserveAmmo(int ammo) 
    {
        Debug.Log($"Setting total reserve ammo for {weaponData.name} to {ammo}");
        totalReserveAmmo = ammo;
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
        Debug.Log($"Reloading {weaponData.name}...");
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

        Quaternion spreadRotation = Quaternion.Euler(0, randomizedValue, randomizedValue);

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

    public void UpdateWeaponStats(WeaponData newData)
    {
        bulletsInMagazine = newData.bulletsInMagazine;
        magazineCapacity = newData.magazineCapacity;
        //this.totalReserveAmmo = totalReserveAmmo;

        fireRate = newData.fireRate;
        weaponType = newData.weaponType;

        bulletsPerShot = newData.bulletPerShot;
        shootType = newData.shootType;

        burstAvailable = newData.burstAvailable;
        burstActive = newData.burstActive;
        burstBulletPerShot = newData.burstBulletPerShot;
        burstFireRate = newData.burstFireRate;
        burstFireDelay = newData.burstFireDelay;

        baseSpread = newData.baseSpread;
        maximumSpread = newData.maxSpread;
        spreadIncreaseRate = newData.spreadIncreaseRate;

        reloadSpeed = newData.reloadSpeed;
        equipmentSpeed = newData.equipmentSpeed;
        gunDistance = newData.gunDistance;
        cameraDistance = newData.cameraDistance;

        defaultFireRate = fireRate;

        damage = newData.damage;

        this.weaponData = newData;

    }
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
