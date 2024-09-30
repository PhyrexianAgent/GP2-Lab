using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyStateController : MonoBehaviour
{
    private static Transform player = null;
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private Transform hqPoint;
    [SerializeField] private float lookTime = 1f;
    [SerializeField] private float sleepTime = 3f;
    [SerializeField, Range(0, 1)] private float minimumDot = 0.73f;
    [SerializeField] private float chaseDist = 9f;
    [SerializeField] private float attackDist = 3.5f;
    [SerializeField] private float patrolStoppingDistance = 1f;
    [SerializeField] private float attackDuration = 2f;
    [SerializeField] private bool canChase = true;
    [SerializeField] private float chaseSpeed = 8f;

    private NavMeshAgent agent;
    private StateMachine stateMachine;

    void Awake(){
        if (player == null)
            player = GameObject.FindWithTag("Player").transform;
    }
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        stateMachine = new StateMachine();

        PatrollingToPoint patrolState = new PatrollingToPoint(agent, patrolPoints, hqPoint, agent.speed);
        LookingAround lookingState = new LookingAround(agent, this, lookTime);
        Sleep sleepState = new Sleep(agent, this, sleepTime);
        ReturningToHQ returnState = new ReturningToHQ(agent, hqPoint, agent.speed);

        stateMachine.AddTransition(patrolState, lookingState, new FuncPredicate(() => agent.remainingDistance == 0));
        stateMachine.AddTransition(lookingState, returnState, new FuncPredicate(() => patrolState.ReturnToHQ));
        stateMachine.AddTransition(lookingState, patrolState, new FuncPredicate(() => {
            if (lookingState.DoneLooking)
                patrolState.AddIndex();
            return lookingState.DoneLooking && !patrolState.ReturnToHQ;
        }));
        stateMachine.AddTransition(sleepState, patrolState, new FuncPredicate(() => sleepState.DoneSleeping));
        stateMachine.AddTransition(returnState, sleepState, new FuncPredicate(() => agent.remainingDistance == 0));

        stateMachine.SetState(sleepState);
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
    }

    void FixedUpdate(){
        stateMachine.FixedUpdate();
    }
}
