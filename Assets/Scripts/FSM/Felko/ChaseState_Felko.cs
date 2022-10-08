using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState_Felko : IState
{
    private FSM_Felko manager;
    private Parameter parameter;

    public ChaseState_Felko(FSM_Felko manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        Debug.Log("Chase");
        parameter.animator.Play("Chase");
    }

    public void OnUpdate()
    {
        manager.FlipTo(parameter.target);
        if (parameter.target)
        {
            //manager.transform.position = Vector2.MoveTowards(manager.transform.position,
            //new Vector2(parameter.target.transform.position.x, manager.transform.position.y),
            //parameter.characterStats.RunSpeed * Time.deltaTime);
            parameter.rb.velocity = new Vector2(manager.transform.localScale.x * parameter.characterStats.RunSpeed, parameter.rb.velocity.y);
        }

        if (parameter.attackCDTimer < 0f && Physics2D.OverlapCircle(parameter.attackPoint.position, parameter.attackArea, parameter.targetLayer))
        {
            manager.TransitionState(FelkoStateType.Attack);
        }
    }
    public void OnExit()
    {
    }
}
