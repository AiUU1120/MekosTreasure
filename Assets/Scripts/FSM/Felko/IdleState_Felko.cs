using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState_Felko : IState
{
    private FSM_Felko manager;
    private Parameter parameter;

    public IdleState_Felko(FSM_Felko manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        Debug.Log("Idle");
        parameter.rb.velocity = new Vector2(0f, parameter.rb.velocity.y);
        parameter.animator.Play("Idle");
    }

    public void OnUpdate()
    {
        if(parameter.target != null)
        {
            manager.TransitionState(FelkoStateType.Ready);
        }
        else
        {
            //Debug.Log("No Target");
        }
    }

    public void OnExit()
    {

    }
}
