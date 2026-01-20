using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    protected int healthPoints = 20;

    [Header("Idle data")]
    public float idleTime;
    public float aggressionRange;

    [Header("Move data")]
    public float moveSpeed;
    public float chaseSpeed;
    private bool manualMovement;
    public float turnSpeed;
    private bool manualRotation;

    [SerializeField] private Transform[] patrolPoints;
    private Vector3[] patrolPointsPosition;
    private int currentPatrolIndex;

    public bool inBattleMode { get; private set; }

    public Transform player { get; private set; }
    public Animator anim { get; private set; }
    public NavMeshAgent agent { get; private set; }
    public EnemyStateMachine stateMachine { get; private set; }

    protected virtual void Awake()
    {
        stateMachine = new EnemyStateMachine();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    protected virtual void Start()
    {
        InitializePatrolPoints();
    }

    protected virtual void Update()
    {

    }

    protected bool ShouldEnterBattleMode() 
    {
        bool inAgresstionRange = Vector3.Distance(transform.position, player.position) < aggressionRange;
        if (inAgresstionRange && !inBattleMode)
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

    public virtual void GetHit() 
    {
        EnterBattleMode();
        healthPoints--;
    }

    public virtual void DeathImpact(Vector3 force, Vector3 hitPoint, Rigidbody rb) 
    {
        StartCoroutine(DeathImpactCoroutine(force, hitPoint, rb));
    }

    private IEnumerator DeathImpactCoroutine(Vector3 force, Vector3 hitPoint, Rigidbody rb) 
    {
        yield return new WaitForSeconds(0.1f);
        rb.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
    }
    
    public void FaceTarget(Vector3 target) 
    {
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);
        Vector3 currentEulerAngles = transform.rotation.eulerAngles;

        float yRotation = Mathf.LerpAngle(currentEulerAngles.y, targetRotation.eulerAngles.y, turnSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Euler(currentEulerAngles.x, yRotation, currentEulerAngles.z);
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, aggressionRange);
    }

}
