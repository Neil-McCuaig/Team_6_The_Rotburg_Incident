using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    public BoxCollider2D coll2D;
    public float interval = 3f;
    public float loweredY = -5f;
    private Animator anim;

    Vector2 originalOffset;

    void Start()
    {
        anim = GetComponent<Animator>();
        coll2D = GetComponent<BoxCollider2D>();
        originalOffset = coll2D.offset;
        StartCoroutine(ToggleRoutine());
    }

    IEnumerator ToggleRoutine()
    {
        while (true)
        {
            coll2D.offset = new Vector2(originalOffset.x, loweredY);
            anim.SetBool("IsOpen", false);
            yield return new WaitForSeconds(interval);

            coll2D.offset = originalOffset;
            anim.SetBool("IsOpen", true);
            yield return new WaitForSeconds(interval);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            FindAnyObjectByType<PlayerHealth>().TakeDamage(10f);
        }
        if(other.CompareTag("Enemy"))
        {
            if (other.GetComponent<EnemyHealth>() != null)
            {
                other.GetComponent<EnemyHealth>().health -= 100f;
            }
        }
    }
}
