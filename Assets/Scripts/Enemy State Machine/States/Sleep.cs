using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sleep : BaseState
{
    public bool DoneSleeping {get{return doneSleeping;}}
    private bool doneSleeping = false;
    private MonoBehaviour mono;
    private float sleepDuration;
    private Coroutine sleepCoroutine;

    public Sleep(NavMeshAgent agent, MonoBehaviour mono, float sleepDuration) : base(agent){
        this.mono = mono;
        this.sleepDuration = sleepDuration;
    }

    private IEnumerator SleepWait(){
        yield return new WaitForSeconds(sleepDuration);
        doneSleeping = true;
    }

    public override void OnEnter(){
        doneSleeping = false;
        mono.StartCoroutine(SleepWait());
    }

    public override void OnExit(){
        if (sleepCoroutine != null)
            mono.StopCoroutine(sleepCoroutine);
    }
}
