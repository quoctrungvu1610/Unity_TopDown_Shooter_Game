using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
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

    private int currentPatrolIndex;

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
    public Vector3 GetPatrolDestination() 
    {
        Vector3 destination = patrolPoints[currentPatrolIndex].transform.position;
        currentPatrolIndex++;
        if(currentPatrolIndex >= patrolPoints.Length) 
        {
            currentPatrolIndex = 0;
        }

        return destination;
    }

    private void InitializePatrolPoints()
    {
        foreach (Transform t in patrolPoints)
        {
            t.parent = null;
        }
    }

    public void AnimationTrigger() 
    {
        stateMachine.currentState.AnimationTrigger();
    }

    public bool PlayerInAggressionRange() 
    {
        return Vector3.Distance(transform.position, player.position) < aggressionRange;
    }

    public void ActivateManualMovement(bool manualMovement) 
    {
        this.manualMovement = manualMovement;   
    }

    public bool ManualMovementActive() 
    {
        return manualMovement;
    }

    public void ActivateManualRotation(bool manualRotation)
    {
        this.manualRotation = manualRotation;
    }

    public bool ManualRotationActive()
    {
        return manualRotation;
    }

    public virtual void GetHit() 
    {
       
    }

    public virtual void HitImpact(Vector3 force, Vector3 hitPoint, Rigidbody rb) 
    {
        StartCoroutine(HitImpactCoroutine(force, hitPoint, rb));
    }

    private IEnumerator HitImpactCoroutine(Vector3 force, Vector3 hitPoint, Rigidbody rb) 
    {
        yield return new WaitForSeconds(0.1f);
        rb.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, aggressionRange);
    }

    public Quaternion FaceTarget(Vector3 target) 
    {
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);
        Vector3 currentEulerAngles = transform.rotation.eulerAngles;

        float yRotation = Mathf.LerpAngle(currentEulerAngles.y, targetRotation.eulerAngles.y, turnSpeed * Time.deltaTime);

        return Quaternion.Euler(currentEulerAngles.x, yRotation, currentEulerAngles.z);
    }
}
