using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinsNum : MonoBehaviour
{
    Text coinsNum;

    private void Awake()
    {
        coinsNum = transform.GetChild(1).GetComponent<Text>();
    }

    void Update()
    {
        coinsNum.text = GameManager.Instance.playStats.Coins.ToString();
    }
}
