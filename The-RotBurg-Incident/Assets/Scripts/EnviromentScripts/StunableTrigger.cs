using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunableTrigger : MonoBehaviour, EnemyStunable
{
    [Header("Stun Settings")]
    public float stunDuration = 5f;
    private bool isStunned = false;

    [Header("Door Prefab")]
    public Animator doorAnimator;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Stun()
    {
        if (isStunned)
        {
            return;
        }
        StartCoroutine(StunRoutine());
    }

    private IEnumerator StunRoutine()
    {
        isStunned = true;
        anim.SetBool("IsWorking", false);

        if (doorAnimator != null)
        {
            doorAnimator.SetBool("IsOpen", true);
        }

        yield return new WaitForSeconds(stunDuration);

        if (doorAnimator != null)
        {
            doorAnimator.SetBool("IsOpen", false);
        }

        anim.SetBool("IsWorking", true);
        isStunned = false;
    }
}
