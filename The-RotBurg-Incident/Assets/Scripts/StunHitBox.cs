using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunHitBox : MonoBehaviour
{
    public Animator enemyAnim;
    public EnemyController enemyController;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Player hit Enemy");
            StartCoroutine(StunTimer());
            enemyController.isStunned = true;
            enemyAnim.SetBool("isStunned", true);
        }
    }

    IEnumerator StunTimer()
    {
        yield return new WaitForSeconds(0.9f);
        enemyAnim.SetBool("isStunned", false);
        enemyController.isStunned = false;
    }
}
