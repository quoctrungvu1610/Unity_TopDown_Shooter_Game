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

    [SerializeField] private Weapon_Data defaulteaponData;
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

    private void Awake()
    {
        equipment = GetComponent<Equipment>();
        inventory = GetComponent<Inventory>();

        if (equipment && inventory)
        {
            equipment.equipmentUpdated += EquipWeapon;
        }

    }

    private void Start()
    {
        player = GetComponent<Player>();
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

    public Weapon WeaponInSlots(WeaponType weaponType)
    {
        WeaponEquipableItem[] inventoryItems = inventory.GetWeaponInventoryItems();

        foreach (WeaponEquipableItem item in inventoryItems)
        {
            if (item.GetWeaponData().weaponType == weaponType)
            {
                return new Weapon(item.GetWeaponData());
            }
        }
        return null;
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
        if (i == 1)
        {
            if (equipment.GetItemInSlot(EquipLocation.BackupWeapon) as WeaponEquipableItem == null)
            {
                EquipWeapon(null);
                return;
            }
            Weapon_Data weaponData = (equipment.GetItemInSlot(EquipLocation.BackupWeapon) as WeaponEquipableItem).GetWeaponData();
            EquipWeapon(new Weapon(weaponData));
        }
        else
        {
            if (equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponEquipableItem == null)
            {
                EquipWeapon(null);
                return;
            }
            Weapon_Data weaponData = (equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponEquipableItem).GetWeaponData();
            EquipWeapon(new Weapon(weaponData));
        }
    }

    private void EquipWeapon()
    {
        WeaponEquipableItem weaponInSlot = equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponEquipableItem;
        WeaponEquipableItem weaponInBackupSlot = equipment.GetItemInSlot(EquipLocation.BackupWeapon) as WeaponEquipableItem;

        SetWeaponReady(false);

        if (weaponInSlot)
        {
            currentWeapon = new Weapon(weaponInSlot.GetWeaponData());
            player.weaponVisuals.PlayWeaponEquipAnimation();
            CameraManager.instance.ChangeCameraDistance(currentWeapon.cameraDistance);
        }
        else
        {
            CameraManager.instance.ChangeCameraDistance(defaultCameraDistance);
            currentWeapon = null;
        }

        if (weaponInBackupSlot)
        {
            backupWeapon = new Weapon(weaponInBackupSlot.GetWeaponData());
        }
        else
        {
            backupWeapon = null;
        }
        player.weaponVisuals.SwitchOnBackupWeaponModel();
        player.weaponVisuals.SwitchOnCurrentWeaponModel();
    }

    private void EquipWeapon(Weapon weapon)
    {
        SetWeaponReady(false);

        if (weapon != null)
        {
            Weapon main = null;
            Weapon backup = null;

            if ((equipment.GetItemInSlot(EquipLocation.Weapon) != null))
            {
                main = new Weapon((equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponEquipableItem).GetWeaponData());
            }

            if ((equipment.GetItemInSlot(EquipLocation.BackupWeapon) != null))
            {
                backup = new Weapon((equipment.GetItemInSlot(EquipLocation.BackupWeapon) as WeaponEquipableItem).GetWeaponData());
            }

            currentWeapon = weapon;


            if (main != null && weapon.weaponType == main.weaponType)
            {
                backupWeapon = backup;

            }
            else if (backup != null && weapon.weaponType == backup.weaponType)
            {
                backupWeapon = main;
            }

            player.weaponVisuals.PlayWeaponEquipAnimation();
            CameraManager.instance.ChangeCameraDistance(currentWeapon.cameraDistance);
        }
        else
        {
            CameraManager.instance.ChangeCameraDistance(defaultCameraDistance);
            currentWeapon = null;
        }
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
