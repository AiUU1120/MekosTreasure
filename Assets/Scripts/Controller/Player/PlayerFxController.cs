using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFxController : MonoBehaviour
{
    public static PlayerFxController instance;

    GameObject basicAttackFx;
    Animator attackAnimator;
    GameObject attackFx;

    [SerializeField] GameObject basicAttackFxPrefab;
    float info;
    //Transform lockTransform;
    //bool fxPlaying;
    float newLocalScale;
    bool isFxPlaying;
    private void Awake()
    {
        //if (instance == null)
        //{
        //    instance = this;
        //}
        //else
        //{
        //    if(instance != this)
        //    {
        //        Destroy(gameObject);
        //    }
        //}

        //basicAttackFx = GameObject.Find("BasicAttackFx").gameObject;
    }


    private void Start()
    {
        //attackAnimator = basicAttackFx.GetComponent<Animator>();
    }
    private void Update()
    {
        if (isFxPlaying)
        {
            info = attackAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        }

        if (info >= 0.99f)
        {
            isFxPlaying = false;
            Destroy(attackFx);
        }
        //if (fxPlaying)
        //{
        //    basicAttackFx.transform.position = lockTransform.position;
        //    basicAttackFx.transform.rotation = lockTransform.rotation;
        //    basicAttackFx.transform.localScale = lockTransform.localScale;
        //    if(info >= 0.98f)
        //    {
        //        fxPlaying = false;
        //    }
        //}
    }

    public void ChangeAttackAnimation(int comboStep)
    {
        if (!isFxPlaying)
        {
            attackFx = Instantiate(basicAttackFxPrefab);

            //SetTransform();
            //fxPlaying = true;
            //Debug.Log("Fuck Unity");
        }
        attackFx.transform.position = new Vector3(transform.position.x + (transform.localScale.x * 1.5f), transform.position.y - 0.2f, transform.position.z);
        attackFx.transform.localScale = transform.localScale;
        attackAnimator = attackFx.GetComponent<Animator>();
        isFxPlaying = true;
        attackAnimator.SetInteger("ComboStep", comboStep);
        attackAnimator.SetTrigger("Hit");
    }

    public void SetTransform()
    {
        basicAttackFx.transform.localScale = transform.localScale;
        basicAttackFx.transform.position = new Vector3(transform.position.x + (transform.localScale.x * 1.5f), transform.position.y - 0.2f, transform.position.z);
        //lockTransform = basicAttackFx.transform.transform;
    }
}
