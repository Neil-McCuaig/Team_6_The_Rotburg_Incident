using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    public float damageAmount;
    private UIEyeballAttackEffect attackEffect;

    private void Start()
    {
        attackEffect = FindAnyObjectByType<UIEyeballAttackEffect>();
    }

    public void ApplyDamageToPlayer()
    {
        FindAnyObjectByType<PlayerHealth>().TakeDamage(damageAmount);
        if (attackEffect != null)
        {
            attackEffect.ActivateOverlay();

            Debug.Log("Triggered");
        }
    }
}
