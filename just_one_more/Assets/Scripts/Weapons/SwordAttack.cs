using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    private float TimeBtwAttack;
    public float startTimeBtwAttack;

    public Transform attackPos;
    public LayerMask IsEnemy;
    public float attackRange;
    public int damage;

    void Update()
    {
        if (TimeBtwAttack <= 0)
        {
            // Move input check here
            if (Input.GetKey(KeyCode.Mouse0))
            {
                Attack(); // Cleaned up by moving logic to a function
                TimeBtwAttack = startTimeBtwAttack; // Only reset timer IF we attacked
            }
        }
        else
        {
            TimeBtwAttack -= Time.deltaTime;
        }
    }

    void Attack()
    {
        // Detect enemies in range of attack
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, IsEnemy);
        
        for (int i = 0; i < enemiesToDamage.Length; i++)
        {
            // LOOK FOR THE CONTROLLER, NOT THE DATA
            MeleeEnemyController enemy = enemiesToDamage[i].GetComponent<MeleeEnemyController>();
            
            if (enemy != null)
            {
                // Call the function we created in the other script
                enemy.TakeDamage(damage);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if(attackPos != null)
            Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }
}