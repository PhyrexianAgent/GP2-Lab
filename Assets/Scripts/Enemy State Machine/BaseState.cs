using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseState : IState
{
    public static Transform player;
    protected static readonly int Walk; // Ignore until animated enemies are implemented and add one per animation implemented with these states
    protected readonly Animator anim; // Ignore until animated enemies are implemented
    protected readonly NavMeshAgent agent;

    protected BaseState(NavMeshAgent agent){ // Add Animator perameter here later
        this.agent = agent;
    }

    public virtual void OnEnter(){

    }
    public virtual void Update(){

    }
    public virtual void FixedUpdate(){

    }
    public virtual void OnExit(){

    }
}
