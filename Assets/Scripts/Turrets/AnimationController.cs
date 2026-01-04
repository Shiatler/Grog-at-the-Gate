using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Play the shoot animation #########################################################
    public void PlayShootAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Shoot");
        }
    }

    // Play the idle animation #########################################################
    public void PlayIdleAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Idle");
        }
    }

    // Play the laser animation #########################################################
    public void PlayLaserAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Laser1");
            animator.SetTrigger("Laser2");
        }
    }
}