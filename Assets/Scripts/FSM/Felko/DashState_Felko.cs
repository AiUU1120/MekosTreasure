using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashState_Felko : IState
{
    private FSM_Felko manager;
    private Parameter parameter;
    private AnimatorStateInfo info;
    //private bool isDebug;
    //private bool isStart;
    private bool isDashSfx;
    public DashState_Felko(FSM_Felko manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }
    public void OnEnter()
    {
        Debug.Log("Enter Dash");
        parameter.rb.velocity = new Vector2(0f, parameter.rb.velocity.y);
        parameter.animator.Play("Dash", 0, 0);
        parameter.dashTimer = parameter.dashTime;
        isDashSfx = false;
    }

    public void OnUpdate()
    {
        info = parameter.animator.GetCurrentAnimatorStateInfo(0);
        
        if (info.normalizedTime > 3f)
        {
            if(!isDashSfx)
            {
                isDashSfx = true;
                AudioManager.Instance.PlaySFX(parameter.felkoDashSfx);
            }
            Dash();
        }
    }

    public void OnExit()
    {
        parameter.dashCDTimer = parameter.dashCD;
        parameter.isSkilling = false;
        parameter.idleTime = 2.5f;
    }

    private void Dash()
    {
        parameter.dashTimer -= Time.deltaTime;
        if (parameter.dashTimer >= 0)
        {
            parameter.dashCol.enabled = true;
            parameter.rb.velocity = new Vector2(parameter.dashSpeed * manager.transform.localScale.x, 0f);
        }
        else if (parameter.dashTimer < 0)
        {
            parameter.dashCol.enabled = false;
            parameter.rb.velocity = new Vector2(0f, parameter.rb.velocity.y);
            manager.TransitionState(FelkoStateType.Ready);
        }
    }
}
