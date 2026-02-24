using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using static UnityEngine.EventSystems.EventTrigger;

public class PlayerMovement : MonoBehaviour
{
    private Player player;

    private PlayerControls controls;
    private CharacterController characterController;
    private Animator animator;

    [Header("Movement Info")]
    private float speed;
    private float verticalVelocity = 0f;

    [SerializeField] private float walkSpeed = 1.5f;
    [SerializeField] private float runSpeed = 3.0f;
    [SerializeField] private float turnSpeed = 7f;
    [SerializeField] private float dodgeSpeed;
    [SerializeField] private float dodgeDistance;

    private Vector3 movementDirection;
    private Vector3 dodgeTarget;
    private Vector3 dodgeDirection;
    public Vector2 moveInput { get; private set; }

    private bool isRunning;
    public bool isDodging;

    [SerializeField] private float gravityScale = 9.81f;


    private void Start()
    {   
        player = GetComponent<Player>();

        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        speed = walkSpeed;

        AssignInputEvents();
    }

    private void Update()
    {
        if(player.health.isDead)
        {
            return;
        }
        if (isDodging == false) 
        {
            ApplyMovement();
            ApplyRotation();
            AnimatorController();
        }
        else if (isDodging)   
        {

            dodgeDirection = (dodgeTarget - transform.position).normalized;

            CollisionFlags flags = characterController.Move(
                dodgeDirection * dodgeSpeed * Time.deltaTime
            );

            if ((flags & CollisionFlags.Sides) != 0)
            {
                StopDodge();
                return;
            }

            if (Vector3.Distance(transform.position, dodgeTarget) < 0.1f)
            {
                StopDodge();
                return;
            }
        }
    }

    void StopDodge()
    {
        animator.SetBool("Dodge", false);
        isDodging = false;
    }

    private void AnimatorController() 
    {
        float xVelocity = Vector3.Dot(movementDirection.normalized, transform.right);
        float zVelocity = Vector3.Dot(movementDirection.normalized, transform.forward);

        animator.SetFloat("xVelocity", xVelocity, 0.1f, Time.deltaTime);
        animator.SetFloat("zVelocity", zVelocity, 0.1f, Time.deltaTime);    

        bool playRunAnimation = isRunning && movementDirection.magnitude > 0;
        animator.SetBool("isRunning", playRunAnimation);
    }

    private void ApplyRotation()
    {
        Vector3 lookingDirection; 
        lookingDirection = player.aim.GetMouseHitInfo().point - transform.position;
        lookingDirection.y = 0f; // Keep the direction horizontal
        lookingDirection.Normalize(); // Normalize to get direction only
        //transform.forward = lookingDirection; // Rotate the player to face the aim direction

        Quaternion desireRotation = Quaternion.LookRotation(lookingDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, desireRotation, turnSpeed * Time.deltaTime);
    }

    private void ApplyMovement()
    {
        movementDirection = new Vector3(moveInput.x, 0, moveInput.y);
        ApplyGravity();
        if (movementDirection.magnitude > 0)
        {
            characterController.Move(movementDirection * Time.deltaTime * speed);
        }
    }

    private void ApplyGravity()
    {
        if(characterController.isGrounded == false)
        {
            verticalVelocity -= gravityScale * Time.deltaTime; // Simple gravity
            movementDirection.y = verticalVelocity; // Apply vertical velocity to movement input
        }
        else
        {
            verticalVelocity = -0.5f;
        }
    }

    private void Dodge()
    {
        if (isDodging == false)
        {
            animator.SetBool("Dodge", true);
            isDodging = true;
            dodgeTarget = transform.position + transform.forward * dodgeDistance;
        }
        else 
        {
            return;
        }
    }

    private void AssignInputEvents()
    {
        controls = player.controls;

        controls.Character.Movement.performed += context => moveInput = context.ReadValue<Vector2>();
        controls.Character.Dodge.performed += context => Dodge();
        controls.Character.Movement.canceled += context => moveInput = Vector2.zero;
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
}