using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface EnemyKnockbackable
{
    void ApplyKnockback(Transform player, float knockBackForce);
}
