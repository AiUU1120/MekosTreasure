using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHpBar : MonoBehaviour
{
    public Image hpBar;
    private Canvas hpCanvas;

    private void Awake()
    {
        hpCanvas = GetComponent<Canvas>();
    }
    private void Start()
    {
        hpCanvas.enabled = false;
    }
    private void Update()
    {
        transform.localScale = transform.parent.localScale;
    }
    public void ChangeHpBar(float curHealth, float maxHealth)
    {
        hpBar.fillAmount = curHealth / maxHealth;
    }

    public void ShowHp()
    {
        hpCanvas.enabled = true;
    }
}
