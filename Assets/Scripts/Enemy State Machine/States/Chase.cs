using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Chase : BaseState
{
    public bool WasHit {get{return hit;}}
    private float attackDistance;
    private MonoBehaviour mono;
    private Coroutine attackCoroutine;
    private Transform player;
    private float chaseSpeed;
    private bool hit = false;
    private Transform transform;
    private float attackDuration;

    public Chase(NavMeshAgent agent, MonoBehaviour mono, Transform transform, Transform player, float attackDistance, float chaseSpeed, float attackDuration) : base(agent){
        this.mono = mono;
        this.attackDistance = attackDistance;
        this.chaseSpeed = chaseSpeed;
        this.transform = transform;
        this.attackDuration = attackDuration;
        this.player = player;
    }

    private IEnumerator Attack(){
        yield return new WaitForSeconds(attackDuration);
        hit = InAttackDistance();
    }

    private bool InAttackDistance() => Vector3.Distance(transform.position, player.position) <= attackDistance;

    public override void OnEnter(){
        agent.destination = player.position;
        agent.speed = chaseSpeed;
    }

    public override void Update(){
        //agent.destination = player.position;
        agent.SetDestination(player.position);
        if (InAttackDistance() && attackCoroutine == null){
            attackCoroutine = mono.StartCoroutine(Attack());
        }
        else if (attackCoroutine != null && !InAttackDistance()){
            mono.StopCoroutine(attackCoroutine);
        }
    }

    public override void OnExit(){
        hit = false;
        if (attackCoroutine != null){
            mono.StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
    }
}
