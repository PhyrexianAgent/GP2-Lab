using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ReturningToHQ : BaseState
{
    private Transform hqPoint;
    private float returningSpeed;

    public ReturningToHQ(NavMeshAgent agent, Transform hqPoint, float returningSpeed) : base(agent){
        this.hqPoint = hqPoint;
        this.returningSpeed = returningSpeed;
    }

    public override void OnEnter(){
        Debug.Log("returning");
        agent.speed = returningSpeed;
        agent.destination = hqPoint.position;
    }
}
