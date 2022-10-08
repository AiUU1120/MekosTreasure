using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpBar : MonoBehaviour
{
    private CharacterStats characterStats;

    public Image spPointImage;

    private PlayerController player;
    private GameObject spBox;
    private CanvasGroup spSpa;
    private float alpha = 0;
    private float alphaSpeed = 6f;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        spBox = GameObject.Find("CanvasSp");
    }

    private void Start()
    {
        spSpa = spBox.GetComponent<CanvasGroup>();
        characterStats = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterStats>();
    }
    private void Update()
    {
        spPointImage.fillAmount = player.curSp / characterStats.MaxSp;

        if (player.curSp >= characterStats.MaxSp)
        {
            if (alpha != spSpa.alpha)
            {
                spSpa.alpha = Mathf.Lerp(spSpa.alpha, alpha, alphaSpeed * Time.deltaTime);
            }
        }
        else
        {
            //spBox.SetActive(true);
            spSpa.alpha = 1;
        }
        if (Mathf.Abs(alpha - spSpa.alpha) <= 0.01)
        {
            spSpa.alpha = alpha;
        }
        /*if (player.curSp >= player.maxSp)
        {
            Invoke("CloseBar", 0.3f);
        }*/
    }

    private void CloseBar()
    {
        if(alpha != spSpa.alpha)
        {
            spSpa.alpha = Mathf.Lerp(spSpa.alpha, alpha, alphaSpeed * Time.deltaTime);
        }
        spBox.SetActive(false);
    }
}
