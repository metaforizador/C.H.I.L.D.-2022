using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationFix : MonoBehaviour
{
    Animator anim;
    public float animSpeed = 0.6f;

    void Start()
    {
        anim = GetComponent<Animator>();
        anim.speed = animSpeed;
    }
}
