using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSaber : MonoBehaviour
{
    [ContextMenuItem("ToggleLightSaber", "ToggleLightSaber")]//debug

    public float AnimationDuration= 1f;

    [SerializeField]
    private Animator animator;

    private bool switchedOn = false;

    public void ToggleLightSaber() 
    {
        switchedOn = !switchedOn;
        animator.speed = 1/AnimationDuration;
        animator.SetBool("SwitchedOn", switchedOn);
    }
}
