using UnityEngine;

public class MeleeEnemyController : MonoBehaviour
{
    private Vector2 playerPosition;
    private Vector2 enemyPosition;
    private Vector2 direction;
    
    // Variable References
    public EnemyData EnemiesData;
    private EnemyData runtimeEnemiesData;
    private Rigidbody2D rb; // Renamed to standard 'rb' convention
    public GameObject coinPrefab;
    private float nextAttackTime = 0f;

    // Caching the player to fix performance issues in Update
    private Transform playerTransform; 

    void Start()
    {
        runtimeEnemiesData = Instantiate(EnemiesData); 
        rb = GetComponent<Rigidbody2D>();
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;
    }

    void FixedUpdate()
    {
        Move();
    }

    // --- NEW METHOD START ---
    // This method is called by SwordAttack.cs
    public void TakeDamage(int damageAmount)
    {
        runtimeEnemiesData.hp -= damageAmount;

        // Check if the enemy is dead
        if (runtimeEnemiesData.hp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Destroy creates the "OnDestroy" event, which will spawn your coin
        Destroy(gameObject);
    }
    // --- NEW METHOD END ---

    void Move()
    {
        // Using cached transform is much faster than GameObject.Find in FixedUpdate
        if (playerTransform == null)
        {
            // Try to find player again if lost (optional)
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) playerTransform = p.transform;
            return; 
        }
        
        GetDirections();
        Vector2 movement = direction * Time.deltaTime * runtimeEnemiesData.moveSpeed;
        rb.MovePosition(rb.position + movement);
    }

    void GetDirections()
    {
        playerPosition = playerTransform.position;
        enemyPosition = transform.position;
        direction = (playerPosition - enemyPosition).normalized; 
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + runtimeEnemiesData.attackSpeed;
            other.gameObject.GetComponent<PlayerController>().takeDamage(runtimeEnemiesData.damage);
        }
    }

    void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + runtimeEnemiesData.attackSpeed;
            other.gameObject.GetComponent<PlayerController>().takeDamage(runtimeEnemiesData.damage);
        }
    }

    void OnDestroy()
    {
        if (!gameObject.scene.isLoaded) return;
        Instantiate(coinPrefab, transform.position, Quaternion.identity); 
    }
}