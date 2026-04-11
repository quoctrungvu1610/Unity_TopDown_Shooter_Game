using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsAlly;
    [SerializeField] private float defaultCameraDistance = 3f;
    [Space]
    private Player player;
    private const float REFERENCE_BULLET_SPEED = 10f;

    [SerializeField] private Weapon currentWeapon;
    [SerializeField] private Weapon backupWeapon;
    private bool weaponReady;
    private bool isShooting;

    [Header("Bullet details")]
    [SerializeField] private float bulletImpactForce = 100;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;
    

    [SerializeField] private Transform weaponHolder;
    [SerializeField] private GameObject weaponPickupPrefab;

    //Equipment
    private Equipment equipment;
    private Inventory inventory;

    private BulletItem mainWeaponAmmoItem;
    private BulletItem backUpWeaponAmmoItem;

    private WeaponEquipableItem weaponInMainSlot;
    private WeaponEquipableItem weaponInBackupSlot;

    private Weapon mainWeaponInstance;
    private Weapon backupWeaponInstance;


    private void Awake()
    {
        equipment = GetComponent<Equipment>();
        inventory = GetComponent<Inventory>();
        player = GetComponent<Player>();

        if (equipment && inventory)
        {
            equipment.equipmentUpdated += EquipWeapon;
        }

    }

    private void Start()
    {
        AsignInputEvents();

        Invoke("EquipStartingWeapon", 0.1f);
    }


    private void Update()
    {
        if (isShooting)
        {
            Shoot();
        }
    }

    public bool HasOnlyOneWeapon()
    {
        WeaponEquipableItem weaponInSlot = equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponEquipableItem;
        WeaponEquipableItem weaponInSlotBackupSlot = equipment.GetItemInSlot(EquipLocation.BackupWeapon) as WeaponEquipableItem;
        return (weaponInSlot != null && weaponInSlotBackupSlot == null) || (weaponInSlot == null && weaponInSlotBackupSlot != null);
    }


    public Weapon CurrentWeapon()
    {
        if (currentWeapon == null)
        {
            return null;
        }
        return currentWeapon;
    }

    public Weapon BackupWeapon()
    {
        if (backupWeapon == null)
        {
            return null;
        }
        return backupWeapon;
    }

    #region Slots management - Pickup/Equip/Drop/Ready
    private void EquipWeapon(int i)
    {
        SetWeaponReady(false);

        if (i == 1)
        {
            if (backupWeapon == null) return;
            currentWeapon = backupWeaponInstance;
            backupWeapon = mainWeaponInstance;
            
        }
        else
        {
            if (currentWeapon == null) return;
            currentWeapon = mainWeaponInstance;
            backupWeapon = backupWeaponInstance;
        }
        player.weaponVisuals.PlayWeaponEquipAnimation();
        ChangeCameraDistance(currentWeapon != null ? currentWeapon.cameraDistance : defaultCameraDistance);
    }

    private void EquipWeapon()
    {
        SetWeaponReady(false);

        weaponInMainSlot = equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponEquipableItem;
        weaponInBackupSlot = equipment.GetItemInSlot(EquipLocation.BackupWeapon) as WeaponEquipableItem;

        //mainWeaponAmmoItem = CheckIfAmmoIsCompatible(weaponInMainSlot, equipment.GetBulletDataInSlot(EquipLocation.MainWeaponAmmo)) ? equipment.GetBulletDataInSlot(EquipLocation.MainWeaponAmmo) : null;
        //backUpWeaponAmmoItem = CheckIfAmmoIsCompatible(weaponInBackupSlot, equipment.GetBulletDataInSlot(EquipLocation.BackupWeaponAmmo)) ? equipment.GetBulletDataInSlot(EquipLocation.BackupWeaponAmmo) : null; ;

        mainWeaponInstance = CreateOrUpdateWeapon(weaponInMainSlot, EquipLocation.MainWeaponAmmo, mainWeaponInstance); 
        backupWeaponInstance = CreateOrUpdateWeapon(weaponInBackupSlot, EquipLocation.BackupWeaponAmmo, backupWeaponInstance);

        currentWeapon = mainWeaponInstance;
        backupWeapon = backupWeaponInstance;

        if (weaponInMainSlot != null)
            player.weaponVisuals.PlayWeaponEquipAnimation();

        ChangeCameraDistance(currentWeapon != null ? currentWeapon.cameraDistance : defaultCameraDistance);

        player.weaponVisuals.SwitchOnBackupWeaponModel();
        player.weaponVisuals.SwitchOnCurrentWeaponModel();
    }

    private bool CheckIfAmmoIsCompatible(WeaponEquipableItem weaponItem, BulletItem bulletItem) 
    {
        if (weaponItem == null || bulletItem == null) return false;
        return weaponItem.GetWeaponData().compatibleBullets.Contains(bulletItem.GetBulletData());
    }

    private void ChangeCameraDistance(float distance) 
    {
        CameraManager.instance.ChangeCameraDistance(distance);
    }

    private Weapon CreateWeaponInstance(WeaponEquipableItem weaponItem, BulletItem bulletItem, EquipLocation equipLocation) 
    {
        if(weaponItem == null) return null;
        Weapon weapon = new Weapon(weaponItem.GetWeaponData());
        if(bulletItem != null) 
        {
            weapon.SetCurrentBulletData(bulletItem.GetBulletData());
            weapon.SetTotalReserveAmmo(equipment.GetNumberInSlot(equipLocation == EquipLocation.Weapon ? EquipLocation.MainWeaponAmmo : EquipLocation.BackupWeaponAmmo));
        }

        return weapon;
    }

    private Weapon CreateOrUpdateWeapon(WeaponEquipableItem weaponItem, EquipLocation ammoSlot, Weapon existingWeapon)
    {
        if (weaponItem == null) return null;

        WeaponData data = weaponItem.GetWeaponData();
        BulletItem ammoItem = equipment.GetBulletDataInSlot(ammoSlot);
        BulletData validBullet = null;

        if (CheckIfAmmoIsCompatible(weaponItem, ammoItem) == false && ammoItem != null) 
        {
            validBullet = ammoItem.GetBulletData();
            inventory.AddToFirstEmptySlot(ammoItem, equipment.GetNumberInSlot(ammoSlot));
            equipment.RemoveItem(ammoSlot);
        }

        int reserveAmmo = equipment.GetNumberInSlot(ammoSlot);

        if (existingWeapon == null)
        {
            existingWeapon = new Weapon(data);
        }
        else
        {
            existingWeapon.UpdateWeaponStats(data);
        }

        existingWeapon.SetCurrentBulletData(validBullet);
        existingWeapon.SetTotalReserveAmmo(reserveAmmo);

        return existingWeapon;
    }


    private void EquipStartingWeapon()
    {
        EquipWeapon();
    }


    public void SetWeaponReady(bool ready)
    {
        weaponReady = ready;
    }

    public bool WeaponReady()
    {
        return weaponReady;
    }

    public void UpdateEquipmentData() 
    {
        equipment.UpdateNumberInSlot(EquipLocation.MainWeaponAmmo, mainWeaponInstance.GetTotalReserveAmmo());
        equipment.UpdateNumberInSlot(EquipLocation.BackupWeaponAmmo, backupWeaponInstance != null ? backupWeaponInstance.GetTotalReserveAmmo() : 0);
    }

    #endregion


    public Vector3 BulletDirection()
    {
        Transform aim = player.aim.Aim();
        Vector3 direction = (aim.position - GunPoint().position).normalized;
        if (player.aim.CanAimPrecisely() == false && player.aim.Target() == null)
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

            if (i >= currentWeapon.bulletsPerShot - 1)
            {
                SetWeaponReady(true);
            }
        }
    }

    private void Shoot()
    {
        if (currentWeapon == null)
        {
            return;
        }

        if (WeaponReady() == false)
        {
            return;
        }

        if (currentWeapon.CanShoot() == false)
        {
            return;
        }

        player.weaponVisuals.PlayFireAnimation();
        player.weaponVisuals.PlayWeaponMuzzleFlash();

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
        bulletScript.BulletSetup(whatIsAlly, currentWeapon.gunDistance, bulletImpactForce, currentWeapon.damage);

        Vector3 bulletDirection = currentWeapon.ApplySpread(BulletDirection());

        rbNewBullet.mass = REFERENCE_BULLET_SPEED / bulletSpeed;
        rbNewBullet.velocity = bulletDirection * bulletSpeed;

        if (currentWeapon.bulletsInMagazine <= 0)
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

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, Mathf.Infinity))
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

        controls.Character.EquipSlot1.performed += context => EquipWeapon(1);
        controls.Character.EquipSlot2.performed += context => EquipWeapon(2);

        //controls.Character.DropCurrentWeapon.performed += context => DropWeapon();
        controls.Character.Reload.performed += context =>
        {
            if (currentWeapon == null) return;
            if (currentWeapon.CanReload() && WeaponReady())
            {
                Reload();
            }
        };

        controls.Character.ToggleWeaponMode.performed += context => currentWeapon.ToggleBurstMode();

    }


    #endregion
}
