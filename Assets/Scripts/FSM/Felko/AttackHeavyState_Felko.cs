using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHeavyState_Felko : IState
{
    private FSM_Felko manager;
    private Parameter parameter;

    private AnimatorStateInfo info;
    private bool isSfxPlay;
    public AttackHeavyState_Felko(FSM_Felko manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }
    public void OnEnter()
    {
        Debug.Log("Enter Attack Heavy");
        parameter.rb.velocity = new Vector2(0f, parameter.rb.velocity.y);
        parameter.animator.Play("AttackHeavy", 0, 0);
        isSfxPlay = false;
    }

    public void OnUpdate()
    {
        info = parameter.animator.GetCurrentAnimatorStateInfo(0);
        if (info.normalizedTime >= 0.95f)
        {
            Debug.Log("Attack Heavy To Ready");
            manager.TransitionState(FelkoStateType.Ready);
        }
        if (info.normalizedTime >= 0.5f)
        {
            if (!isSfxPlay)
            {
                isSfxPlay = true;
                AudioManager.Instance.PlaySFX(parameter.felkoHeavyAttackSfx);
            }
        }
    }

    public void OnExit()
    {
        parameter.attackHeavyCDTimer = parameter.attackHeavyCD;
        parameter.isSkilling = false;
        parameter.idleTime = 3.5f;
    }
}
