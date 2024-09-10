using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class DamageNum : MonoBehaviour
{
    public Text damageText;
    public float lifeTime;//字体显示时间
    public float upSpeed;//字体移动速度

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.position += new Vector3(0, upSpeed * Time.deltaTime, 0);
    }

    public void ShowUIDamge(float _amount)
    {
        damageText.text = _amount.ToString();
    }
}
