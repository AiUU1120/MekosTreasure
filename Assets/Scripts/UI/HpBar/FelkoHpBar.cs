using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FelkoHpBar : MonoBehaviour
{
    [SerializeField]
    private Image hpBar;
    [SerializeField]
    private RectTransform barPanelTransform;
    [SerializeField]
    Animator animator;
    [SerializeField]
    private AudioData felkoBarSfx;

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.Play("HpShowing");
        AudioManager.Instance.PlaySFX(felkoBarSfx);
    }
    
    public void ChangeHpBar(float curHealth, float maxHealth)
    {
        hpBar.fillAmount = curHealth / maxHealth;
    }

    public void ShowHp()
    {
    }
}
