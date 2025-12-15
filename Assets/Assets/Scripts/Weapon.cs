using Unity.VisualScripting;

[System.Serializable]// This attribute makes the class serializable in Unity

public class Weapon
{
    public WeaponType weaponType;
    public int bulletsInMagazine;
    public int magazineCapacity;

    public int totalReserveAmmo;

    public bool CanShoot() 
    {
        return HaveEnoughBullets();
    }

    private bool HaveEnoughBullets()
    {
        if (bulletsInMagazine > 0)
        {
            bulletsInMagazine--;
            return true;
        }

        return false;
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
}
public enum WeaponType
{
    Pistol,
    Revolver,
    AutoRifle,
    Shotgun,
    Rifle
}
