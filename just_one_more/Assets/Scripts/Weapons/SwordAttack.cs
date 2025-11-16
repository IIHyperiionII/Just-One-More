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
            if (Input.GetKey(KeyCode.Mouse0))
            {
                Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, IsEnemy);
                for (int i = 0; i < enemiesToDamage.Length; i++)
                {
                    enemiesToDamage[i].GetComponent<EnemyData>().hp -= damage;
                }
            }
            TimeBtwAttack = startTimeBtwAttack;
        }
        else
        {
            TimeBtwAttack -= Time.deltaTime;
        }
    }
    // void OnGizmosSelected()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireSphere(attackPos.position, attackRange);
    // }
    void OnDrawGizmosSelected()
    {
        // Display the explosion radius when selected
        Gizmos.color = new Color(1, 1, 0, 0.75F);
        Gizmos.DrawSphere(transform.position, attackRange);
    }
}

// public class SwordSwing2D : MonoBehaviour
// {
//     [Header("Swing Settings")]
//     [SerializeField] private float swingTorque = 100f;    // Torque for 2D rotation (adjust for swing speed)
//     [SerializeField] private float swingDuration = 0.4f;  // Duration of the swing
//     [SerializeField] private float swingAngle = 90f;      // Angle of swing (degrees)
//     [SerializeField] private float damage = 25f;          // Damage dealt to enemies

//     [Header("Cooldown & Detection")]
//     [SerializeField] private float attackCooldown = 0.8f; // Time between swings
//     [SerializeField] private LayerMask enemyLayer;       // Enemy layer (set in Inspector)

//     private Rigidbody2D rb;
//     private Collider2D swordCollider;
//     private bool isSwinging = false;
//     private float lastSwingTime = 0f;
//     private Quaternion startRotation;
//     private Vector3 startPosition;

//     void Start()
//     {
//         rb = GetComponent<Rigidbody2D>();
//         swordCollider = GetComponent<Collider2D>();
        
//         // Store initial state
//         startRotation = transform.localRotation;
//         startPosition = transform.localPosition;
        
//         // Start with physics off and collider disabled
//         rb.bodyType = RigidbodyType2D.Kinematic;
//         swordCollider.enabled = false;
//     }

//     void Update()
//     {
//         // Trigger swing on left mouse click (if not on cooldown)
//         if (Input.GetMouseButtonDown(0) && !isSwinging && Time.time >= lastSwingTime + attackCooldown)
//         {
//             StartSwing();
//         }
//     }

//     void StartSwing()
//     {
//         isSwinging = true;
//         lastSwingTime = Time.time;
        
//         // Enable physics and collider
//         rb.bodyType = RigidbodyType2D.Dynamic;
//         swordCollider.enabled = true;
        
//         // Apply torque for a 2D swing (rotates around Z-axis)
//         rb.AddTorque(-swingTorque, ForceMode2D.Impulse); // Negative for clockwise swing; adjust as needed
        
//         // Reset after swing duration
//         Invoke(nameof(EndSwing), swingDuration);
//     }

//     void EndSwing()
//     {
//         isSwinging = false;
        
//         // Reset to kinematic, stop motion, and disable collider
//         rb.bodyType = RigidbodyType2D.Kinematic;
//         rb.angularVelocity = 0f;
//         swordCollider.enabled = false;
        
//         // Snap back to initial position and rotation
//         transform.localRotation = startRotation;
//         transform.localPosition = startPosition;
//     }

//     // Detect hits during swing
//     void OnTriggerEnter2D(Collider2D other)
//     {
//         if (isSwinging && ((1 << other.gameObject.layer) & enemyLayer) != 0)
//         {
//             // Hit an enemy
//             EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
//             if (enemyHealth != null)
//             {
//                 enemyHealth.TakeDamage(damage);
//                 Debug.Log($"Sword hit {other.name}! Dealt {damage} damage.");
//             }
            
//             // Optional: Add knockback
//             Rigidbody2D enemyRb = other.GetComponent<Rigidbody2D>();
//             if (enemyRb != null)
//             {
//                 Vector2 knockbackDir = (other.transform.position - transform.position).normalized;
//                 enemyRb.AddForce(knockbackDir * 5f, ForceMode2D.Impulse);
//             }
//         }
//     }
// }