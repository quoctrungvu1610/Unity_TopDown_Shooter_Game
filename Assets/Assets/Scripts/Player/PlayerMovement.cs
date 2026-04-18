using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using static UnityEngine.EventSystems.EventTrigger;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float maxDodgeTime = 0.4f;
    [SerializeField] private float walkSpeed = 1.5f;
    [SerializeField] private float runSpeed = 3.0f;
    [SerializeField] private float turnSpeed = 7f;
    [SerializeField] private float dodgeSpeed;
    [SerializeField] private float dodgeDistance;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float gravityScale = 9.81f;
    [SerializeField] private Image dodgeCooldownImage;

    private Player player;
    private PlayerControls controls;
    private CharacterController characterController;
    private Animator animator;

    private float speed;
    private float verticalVelocity = 0f;
    private Vector3 movementDirection;
    private Vector3 dodgeTarget;
    private Vector3 dodgeDirection;
    private float dodgeTimer;
    private bool isRunning;
    private bool isDodgeCooldown = false;
    private float dodgeCooldownTime = 1f;
    private float dodgeCooldownTimer = 0f;

    public Vector2 moveInput { get; private set; }
    public bool isDodging;

    private Vector3 toTarget;
    private float remainingDistance;
    private float moveDistance;
    private float radius;
    private Vector3 castOrigin;
    private Vector3 beforePosition;
    private float movedDistance;

    private Vector3 flatDirection;
    private Vector3 normalizedDir;

    private Vector3 lookingDirection;
    private Quaternion desireRotation;

    private Vector3 horizontalMovement;
    private Vector3 finalMovement;

    private void Start()
    {
        player = GetComponent<Player>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        speed = walkSpeed;
        dodgeCooldownImage.gameObject.SetActive(false);
        AssignInputEvents();
    }

    private void Update()
    {
        if (player.health.isDead)
        {
            return;
        }

        if (!isDodging)
        {
            ApplyMovement();
            ApplyRotation();
            AnimatorController();
            return;
        }
        ApplyDodge();
    }

    private bool ApplyDodge()
    {
        toTarget = dodgeTarget - transform.position;
        remainingDistance = toTarget.magnitude;


        if (remainingDistance <= 0.05f)
        {
            StopDodge();
            return false;
        }

        dodgeDirection = toTarget.normalized;
        moveDistance = dodgeSpeed * Time.deltaTime;
        moveDistance = Mathf.Min(moveDistance, remainingDistance);
        radius = characterController.radius * 0.9f;
        castOrigin = transform.position + characterController.center;

        if (Physics.SphereCast(
            castOrigin,
            radius,
            dodgeDirection,
            out RaycastHit hit,
            moveDistance,
            obstacleLayer,
            QueryTriggerInteraction.Ignore))
        {

            StopDodge();
            return false;
        }

        beforePosition = transform.position;

        CollisionFlags flags = characterController.Move(
            dodgeDirection * moveDistance
        );

        movedDistance = Vector3.Distance(beforePosition, transform.position);

        if (movedDistance < 0.001f)
        {
            StopDodge();
            return false;
        }

        if ((flags & CollisionFlags.Sides) != 0)
        {
            StopDodge();
            return false;
        }

        return true;
    }

    private void StopDodge()
    {
        animator.SetBool("Dodge", false);
        isDodging = false;
        isDodgeCooldown = true;
        StartCoroutine(StartCooldownDodge());
    }

    private IEnumerator StartCooldownDodge() {    
        dodgeCooldownImage.gameObject.SetActive(true);
        dodgeCooldownTimer = 0f;
        while (dodgeCooldownTimer < dodgeCooldownTime)
        {
            dodgeCooldownTimer += Time.deltaTime;
            dodgeCooldownImage.fillAmount = dodgeCooldownTimer / dodgeCooldownTime;
            yield return null;
        }
        dodgeCooldownImage.fillAmount = 1f;
        dodgeCooldownImage.gameObject.SetActive(false);
        isDodgeCooldown = false;
    }

    private void AnimatorController()
    {
        flatDirection = new Vector3(movementDirection.x, 0f, movementDirection.z);

        float xVelocity = 0f;
        float zVelocity = 0f;

        if (flatDirection.sqrMagnitude > 0.001f)
        {
            normalizedDir = flatDirection.normalized;
            xVelocity = Vector3.Dot(normalizedDir, transform.right);
            zVelocity = Vector3.Dot(normalizedDir, transform.forward);
        }

        animator.SetFloat("xVelocity", xVelocity, 0.1f, Time.deltaTime);
        animator.SetFloat("zVelocity", zVelocity, 0.1f, Time.deltaTime);

        bool playRunAnimation = isRunning && flatDirection.sqrMagnitude > 0.001f;
        animator.SetBool("isRunning", playRunAnimation);
    }

    private void ApplyRotation()
    {
        if(player.weapon.HasMainWeaponEquipped() == false)
        {
            ApplyRotationFreeLook();
            return;
        }
        ApplyRotaionHangingWeapon();
    }

    private void ApplyRotaionHangingWeapon()
    {
        lookingDirection = player.aim.GetMouseHitInfo().point - transform.position;
        lookingDirection.y = 0f;

        if (lookingDirection.sqrMagnitude < 0.001f) return;

        lookingDirection.Normalize();

        desireRotation = Quaternion.LookRotation(lookingDirection);
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            desireRotation,
            turnSpeed * Time.deltaTime
        );
    }

    private void ApplyRotationFreeLook()
    {
        if (moveInput.sqrMagnitude < 0.001f) return;
        Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
        desireRotation = Quaternion.LookRotation(moveDirection);
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            desireRotation,
            turnSpeed * Time.deltaTime
        );
    }

    private void ApplyMovement()
    {
        ApplyGravity();

        horizontalMovement = new Vector3(moveInput.x, 0f, moveInput.y);
        movementDirection = horizontalMovement;
        finalMovement = horizontalMovement * speed;
        finalMovement.y = verticalVelocity;

        characterController.Move(finalMovement * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        if (!characterController.isGrounded)
        {
            verticalVelocity -= gravityScale * Time.deltaTime;
        }
        else
        {
            if (verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }
        }
    }

    private void Dodge()
    {
        if (isDodging) return;
        if(isDodgeCooldown) return;

        animator.SetBool("Dodge", true);
        isDodging = true;

        Vector3 direction = transform.forward;
        float maxDistance = dodgeDistance;

        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, dodgeDistance, obstacleLayer))
        {

            maxDistance = hit.distance - 0.2f;
        }

        dodgeTarget = transform.position + direction * maxDistance;
    }

    private void AssignInputEvents()
    {
        controls = player.controls;

        controls.Character.Movement.performed += context =>
        {
            moveInput = context.ReadValue<Vector2>();
        };

        controls.Character.Movement.canceled += context =>
        {
            moveInput = Vector2.zero;
        };

        controls.Character.Dodge.performed += context => Dodge();

        controls.Character.Run.performed += context =>
        {
            speed = runSpeed;
            isRunning = true;
        };

        controls.Character.Run.canceled += context =>
        {
            speed = walkSpeed;
            isRunning = false;
        };
    }


    //private void Start()
    //{   
    //    player = GetComponent<Player>();

    //    characterController = GetComponent<CharacterController>();
    //    animator = GetComponentInChildren<Animator>();

    //    speed = walkSpeed;

    //    AssignInputEvents();
    //}

    //private void Update()
    //{
    //    if(player.health.isDead)
    //    {
    //        return;
    //    }
    //    if (isDodging == false) 
    //    {
    //        ApplyMovement();
    //        ApplyRotation();
    //        AnimatorController();
    //    }
    //    else if (isDodging)   
    //    {

    //        dodgeDirection = (dodgeTarget - transform.position).normalized;

    //        CollisionFlags flags = characterController.Move(
    //            dodgeDirection * dodgeSpeed * Time.deltaTime
    //        );

    //        if ((flags & CollisionFlags.Sides) != 0)
    //        {
    //            StopDodge();
    //            return;
    //        }

    //        if (Vector3.Distance(transform.position, dodgeTarget) < 0.1f)
    //        {
    //            StopDodge();
    //            return;
    //        }
    //    }
    //}

    //void StopDodge()
    //{
    //    animator.SetBool("Dodge", false);
    //    isDodging = false;
    //}

    //private void AnimatorController() 
    //{
    //    float xVelocity = Vector3.Dot(movementDirection.normalized, transform.right);
    //    float zVelocity = Vector3.Dot(movementDirection.normalized, transform.forward);

    //    animator.SetFloat("xVelocity", xVelocity, 0.1f, Time.deltaTime);
    //    animator.SetFloat("zVelocity", zVelocity, 0.1f, Time.deltaTime);    

    //    bool playRunAnimation = isRunning && movementDirection.magnitude > 0;
    //    animator.SetBool("isRunning", playRunAnimation);
    //}

    //private void ApplyRotation()
    //{
    //    Vector3 lookingDirection; 
    //    lookingDirection = player.aim.GetMouseHitInfo().point - transform.position;
    //    lookingDirection.y = 0f; // Keep the direction horizontal
    //    lookingDirection.Normalize(); // Normalize to get direction only
    //    //transform.forward = lookingDirection; // Rotate the player to face the aim direction

    //    Quaternion desireRotation = Quaternion.LookRotation(lookingDirection);
    //    transform.rotation = Quaternion.Lerp(transform.rotation, desireRotation, turnSpeed * Time.deltaTime);
    //}

    //private void ApplyMovement()
    //{
    //    movementDirection = new Vector3(moveInput.x, 0, moveInput.y);
    //    ApplyGravity();
    //    if (movementDirection.magnitude > 0)
    //    {
    //        characterController.Move(movementDirection * Time.deltaTime * speed);
    //    }
    //}

    //private void ApplyGravity()
    //{
    //    if(characterController.isGrounded == false)
    //    {
    //        verticalVelocity -= gravityScale * Time.deltaTime; // Simple gravity
    //        movementDirection.y = verticalVelocity; // Apply vertical velocity to movement input
    //    }
    //    else
    //    {
    //        verticalVelocity = -0.5f;
    //    }
    //}

    //private void Dodge()
    //{
    //    if (isDodging == false)
    //    {
    //        animator.SetBool("Dodge", true);
    //        isDodging = true;
    //        dodgeTarget = transform.position + transform.forward * dodgeDistance;
    //    }
    //    else 
    //    {
    //        return;
    //    }
    //}

    //private void AssignInputEvents()
    //{
    //    controls = player.controls;

    //    controls.Character.Movement.performed += context => moveInput = context.ReadValue<Vector2>();
    //    controls.Character.Dodge.performed += context => Dodge();
    //    controls.Character.Movement.canceled += context => moveInput = Vector2.zero;
    //    controls.Character.Run.performed += context =>
    //    {
    //        speed = runSpeed;
    //        isRunning = true;
    //    };
    //    controls.Character.Run.canceled += context =>
    //    {
    //        speed = walkSpeed;
    //        isRunning = false;
    //    };
    //}
}