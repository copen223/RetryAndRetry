using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorState : MonoBehaviour
{
    public ActorController Controller;

    public virtual void StateStart() { }
    public virtual void StateUpdate() { }

    public virtual void StateExit() { }

    public void ChangeStateTo<T>()
    {
        foreach (ActorState actorState in Controller.ActorStates)
        {
            if (actorState is T)
            {
                Controller.currentState.StateExit();
                Controller.currentState = actorState;
                Controller.currentState.StateStart();
            }
        }
    }

    private void Start()
    {
        Controller = transform.GetComponent<ActorController>();
    }
}
