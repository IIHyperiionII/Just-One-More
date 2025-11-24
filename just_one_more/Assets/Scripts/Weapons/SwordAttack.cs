using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    [Header("Stats")]
    private float TimeBtwAttack;
    public float startTimeBtwAttack;
    public float attackRange = 1.5f;
    public int damage = 1;

    [Header("Configuration")]
    public Transform attackPos; 
    public LayerMask IsEnemy;
    
    // 180 = Half Circle, 90 = Triangle/Cone
    [Range(0, 360)]
    public float attackAngle = 180f; 

    public Animator animator;

    void Update()
    {
        RotateTowardsMouse();

        if (TimeBtwAttack <= 0)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                Attack();
                TimeBtwAttack = startTimeBtwAttack;
            }
        }
        else
        {
            TimeBtwAttack -= Time.deltaTime;
        }
    }

    void RotateTowardsMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f; 

        Vector2 direction = (mousePos - transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        attackPos.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Attack()
    {

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, IsEnemy);
        
        for (int i = 0; i < enemiesToDamage.Length; i++)
        {
            Vector2 directionToEnemy = (enemiesToDamage[i].transform.position - attackPos.position).normalized;
            
            float angleToEnemy = Vector2.Angle(attackPos.right, directionToEnemy);

            if (angleToEnemy < attackAngle / 2f)
            {
                MeleeEnemyController enemy = enemiesToDamage[i].GetComponent<MeleeEnemyController>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        if (attackPos == null) return;

        Gizmos.color = new Color(1, 1, 0, 0.3f); 
        Gizmos.DrawWireSphere(attackPos.position, attackRange);

        Gizmos.color = Color.red;
        
        Vector3 topDir = Quaternion.Euler(0, 0, attackAngle / 2) * attackPos.right;
        Vector3 botDir = Quaternion.Euler(0, 0, -attackAngle / 2) * attackPos.right;

        Gizmos.DrawLine(attackPos.position, attackPos.position + topDir * attackRange);
        Gizmos.DrawLine(attackPos.position, attackPos.position + botDir * attackRange);
    }
}