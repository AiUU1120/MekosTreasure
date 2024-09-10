using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowSprite : MonoBehaviour
{
    private Transform player;
    private SpriteRenderer thisSprite;
    private SpriteRenderer playerSprite;
    private Color color;

    [Header("Time Controller")]
    public float activeTime;
    public float activeStart;

    [Header("Alpha Controller")]
    public float alphaSet;
    [Range(0f, 1f)]
    public float alphaMultiplier; 
    private float alpha;

    private void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        thisSprite = GetComponent<SpriteRenderer>();
        playerSprite = player.GetComponent<SpriteRenderer>();

        alpha = alphaSet;
        thisSprite.sprite = playerSprite.sprite;
        transform.position = player.position;
        transform.localScale = player.localScale;
        transform.rotation = player.rotation;

        activeStart = Time.time;
    }

    void FixedUpdate()
    {
        alpha *= alphaMultiplier;

        color = new Color(0.952f, 0.882f, 0.682f, alpha);

        thisSprite.color = color;

        if(Time.time >= activeStart + activeTime)
        {
            ShadowPool.instance.ReturnPool(this.gameObject);
        }
    }
}
