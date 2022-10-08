using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyState_Felko : IState
{
    private FSM_Felko manager;
    private Parameter parameter;


    public ReadyState_Felko(FSM_Felko manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        Debug.Log("Ready");
        parameter.idleTimer = parameter.idleTime;
        parameter.rb.velocity = new Vector2(0f, parameter.rb.velocity.y);
        parameter.animator.Play("Ready");
        manager.FlipTo(parameter.target);
    }

    public void OnUpdate()
    {
        parameter.idleTimer -= Time.deltaTime;
        if(parameter.idleTimer < 0f)
        {
            manager.TransitionState(FelkoStateType.Chase);
        }
    }

    public void OnExit()
    {
    }
}
