using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    private CharacterStats characterStats;

    public Image hpPointImage;

    private void Start()
    {
        characterStats = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterStats>();
    }

    private void Update()
    {
        hpPointImage.fillAmount = characterStats.characterData.curHealth / characterStats.characterData.maxHealth;
    }
}
