using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform playerBody;
    public PlayerControls controls { get; private set; }
    public PlayerAim aim { get; private set; }
    public PlayerMovement movement { get; private set; }
    public PlayerWeaponController weapon { get; private set; }
    public PlayerWeaponVisual weaponVisuals { get; private set; }
    public PlayerInteraction interaction { get; private set; }
    public PlayerHealth health { get; private set; }
    public Animator anim { get; private set; }
    public Equipment equipment { get; private set; }
    public BaseStat stat { get; private set; }

    public Ragdoll ragdoll { get; private set; }

    public Shopper shopper { get; private set; }
    public Inventory inventory { get; private set; }
    public Purse purse { get; private set; }
    public Looter looter { get; private set; }

    private void Awake()
    {
        controls = new PlayerControls();
        anim = GetComponentInChildren<Animator>();
        ragdoll = GetComponent<Ragdoll>();
        health = GetComponent<PlayerHealth>();
        aim = GetComponent<PlayerAim>();
        movement = GetComponent<PlayerMovement>();
        weapon = GetComponent<PlayerWeaponController>();
        weaponVisuals = GetComponent<PlayerWeaponVisual>();
        interaction = GetComponent<PlayerInteraction>();
        equipment = GetComponent<Equipment>();
        stat = GetComponent<BaseStat>();
        shopper = GetComponent<Shopper>();
        inventory = GetComponent<Inventory>();
        purse = GetComponent<Purse>();
        looter = GetComponent<Looter>();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
