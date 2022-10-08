using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall_Felko : MonoBehaviour
{
    GameObject Felko;
    Collider2D collider2d;

    Vector3 dir;

    public float Speed;

    //public float Damge;
    public int minDamage;

    public int maxDamage;

    public float LifeTime;

    private float localX;

    void Start()
    {
        Felko = GameObject.FindWithTag("Boss");
        collider2d = GetComponent<Collider2D>();

        dir = transform.localScale;
        localX = Felko.transform.localScale.x > 0 ? 1 : -1;
    }

    void Update()
    {
        Move();
        LifeTime -= Time.deltaTime;
        if (LifeTime <= 0 || Felko.GetComponent<FSM_Felko>().parameter.isDead)
        {
            Destroy(gameObject);
        }
    }
    public void Move()
    {
        if (localX <= 0)
        {
            transform.localScale = new Vector3(dir.x, dir.y, dir.z);
            transform.position += Speed * -transform.right * Time.deltaTime;
        }
        else if (localX > 0)
        {
            transform.localScale = new Vector3(-dir.x, dir.y, dir.z);
            transform.position += Speed * transform.right * Time.deltaTime;
        }
    }
    public void CloseCollider()
    {
        collider2d.enabled = false;
    }
    public void Destroy()
    {
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttacked"))
        {
            Debug.Log("FireBall Attack!");
            var targetStats = other.GetComponentInParent<CharacterStats>();
            int dierection = transform.localScale.x > 0 ? 1 : -1;
            targetStats.TakeDamage(minDamage, maxDamage, targetStats, dierection);
            Destroy();
        }
        else if (other.CompareTag("Ground"))
        {
            Destroy();
        }
    }
}
