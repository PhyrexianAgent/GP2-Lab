using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public static Transform player;

    private enum State{
        PATROLLING_TO_POINT,
        LOOKING_AROUND,
        CHASE,
        ATTACKING,
        INVESTIGATING,
        RETURN_TO_HQ,
        SLEEP
    }
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private Transform hqPoint;
    [SerializeField] private float lookTime = 2f;
    [SerializeField] private float sleepTime = 3f;
    [SerializeField, Range(0, 1)] private float minimumDot = 0.1f;
    [SerializeField] private float chaseDist = 6f;
    [SerializeField] private float attackDist = 4f;
    [SerializeField] private float patrolStoppingDistance = 1f;
    [SerializeField] private float attackDuration = 2f;
    [SerializeField] private bool canChase = true;
    [SerializeField] private float chaseSpeed = 8f;

    private NavMeshAgent nav;
    private State state = State.SLEEP;
    private int pointIndex = 0;
    private bool doneSleeping = false;
    private Coroutine lookCoroutine;
    private Coroutine attackCoroutine;
    private bool playerWasHit = false;
    private float patrolSpeed;

    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        patrolSpeed = nav.speed;
        StartCoroutine(Sleep(sleepTime));
    }

    float MinValue(float value, float min){
        if (value > 0 && value < min)
            value = -1;
        return value;
    }

    bool PlayerInViewAngle(){
        Vector3 diff = player.position - transform.position;
        diff.Normalize();
        return MinValue(Vector3.Dot(transform.forward, diff), minimumDot) > 0;
    }

    bool InChaseDistance(){
        return Vector3.Distance(transform.position, player.position) <= chaseDist;
    }

    bool InAttackDistance(){
        return Vector3.Distance(transform.position, player.position) <= attackDist;
    }

    IEnumerator Sleep(float seconds){
        doneSleeping = false;
        yield return new WaitForSeconds(seconds);
        doneSleeping = true;
    }

    IEnumerator Attack(){
        yield return new WaitForSeconds(attackDuration);
        playerWasHit = true;
    }

    bool DecideIfChasingAndAttacking(){
        if (PlayerInViewAngle() && canChase){
            if (InAttackDistance()){
                if (lookCoroutine != null)
                    StopCoroutine(lookCoroutine);
                state = State.ATTACKING;
                attackCoroutine = StartCoroutine(Attack());
                nav.speed = chaseSpeed;
                return true;
            }
            else if (InChaseDistance()){
                if (lookCoroutine != null)
                    StopCoroutine(lookCoroutine);
                state = State.CHASE;
                nav.speed = chaseSpeed;
                return true;
            }
        }
        return false;
    }

    void StateActions(){
        switch(state){
            case State.PATROLLING_TO_POINT:
                if (DecideIfChasingAndAttacking()){
                    break;
                }
                if (nav.remainingDistance == 0){
                    state = State.LOOKING_AROUND;
                    lookCoroutine = StartCoroutine(Sleep(lookTime));
                }
                break;
            case State.LOOKING_AROUND:
                if (DecideIfChasingAndAttacking()){
                    break;
                }
                if (doneSleeping){
                    pointIndex += 1;
                    if (pointIndex >= patrolPoints.Length){
                        state = State.RETURN_TO_HQ;
                        nav.destination = hqPoint.position;
                        break;
                    }
                    state = State.PATROLLING_TO_POINT;
                    nav.destination = patrolPoints[pointIndex].position;
                }
                break;
            case State.SLEEP:
                if (doneSleeping){
                    state = State.PATROLLING_TO_POINT;
                    pointIndex = 0;
                    nav.destination = patrolPoints[pointIndex].position;
                }
                break;
            case State.CHASE:
                nav.destination = player.position;
                if (InAttackDistance()){
                    state = State.ATTACKING;
                    attackCoroutine = StartCoroutine(Attack());
                }
                else if (!InChaseDistance()){
                    state = State.PATROLLING_TO_POINT;
                    nav.speed = patrolSpeed;
                    nav.destination = patrolPoints[pointIndex].position;
                }
                break;
            case State.ATTACKING:
                nav.destination = player.position;
                if (!InAttackDistance() && InChaseDistance()){
                    if (attackCoroutine != null)
                        StopCoroutine(attackCoroutine);
                    state = State.CHASE;
                }
                if (playerWasHit){
                    if (attackCoroutine != null)
                        StopCoroutine(attackCoroutine);
                    state = State.RETURN_TO_HQ;
                    nav.speed = patrolSpeed;
                    nav.destination = hqPoint.position;
                }
                break;
            case State.RETURN_TO_HQ:
                if (nav.remainingDistance == 0){
                    StartCoroutine(Sleep(sleepTime));
                    state = State.SLEEP;
                }
                break;
        }
    }
    void Update()
    {
        StateActions();
        Debug.DrawLine(transform.position, transform.position + transform.forward * chaseDist, Color.blue, 0);
        Debug.DrawLine(transform.position, transform.position + transform.forward * attackDist, Color.red, 0);
    }
}
