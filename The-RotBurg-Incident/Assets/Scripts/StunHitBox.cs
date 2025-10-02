using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunHitBox : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyStunable stunnable = other.GetComponent<EnemyStunable>();
            if (stunnable != null)
            {
                stunnable.Stun();
            }
        }
    }
}
