using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState_Felko : IState
{
    private FSM_Felko manager;
    private Parameter parameter;

    private AnimatorStateInfo info;

    public AttackState_Felko(FSM_Felko manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        Debug.Log("Enter Attack");
        parameter.rb.velocity = new Vector2(0f, parameter.rb.velocity.y);
        parameter.animator.Play("Attack");
        AudioManager.Instance.PlaySFX(parameter.felkoAttackSfx);
    }

    public void OnUpdate()
    {
        info = manager.parameter.animator.GetCurrentAnimatorStateInfo(0);

        if(info.normalizedTime >= 0.95f)
        {
            manager.TransitionState(FelkoStateType.Ready);
        }
    }

    public void OnExit()
    {
        parameter.attackCDTimer = parameter.attackCD;
        parameter.idleTime = 1.5f;
    }
}
