using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    public float damageAmount;
    private PlayerController player;
    private UIEyeballAttackEffect attackEffect;

    private void Start()
    {
        attackEffect = FindAnyObjectByType<UIEyeballAttackEffect>();
        player = FindAnyObjectByType<PlayerController>();
    }

    public void ApplyDamageToPlayer()
    {
        FindAnyObjectByType<PlayerHealth>().TakeDamage(damageAmount, gameObject);

        if (!player.isDead)
        {
            if (attackEffect != null)
            {
                attackEffect.ActivateOverlay();
            }
        }
    }
}
