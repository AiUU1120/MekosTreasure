using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallState_Felko : IState
{
    private FSM_Felko manager;
    private Parameter parameter;

    private AnimatorStateInfo info;
    public FireBallState_Felko(FSM_Felko manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }
    public void OnEnter()
    {
        Debug.Log("Enter Fire Ball");
        parameter.rb.velocity = new Vector2(0f, parameter.rb.velocity.y);
        parameter.animator.Play("FireBall", 0, 0);
    }

    public void OnUpdate()
    {
        info = parameter.animator.GetCurrentAnimatorStateInfo(0);
        if (info.normalizedTime >= 3f)
        {
            Debug.Log("FireBall Create");
            manager.FireBallCreate();
            AudioManager.Instance.PlaySFX(parameter.felkoFireBallSfx);
            manager.TransitionState(FelkoStateType.Ready);
        }
    }

    public void OnExit()
    {
        parameter.fireBallCDTimer = parameter.fireBallCD;
        parameter.isSkilling = false;
        parameter.idleTime = 3.5f;
    }
}
