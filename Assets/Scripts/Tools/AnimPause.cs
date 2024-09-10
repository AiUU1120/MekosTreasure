using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPause : MonoBehaviour
{
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void endPlay()
    {
        animator.speed = 0;
    }
}
