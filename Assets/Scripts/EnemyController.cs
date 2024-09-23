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
        SLEEP,
        SLEEP_START
    }
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float lookTime = 1f;
    [SerializeField] private float initialLookTime = 3f;
    [SerializeField, Range(0, 1)] private float minimumDot = 0.1f;
    [SerializeField] private float chaseDist = 6f;
    [SerializeField] private float attackDist = 4f;
    [SerializeField] private float patrolStoppingDistance = 1f;
    [SerializeField] private float attackDuration = 2f;

    private NavMeshAgent nav;
    private State state = State.SLEEP_START;
    private int pointIndex = 0;
    private bool doneSleeping = false;
    private Coroutine lookCoroutine;
    private Coroutine attackCoroutine;
    private bool playerWasHit = false;

    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        StartCoroutine(Sleep(initialLookTime));
    }

    float MinValue(float value, float min){
        if (value > 0 && value < min)
            value = -1;
        return value;
    }

    void AddPointIndex(){
        pointIndex += 1;
        if (pointIndex >= patrolPoints.Length)
            pointIndex = 0;
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

    void StateActions(){
        switch(state){
            case State.PATROLLING_TO_POINT:
                if (PlayerInViewAngle()){
                    if (InChaseDistance()){
                        if (InAttackDistance()){
                            if (lookCoroutine != null)
                                StopCoroutine(lookCoroutine);
                            if (InAttackDistance()){
                                state = State.ATTACKING;
                                attackCoroutine = StartCoroutine(Attack());
                                break;
                            }
                            state = State.CHASE;
                            break;
                        }
                    }
                }
                if (nav.remainingDistance == 0){
                    state = State.LOOKING_AROUND;
                    lookCoroutine = StartCoroutine(Sleep(lookTime));
                }
                break;
            case State.LOOKING_AROUND:
                if (doneSleeping){
                    AddPointIndex();
                    state = State.PATROLLING_TO_POINT;
                    nav.destination = patrolPoints[pointIndex].position;
                }
                break;
            case State.SLEEP_START:
                if (doneSleeping){
                    state = State.PATROLLING_TO_POINT;
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
                if (playerWasHit)
                    Debug.Log("player hit");
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
