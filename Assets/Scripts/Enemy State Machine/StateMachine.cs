using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StateMachine
{
    class StateNode{
        public IState State {get;}
        public HashSet<ITransition> Transitions {get;}

        public StateNode(IState state){
            State = state;
            Transitions = new HashSet<ITransition>();
        }

        public void AddTransition(IState to, IPredicate condition){
            Transitions.Add(new Transition(to, condition));
        }
    }

    private StateNode currentState;
    private Dictionary<Type, StateNode> nodes = new Dictionary<Type, StateNode>();

    private StateNode GetOrAddNode(IState state){
        StateNode node = nodes.GetValueOrDefault(state.GetType());
        if (node == null){
            node = new StateNode(state);
            nodes.Add(state.GetType(), node);
        }
        return node;
    }

    public void AddTransition(IState from, IState to, IPredicate condition){
        GetOrAddNode(from).AddTransition(to, condition);
    }

    private ITransition GetTransition(){
        foreach (ITransition transition in currentState.Transitions){
            if (transition.Condition.Evaluate())
                return transition;
        }
        return null;
    }

    public void SetState(IState state){
        currentState = nodes[state.GetType()];
        currentState.State?.OnEnter();
    }

    public void ChangeState(IState state){
        if (currentState.State == state)
            return;

        IState previousState = currentState.State;
        IState nextState = nodes[state.GetType()].State;

        previousState?.OnExit();
        nextState?.OnEnter();

        currentState = nodes[state.GetType()];
    }

    public void Update(){
        ITransition transition = GetTransition();
        if (transition != null)
            ChangeState(transition.To);
        currentState.State?.Update();
    }

    public void FixedUpdate(){
        currentState.State?.FixedUpdate();
    }
}
