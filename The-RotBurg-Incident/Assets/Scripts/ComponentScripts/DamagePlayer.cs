using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    public float damageAmount;

    public void ApplyDamageToPlayer()
    {
        FindAnyObjectByType<PlayerHealth>().TakeDamage(damageAmount);
    }
}
