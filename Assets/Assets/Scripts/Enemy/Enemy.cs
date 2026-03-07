using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    public LayerMask whatIsAlly;
    public LayerMask whatIsPlayer;
    [Space]
    //public int healthPoints = 20;

    [Header("Idle data")]
    public float idleTime;
    public float aggressionRange;

    [Header("Move data")]
    public float walkSpeed = 1.5f;
    public float runSpeed = 3f;
    private bool manualMovement;
    public float turnSpeed;
    private bool manualRotation;

    private bool isDead = false;

    [SerializeField] private Transform[] patrolPoints;
    private Vector3[] patrolPointsPosition;
    private int currentPatrolIndex;

    private int meleeDamage;

    public bool inBattleMode { get; private set; }
    protected bool isMeleeAttackReady;

    public Transform player { get; private set; }
    public Animator anim { get; private set; }
    public NavMeshAgent agent { get; private set; }
    public EnemyStateMachine stateMachine { get; private set; }

    public Enemy_Visual visuals { get; private set; }

    public Ragdoll ragdoll { get; private set; }
    public Enemy_Health health { get; private set; }

    protected virtual void Awake()
    {
        ragdoll = GetComponent<Ragdoll>();
        health = GetComponent<Enemy_Health>();
        stateMachine = new EnemyStateMachine();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        visuals = GetComponent<Enemy_Visual>();
    }

    protected virtual void Start()
    {
        InitializePatrolPoints();
    }

    protected virtual void Update()
    {
        if (ShouldEnterBattleMode())
        {
            EnterBattleMode();
        }

    }

    protected virtual void InitializePerk() 
    {
    
    }

    protected bool ShouldEnterBattleMode() 
    {
       
        if (IsPlayerInAgrresionRange() && !inBattleMode)
        {
            EnterBattleMode();
            return true;
        }
        return false;
    }

    public virtual void EnterBattleMode() 
    {
        inBattleMode = true;
    }

    #region Patrol Logic

    public Vector3 GetPatrolDestination() 
    {
        Vector3 destination = patrolPointsPosition[currentPatrolIndex];
        currentPatrolIndex++;
        if(currentPatrolIndex >= patrolPoints.Length) 
        {
            currentPatrolIndex = 0;
        }

        return destination;
    }

    private void InitializePatrolPoints()
    {
        patrolPointsPosition = new Vector3[patrolPoints.Length];
        for (int i = 0; i < patrolPoints.Length; i++) 
        {
            patrolPointsPosition[i] = patrolPoints[i].position;
            patrolPoints[i].gameObject.SetActive(false);
        }

    }

    #endregion

    #region Animation Events

    public void AnimationTrigger() 
    {
        stateMachine.currentState.AnimationTrigger();
    }

    public void ActivateManualMovement(bool manualMovement) 
    {
        this.manualMovement = manualMovement;   
    }

    public bool ManualMovementActive() 
    {
        return manualMovement;
    }

    public virtual void AbilityTrigger() 
    {
        stateMachine.currentState.AbilityTrigger();
    }

    public void ActivateManualRotation(bool manualRotation)
    {
        this.manualRotation = manualRotation;
    }

    public bool ManualRotationActive()
    {
        return manualRotation;
    }

    #endregion

    public virtual void GetHit(int damage) 
    {
        health.ReduceHealth(damage);
        if(health.ShouldDie()) 
        {
            Die();
            return;
        }
        EnterBattleMode();
    }

    public virtual void Die() 
    {
        isDead = true;
    }

    public virtual void MeleeAttackCheck(Transform[] damagePoints, float attackCheckRadius, GameObject fx )
    {
        if (isMeleeAttackReady == false)
        {
            return;
        }

        foreach (Transform attackPoint in damagePoints)
        {

            Collider[] detectedHits = Physics.OverlapSphere(attackPoint.position, attackCheckRadius, whatIsPlayer);

            for (int i = 0; i < detectedHits.Length; i++)
            {
                IDamageable damageable = detectedHits[i].GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(meleeDamage);
                    isMeleeAttackReady = false;
                    GameObject newAttackFx = ObjectPool.instance.GetObject(fx, attackPoint);

                    ObjectPool.instance.ReturnObject(newAttackFx, 1f);
                    return;
                }
            }
        }
    }

    public void EnableMeleeAttackCheck(bool enable)
    {
        isMeleeAttackReady = enable;
    }

    public virtual void BulletImpact(Vector3 force, Vector3 hitPoint, Rigidbody rb) 
    {
        StartCoroutine(DeathImpactCoroutine(force, hitPoint, rb));
    }

    private IEnumerator DeathImpactCoroutine(Vector3 force, Vector3 hitPoint, Rigidbody rb) 
    {
        yield return new WaitForSeconds(0.1f);
        if(rb != null)
            rb.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
    }
    
    public void FaceTarget(Vector3 target, float turnSpeed = 0) 
    {
        if (isDead)
            return;
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);
        Vector3 currentEulerAngles = transform.rotation.eulerAngles;

        if (turnSpeed == 0) 
        {
            turnSpeed = this.turnSpeed;
        }

        float yRotation = Mathf.LerpAngle(currentEulerAngles.y, targetRotation.eulerAngles.y, turnSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Euler(currentEulerAngles.x, yRotation, currentEulerAngles.z);
    }

    public bool IsPlayerInAgrresionRange() 
    {
        return Vector3.Distance(transform.position, player.position) < aggressionRange;
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, aggressionRange);
    }

}
