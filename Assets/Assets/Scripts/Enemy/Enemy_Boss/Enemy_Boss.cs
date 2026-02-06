using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public enum BossWeaponType 
{
    Flamethrower,
    Hammer
}
public class Enemy_Boss : Enemy
{
    [Header("Boss Details")]
    public BossWeaponType bossWeaponType;
    public float actionCooldown = 10;
    public float attackRange;

    [Header("Ability")]
    public float minAbilityDistance;
    public float abilityCooldown;
    private float lastTimeUsedAbility;

    [Header("Flamethrower")]
    public float flamethrowDuration;
    public ParticleSystem flamethrower;
    public bool flamethrowActive { get; private set; }

    [Header("Hammer")]
    public GameObject activationPrefab;

    [Header("Jump Attack")]
    public float jumpAttackCooldown = 10;
    private float lastTimeJumped;
    public float travelTimeToTarget = 1;
    public float minJumpDistanceRequired;
    [Space]
    public float impactRadius = 2.5f;
    public float impactPower = 10;
    public Transform impactPoint;
    [SerializeField] private float upforceMultiplier = 1;

    [Space]
    [SerializeField] private LayerMask whatToIgnore;

    [Header("Attack")]
    [SerializeField] private Transform[] damagePoints;
    [SerializeField] private float attackCheckRadius;
    [SerializeField] private GameObject meleeAttackFx;

    public IdleState_Boss idleState { get; private set; }
    public MoveState_Boss moveState { get; private set; }
    public AttackState_Boss attackState { get; private set; }
    public JumpAttackState_Boss jumpAttackState { get; private set; }
    public AbilityState_Boss abilityState { get; private set; }
    public DeadState_Boss deadState { get; private set; }
    public Enemy_BossVisual bossVisual { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        bossVisual = GetComponent<Enemy_BossVisual>();

        idleState = new IdleState_Boss(this, stateMachine, "Idle");
        moveState = new MoveState_Boss(this, stateMachine, "Move");
        attackState = new AttackState_Boss(this, stateMachine, "Attack");
        jumpAttackState = new JumpAttackState_Boss(this, stateMachine, "JumpAttack");
        abilityState = new AbilityState_Boss(this, stateMachine, "Ability");
        deadState = new DeadState_Boss(this, stateMachine, "Idle");
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();

        stateMachine.currentState.Update();

        if (ShouldEnterBattleMode()) 
        {
            EnterBattleMode();
        }

        MeleeAttackCheck(damagePoints, attackCheckRadius, meleeAttackFx);
    }

  
    public override void Die()
    {
        base.Die();

        if (stateMachine.currentState != deadState)
        {
            stateMachine.ChangeState(deadState);
        }
    }

    public override void EnterBattleMode()
    {
        if(inBattleMode) 
        {
            return;
        }
        base.EnterBattleMode();
        stateMachine.ChangeState(moveState);
    }

    public void ActivateFlamethrower(bool activate) 
    {
        flamethrowActive = activate;
        if (!activate) 
        {
            flamethrower.Stop();
            anim.SetTrigger("StopFlamethrower");
            
            return;
        }
        var mainModule = flamethrower.main;
        var extraModule_1 = flamethrower.transform.GetChild(0).GetComponent<ParticleSystem>().main;
        var extraModule_2 = flamethrower.transform.GetChild(1).GetComponent<ParticleSystem>().main;

        mainModule.duration = flamethrowDuration;
        extraModule_1.duration = flamethrowDuration;
        extraModule_2.duration = flamethrowDuration;


        flamethrower.Clear();
        flamethrower.Play();
    }

    public void ActivateHammer() 
    {
        GameObject newActivation = ObjectPool.instance.GetObject(activationPrefab, impactPoint);

        ObjectPool.instance.ReturnObject(newActivation, 1);
    }

    public bool CanDoAbility() 
    {
        bool playerWithinDistance = Vector3.Distance(transform.position, player.position) < minAbilityDistance;

        if (playerWithinDistance == false)
        {
            return false;
        }

        if (Time.time > lastTimeUsedAbility + abilityCooldown && playerWithinDistance) 
        {
            return true;
        }
        return false;
    }

    public void SetAbilityOnCooldown() 
    {
        Debug.Log("Set Cool Down Ability");
        lastTimeUsedAbility = Time.time;
    }

    public void JumpImpact() 
    {
        Transform impactPoint = this.impactPoint;

        if (impactPoint == null) 
        {
            impactPoint = transform;
        }
        Collider[] colliders = Physics.OverlapSphere(impactPoint.position, impactRadius);

        foreach (Collider hit in colliders)
        {

            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddExplosionForce(impactPower, transform.position, impactRadius, upforceMultiplier, ForceMode.Impulse);
            }
        }
    }

    public bool CanDoJumpAttack() 
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer < minJumpDistanceRequired) 
        {
            return false;
        }

        if (Time.time > lastTimeJumped + jumpAttackCooldown && IsPlayerInClearSight())      
        {
            
            return true;
        }
        return false;
    }

    public void SetJumpAttackOnCooldown() 
    {
        Debug.Log("Set Cool Down Jump");
        lastTimeJumped = Time.time;
    }

    public bool IsPlayerInClearSight()
    {
        Vector3 myPos = transform.position + new Vector3(0, 1.5f, 0);
        Vector3 playerPos = player.position + Vector3.up;

        Vector3 directionToPlayer = (playerPos - myPos).normalized;

        if (Physics.Raycast(myPos, directionToPlayer, out RaycastHit hit, 100, ~whatToIgnore))
        {
            if (hit.transform == player || hit.transform.parent == player) 
            {
                return true;
            }
        }

        return false;
    }

    public bool PlayerInAttackRange()
    {
        return Vector3.Distance(transform.position, player.position) < attackRange;
    }

    protected  override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (player != null) 
        {
            Vector3 myPos = transform.position + new Vector3(0, 1.5f, 0);
            Vector3 playerPos = player.position + Vector3.up;

            Gizmos.color = Color.green;

            Gizmos.DrawLine(myPos, playerPos);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minJumpDistanceRequired);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, impactRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, minAbilityDistance);

        if (damagePoints.Length > 0) 
        {
            foreach (var damagePoint in damagePoints) 
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(damagePoint.position, attackCheckRadius);
            }
        }

    }
}
