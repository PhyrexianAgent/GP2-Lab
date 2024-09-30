using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrollingToPoint : BaseState
{
    public bool ReturnToHQ {get{
        return index >= points.Length;
    }}
    private Transform[] points;
    private Transform hqPoint;
    private int index = 0;
    private float patrolSpeed;
    public PatrollingToPoint(NavMeshAgent agent, Transform[] points, Transform hqPoint, float patrolSpeed) : base(agent){
        this.points = points;
        this.hqPoint = hqPoint;
        this.patrolSpeed = patrolSpeed;
    }

    public void AddIndex(){
        index++;
        // Add transition for when index too high
    }

    public override void OnEnter(){
        agent.speed = patrolSpeed;
        if (ReturnToHQ)
            index = 0;
        if (!ReturnToHQ)
            agent.SetDestination(points[index].position);
    }
}
