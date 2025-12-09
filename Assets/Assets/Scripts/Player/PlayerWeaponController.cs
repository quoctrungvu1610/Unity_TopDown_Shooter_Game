using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    private Player player;
    private const float REFERENCE_BULLET_SPEED = 20f;

    [SerializeField] private Weapon currentWeapon;


    [Header("Bullet details")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private Transform gunPoint;


    [SerializeField] private Transform weaponHolder;


    [Header("Inventory")]
    [SerializeField] private int maxSlots = 2;
    [SerializeField] private List<Weapon> weaponSlots;

    private void Start()
    {
        player = GetComponent<Player>();
        AsignInputEvents();

        currentWeapon = weaponSlots[0];
        currentWeapon.ammo = currentWeapon.maxAmmo;
    }


    #region Slots management - Pickup/Equip/Drop
    private void EquipWeapon(int i) 
    {
        currentWeapon = weaponSlots[i];
    }

    public void PickupWeapon(Weapon newWeapon) 
    {
        if(weaponSlots.Count >= maxSlots) 
        {
            Debug.Log("No Slots Avalible ");
            return;
        }
        weaponSlots.Add(newWeapon);
    }

    private void DropWeapon() 
    {
        if (weaponSlots.Count <= 1) 
        {
            return;
        }

        weaponSlots.Remove(currentWeapon);
        currentWeapon = weaponSlots[0];
    }
    #endregion

    public Vector3 BulletDirection()
    {
        Transform aim = player.aim.Aim();
        Vector3 direction = (aim.position - gunPoint.position).normalized;
        if(player.aim.CanAimPrecisely() == false && player.aim.Target() == null)
            direction.y = 0;

        return direction;
    }

    private void Shoot()
    {
        if(currentWeapon.CanShoot() == false) 
        {
            return;
        }

        GameObject newBullet = Instantiate(bulletPrefab, gunPoint.position, Quaternion.LookRotation(gunPoint.forward));

        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();

        rbNewBullet.mass = REFERENCE_BULLET_SPEED / bulletSpeed;
        rbNewBullet.velocity = BulletDirection() * bulletSpeed;

        Destroy(newBullet, 5);
        GetComponentInChildren<Animator>().SetTrigger("Fire");
    }

    public Transform GunPoint()
    {
        return gunPoint;
    }

    #region Asign Input Events
    private void AsignInputEvents()
    {
        player.controls.Character.Fire.performed += context => Shoot();
        player.controls.Character.EquipSlot1.performed += context => EquipWeapon(0);
        player.controls.Character.EquipSlot2.performed += context => EquipWeapon(1);
        player.controls.Character.DropCurrentWeapon.performed += context => DropWeapon();
    }
    #endregion
}
