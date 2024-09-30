using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LookingAround : BaseState
{
    public bool DoneLooking {get{
        return doneLooking;
    }}
    private Coroutine lookCoroutine;
    private float lookDuration;
    private bool doneLooking = false;
    private MonoBehaviour mono; // Need this to call Coroutine from this class, otherwise makes things way too complicated

    public LookingAround(NavMeshAgent agent, MonoBehaviour mono, float lookDuration) : base(agent){
        this.lookDuration = lookDuration;
        this.mono = mono;
    }

    private IEnumerator LookAround(){
        yield return new WaitForSeconds(lookDuration);
        doneLooking = true;
    }

    public override void OnEnter(){
        doneLooking = false;
        lookCoroutine = mono.StartCoroutine(LookAround());
    }

    public override void OnExit(){
        if (lookCoroutine != null)
            mono.StopCoroutine(lookCoroutine);
    }
}
