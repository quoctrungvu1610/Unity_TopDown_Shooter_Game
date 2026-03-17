using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
    [Header("Idle data")]
    public float idleTime;

    [Header("Move data")]
    public float walkSpeed = 1.5f;
    public float runSpeed = 3f;
    public float turnSpeed;

    [SerializeField] private Transform[] patrolPoints;
    private Vector3[] patrolPointsPosition;
    private int currentPatrolIndex;

    private bool isDead = false;

    public Animator anim { get; private set; }
    public NavMeshAgent agent { get; private set; }
    public NPCStateMachine stateMachine { get; private set; }

    public IdleState_NPC idleState { get; private set; }
    public MoveState_NPC moveState { get; private set; }


    protected virtual void Awake()
    {
        stateMachine = new NPCStateMachine();

        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        idleState = new IdleState_NPC(this, stateMachine, "Idle");
        moveState = new MoveState_NPC(this, stateMachine, "Move");
    }

    protected virtual void Start()
    {
        InitializePatrolPoints();
        stateMachine.Initialize(idleState);
    }

    protected virtual void Update()
    {
        stateMachine.currentState.Update();

    }

    public Vector3 GetPatrolDestination()
    {
        Vector3 destination = patrolPointsPosition[currentPatrolIndex];
        currentPatrolIndex++;
        if (currentPatrolIndex >= patrolPoints.Length)
        {
            currentPatrolIndex = 0;
        }

        return destination;
    }

    private void InitializePatrolPoints()
    {
        Debug.Log("Initializing patrol points");
        patrolPointsPosition = new Vector3[patrolPoints.Length];
        for (int i = 0; i < patrolPoints.Length; i++)
        {
            patrolPointsPosition[i] = patrolPoints[i].position;
            patrolPoints[i].gameObject.SetActive(false);
        }

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
}
