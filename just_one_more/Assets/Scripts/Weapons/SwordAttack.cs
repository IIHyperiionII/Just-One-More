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
    public Transform attackPos; // This should be a child of the Player
    public LayerMask IsEnemy;
    
    // 180 = Half Circle, 90 = Triangle/Cone
    [Range(0, 360)]
    public float attackAngle = 180f; 

    void Update()
    {
        // 1. Always make the attack point face the mouse cursor
        RotateTowardsMouse();

        // 2. Handle Attack Cooldown
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
        // Get mouse position in World Space
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f; // Flatten Z axis for 2D

        // Calculate direction from the Attack Point (Player center) to the Mouse
        Vector2 direction = (mousePos - transform.position).normalized;

        // Calculate the angle in degrees
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Apply the rotation to the attackPos transform
        // We use the Z axis because this is a 2D game
        attackPos.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Attack()
    {
        // 1. Detect ALL enemies in range (The full circle)
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, IsEnemy);
        
        for (int i = 0; i < enemiesToDamage.Length; i++)
        {
            // 2. Filter for enemies inside the Angle (The Cone/Triangle)
            Vector2 directionToEnemy = (enemiesToDamage[i].transform.position - attackPos.position).normalized;
            
            // attackPos.right is now pointing at the mouse because of RotateTowardsMouse()
            float angleToEnemy = Vector2.Angle(attackPos.right, directionToEnemy);

            // Check if enemy is within half of the total angle (e.g., within 90 degrees of center for a 180 degree swing)
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

        // Draw the Range Circle
        Gizmos.color = new Color(1, 1, 0, 0.3f); // Yellow transparent
        Gizmos.DrawWireSphere(attackPos.position, attackRange);

        // Draw the Cone/Triangle Lines
        Gizmos.color = Color.red;
        
        // These calculations rotate the lines relative to where the attackPos is currently facing
        Vector3 topDir = Quaternion.Euler(0, 0, attackAngle / 2) * attackPos.right;
        Vector3 botDir = Quaternion.Euler(0, 0, -attackAngle / 2) * attackPos.right;

        Gizmos.DrawLine(attackPos.position, attackPos.position + topDir * attackRange);
        Gizmos.DrawLine(attackPos.position, attackPos.position + botDir * attackRange);
    }
}