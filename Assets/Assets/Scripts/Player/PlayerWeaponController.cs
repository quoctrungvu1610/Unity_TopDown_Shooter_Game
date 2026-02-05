using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsAlly;
    [Space]
    private Player player;
    private const float REFERENCE_BULLET_SPEED = 10f;

    [SerializeField] private Weapon_Data defaulteaponData;
    [SerializeField] private Weapon currentWeapon;
    private bool weaponReady;
    private bool isShooting;

    [Header("Bullet details")]
    [SerializeField] private float bulletImpactForce = 100;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;
    

    [SerializeField] private Transform weaponHolder;


    [Header("Inventory")]
    [SerializeField] private int maxSlots = 2;
    [SerializeField] private List<Weapon> weaponSlots;

    [SerializeField] private GameObject weaponPickupPrefab;

    private void Start()
    {
        player = GetComponent<Player>();
        AsignInputEvents();

        Invoke("EquipStartingWeapon", 0.1f);
    }

    private void Update()
    {
        if(isShooting) 
        {
            Shoot();
        }
    }

    public bool HasOnlyOneWeapon() 
    {
        return weaponSlots.Count <= 1;
    }

    public Weapon WeaponInSlots(WeaponType weaponType) 
    {
        foreach(Weapon weapon in weaponSlots) 
        {
            if(weapon.weaponType == weaponType) 
            {
                return weapon;
            }
        }
        return null;
    }

    public Weapon CurrentWeapon()
    {
        return currentWeapon;
    }

  
    #region Slots management - Pickup/Equip/Drop/Ready
    private void EquipWeapon(int i) 
    {
        if (i >= weaponSlots.Count)
        {
            return;
        }
        SetWeaponReady(false);

        currentWeapon = weaponSlots[i];
        player.weaponVisuals.PlayWeaponEquipAnimation();
        CameraManager.instance.ChangeCameraDistance(currentWeapon.cameraDistance);
    }

    private void EquipStartingWeapon() 
    {
        weaponSlots[0] = new Weapon(defaulteaponData);

        EquipWeapon(0);
    }

    public void PickupWeapon(Weapon newWeapon) 
    {

        //If weapon already in slots, just add ammo
        if (WeaponInSlots(newWeapon.weaponType) != null) 
        {
            WeaponInSlots(newWeapon.weaponType).totalReserveAmmo += newWeapon.bulletsInMagazine;
            return;
        }
           
        if (weaponSlots.Count >= maxSlots && newWeapon.weaponType != currentWeapon.weaponType) 
        {
            int weaponIndex = weaponSlots.IndexOf(currentWeapon);
            player.weaponVisuals.SwitchOffWeaponModels();
            weaponSlots[weaponIndex] = newWeapon;

            CreateWeaponOnTheGround();
            EquipWeapon(weaponIndex);
            return;
        }

        weaponSlots.Add(newWeapon);
        player.weaponVisuals.SwitchOnBackupWeaponModel();
    }

    private void DropWeapon()
    {
        if (HasOnlyOneWeapon())
        {
            return;
        }

        CreateWeaponOnTheGround();

        weaponSlots.Remove(currentWeapon);
        EquipWeapon(0);
    }

    private void CreateWeaponOnTheGround()
    {
        GameObject droppedWeapon = ObjectPool.instance.GetObject(weaponPickupPrefab, transform);
        droppedWeapon.GetComponent<Pickup_Weapon>()?.SetupPickupWeapon(currentWeapon, transform);
    }

    public void SetWeaponReady(bool ready)
    {
        weaponReady = ready;
    }

    public bool WeaponReady()
    {
        return weaponReady;
    }

    #endregion

    
    public Vector3 BulletDirection()
    {
        Transform aim = player.aim.Aim();
        Vector3 direction = (aim.position - GunPoint().position).normalized;
        if(player.aim.CanAimPrecisely() == false && player.aim.Target() == null)
            direction.y = 0;

        return direction;
    }

    IEnumerator BurstFire()
    {
        SetWeaponReady(false);

        for (int i = 0; i < currentWeapon.bulletsPerShot; i++)
        {
            FireSingleBullet();
            yield return new WaitForSeconds(currentWeapon.burstFireDelay);

            if(i >= currentWeapon.bulletsPerShot - 1) 
            {
                SetWeaponReady(true);
            }
        }
    }

    private void Shoot()
    {
        if (WeaponReady() == false)
        {
            return;
        }

        if (currentWeapon.CanShoot() == false)
        {
            return;
        }
        
        player.weaponVisuals.PlayFireAnimation();

        if (currentWeapon.shootType == ShootType.Single)
        {
            isShooting = false;
        }

        if (currentWeapon.BurstActivated() == true) 
        {
            StartCoroutine(BurstFire());
            return;
        }

        FireSingleBullet();
        TriggerEnemyDodge();

    }

    private void FireSingleBullet()
    {
        currentWeapon.bulletsInMagazine--;

        GameObject newBullet = ObjectPool.instance.GetObject(bulletPrefab, GunPoint());
        newBullet.transform.position = GunPoint().position;
        newBullet.transform.rotation = Quaternion.LookRotation(GunPoint().forward);

        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();

        Bullet bulletScript = newBullet.GetComponent<Bullet>();
        bulletScript.BulletSetup(whatIsAlly, currentWeapon.gunDistance, bulletImpactForce);

        Vector3 bulletDirection = currentWeapon.ApplySpread(BulletDirection());

        rbNewBullet.mass = REFERENCE_BULLET_SPEED / bulletSpeed;
        rbNewBullet.velocity = bulletDirection * bulletSpeed;

        if(currentWeapon.bulletsInMagazine <= 0) 
        {
            Reload();
        }


    }

    private void Reload()
    {
        SetWeaponReady(false);
        player.weaponVisuals.PlayReloadAnimation();
    }


    public Transform GunPoint()
    {
        return player.weaponVisuals.CurrentWeaponModel().gunPoint;
    }

    private void TriggerEnemyDodge() 
    {
        Vector3 rayOrigin = GunPoint().position;
        Vector3 rayDirection = BulletDirection();

        if(Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, Mathf.Infinity)) 
        {
            Enemy_Melee enemy = hit.collider.gameObject.GetComponentInParent<Enemy_Melee>();
            if (enemy != null)
            {
                 enemy.ActivateDodgeRoll();
            }
        }
    }

    #region Asign Input Events
    private void AsignInputEvents()
    {
        PlayerControls controls = player.controls;
        controls.Character.Fire.performed += context => isShooting = true;
        controls.Character.Fire.canceled += context => isShooting = false;

        controls.Character.EquipSlot1.performed += context => EquipWeapon(0);
        controls.Character.EquipSlot2.performed += context => EquipWeapon(1);
        controls.Character.EquipSlot3.performed += context => EquipWeapon(2);
        controls.Character.EquipSlot4.performed += context => EquipWeapon(3);
        controls.Character.EquipSlot5.performed += context => EquipWeapon(4); 

        controls.Character.DropCurrentWeapon.performed += context => DropWeapon();
        controls.Character.Reload.performed += context =>
        {
            if(currentWeapon.CanReload() && WeaponReady())
            {
                Reload();
            }
        };

        controls.Character.ToggleWeaponMode.performed += context => currentWeapon.ToggleBurstMode();

    }

    #endregion
}
