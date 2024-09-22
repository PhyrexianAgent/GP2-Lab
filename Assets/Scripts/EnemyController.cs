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
        INVESTIGATING,
        RETURN_TO_HQ,
        SLEEP,
        SLEEP_START
    }
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float lookTime = 1f;
    [SerializeField] private float initialLookTime = 3f;
    [SerializeField, Range(0, 1)] private float minimumDot = 0.1f;
    [SerializeField] private float chaseDist = 5f;
    [SerializeField] private float attackDist = 3f;
    [SerializeField] private float patrolStoppingDistance = 1f;

    private NavMeshAgent nav;
    private State state = State.SLEEP_START;
    private int pointIndex = 0;
    private bool doneSleeping = false;

    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        StartCoroutine(Sleep(initialLookTime));
        //nav.destination = patrolPoints[0].position;
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

    bool CanSeePlayer(){
        return PlayerInViewAngle() && Vector3.Distance(transform.position, player.position) <= chaseDist;
    }

    IEnumerator Sleep(float seconds){
        doneSleeping = false;
        yield return new WaitForSeconds(seconds);
        doneSleeping = true;
    }
    void StateActions(){
        switch(state){
            case State.PATROLLING_TO_POINT:
                /*if (CanSeePlayer()){
                    state = State.CHASE;
                    break;
                }*/
                if (nav.remainingDistance == 0){//Vector3.Distance(transform.position, patrolPoints[pointIndex].position) <= patrolStoppingDistance){
                    state = State.LOOKING_AROUND;
                    StartCoroutine(Sleep(lookTime));
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
        }
    }
    void Update()
    {
        StateActions();
    }
}
