using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AttackData_EnemyMelee 
{
    public string attackName;
    public float attackRange;
    public float moveSpeed;
    public float attackIndex;
    [Range(1, 3)]
    public float animationSpeed;
    public AttackType_Melee attackType;
}

public enum AttackType_Melee 
{
    Close,
    Charge
}

public enum EnemyMelee_Type 
{
    Regular,
    Shield,
    Dodge,
    AxeThrow,
}

public class Enemy_Melee : Enemy
{
    public IdleState_Melee idleState { get; private set; }
    public MoveState_Melee moveState { get; private set; }
    public RecoveryState_Melee recoveryState { get; private set; }
    public ChaseState_Melee chaseState { get; private set; }
    public AttackState_Melee attackState { get; private set; }
    public DeadState_Melee deadState { get; private set; }
    public AbilityState_Melee abilityState { get; private set; }

    [Header("Enemy Settings")]
    public EnemyMelee_Type meleeType;
    [SerializeField] private Transform shieldTranform;
    public Enemy_MeleeWeaponType weaponType;
    public float dodgeCooldown;
    private float lastTimeDodge = -10f;

    [Header("Axe Throw Ability")]
    public GameObject axePrefab;
    public float axeFlySpeed;
    public float axeAimTimer;
    public float axeThrowCooldown;
    public float lastTimeAxeThrown;
    public Transform axeStartPoint;

    [Header("Atack Data")]
    public AttackData_EnemyMelee attackData;
    public List<AttackData_EnemyMelee> attackList;

    protected override void Awake()
    {
        base.Awake();

        idleState = new IdleState_Melee(this, stateMachine, "Idle");
        moveState = new MoveState_Melee(this, stateMachine, "Move");
        recoveryState = new RecoveryState_Melee(this, stateMachine, "Recovery");
        chaseState = new ChaseState_Melee(this, stateMachine, "Chase");
        attackState = new AttackState_Melee(this, stateMachine, "Attack");
        deadState = new DeadState_Melee(this, stateMachine, "Idle");//idle is just a place holder here, we use ragdoll
        abilityState = new AbilityState_Melee(this, stateMachine, "AxeThrow");
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
        InitializePerk();
        visuals.SetupLook();
        UpdateAttackData();
    }

    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();
    }

    public override void EnterBattleMode()
    {
        if(inBattleMode) 
        {
            return;
        }
        base.EnterBattleMode();
        stateMachine.ChangeState(recoveryState);
    }


    public override void AbilityTrigger()
    {
        base.AbilityTrigger();

        walkSpeed = walkSpeed * 0.6f;
        visuals.EnableWeaponModel(false);
    }

    public void UpdateAttackData() 
    {
        Enemy_WeaponModel currentWeapon = visuals.currentWeaponModel.GetComponent<Enemy_WeaponModel>();
        if (currentWeapon.weaponData != null)
        {
            attackList = new List<AttackData_EnemyMelee>(currentWeapon.weaponData.attackData);
            turnSpeed =currentWeapon.weaponData.turnSpeed;
        }
    }

    protected override void InitializePerk() 
    {
        if (meleeType == EnemyMelee_Type.AxeThrow) 
        {
            weaponType = Enemy_MeleeWeaponType.Throw;
        }

        if(meleeType == EnemyMelee_Type.Shield) 
        {
            anim.SetFloat("ChaseIndex", 1);
            shieldTranform.gameObject.SetActive(true);
            weaponType = Enemy_MeleeWeaponType.OneHand;
        }
        else 
        {
            anim.SetFloat("ChaseIndex", 0);
            shieldTranform.gameObject.SetActive(false);
        }

        if(meleeType == EnemyMelee_Type.Dodge) 
        {
            weaponType = Enemy_MeleeWeaponType.Unarmed;
        }
    }

    public override void GetHit()
    {
        base.GetHit();
        if (healthPoints <= 0 && stateMachine.currentState != deadState) 
        {
            stateMachine.ChangeState(deadState);
        } 
    }

    public bool PlayerInAttackRange()
    {
        return Vector3.Distance(transform.position, player.position) < attackData.attackRange;
    }

    public void ActivateDodgeRoll()
    {

        if (meleeType != EnemyMelee_Type.Dodge) 
        {
            return;
        }

        if (stateMachine.currentState != chaseState) 
        {
            return;
        }

        if(Vector3.Distance(transform.position, player.position) < 2f) 
        {
            return;
        }

        float dodgeAnimationDuration = GetAnimationClipDuration("Dodge Roll(Not Read-Only)");

        if (Time.time > dodgeAnimationDuration + lastTimeDodge) 
        {
            lastTimeDodge = Time.time;
            anim.SetTrigger("Dodge");
        }
    }

    public bool CanThrowAxe() 
    {
        if(meleeType != EnemyMelee_Type.AxeThrow) 
        {
            return false;
        }

        if (Time.time > lastTimeAxeThrown + axeThrowCooldown) 
        {
            lastTimeAxeThrown = Time.time;
            return true;
        }
        return false;
    }

    public float GetAnimationClipDuration(string clipName) 
    {
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;

        foreach (AnimationClip clip in clips) 
        {
            if(clip.name == clipName) 
            {
                return clip.length;
            }
        }

        Debug.Log("No such animation clip found");
        return 0;
    } 

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackData.attackRange);
    }
}
