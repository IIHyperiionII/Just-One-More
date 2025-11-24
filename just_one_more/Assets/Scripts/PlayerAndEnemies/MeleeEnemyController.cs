using System.Collections;
using UnityEngine;

public class MeleeEnemyController : MonoBehaviour, IEnemy
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
    private string enemyType;
    private bool isChangingSprite = false;
    private bool isInvisible = false;

    // Caching the player to fix performance issues in Update
    private Transform playerTransform; 

    void Start()
    {
        if (runtimeEnemiesData == null){
        runtimeEnemiesData = Instantiate(EnemiesData); // Create an instance of the EnemyData for this enemy only
        }
        Rigidbody = GetComponent<Rigidbody2D>();
        runtimeEnemiesData = Instantiate(EnemiesData); 
        rb = GetComponent<Rigidbody2D>();
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;
    }

    void FixedUpdate()
    {
        if (GameModeManager.playerInCasino) return;
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

    public EnemyData GetEnemyData()
    {
        if (runtimeEnemiesData == null)
        {
            runtimeEnemiesData = Instantiate(EnemiesData);
        }
        return runtimeEnemiesData;
    }
    public Transform GetTransform()
    {
        return transform;
    }
    public void SetEnemyType(string type)
    {
        enemyType = type;
    }
    public string GetEnemyType()
    {
        return enemyType;
    }
    public void TakeDamage(int damage)
    {
        runtimeEnemiesData.hp -= damage;
        if (runtimeEnemiesData.hp <= 0)
        {
            Destroy(gameObject);
        }
        if (GameManager.Instance.runtimePlayerData.needToGamble > 70 && Random.Range(0, 100) < 20 && !isChangingSprite)
        {
            Sprite newSprite = GameManager.Instance.GetRandomSprite(GetComponent<SpriteRenderer>().sprite);
            StartCoroutine(SpriteChange(newSprite));
        }
        if (GameManager.Instance.runtimePlayerData.needToGamble > 70 && Random.Range(0, 100) < 20 && !isInvisible)
        {
            StartCoroutine(BecomeInvisible());
        }
    }
    IEnumerator SpriteChange(Sprite newSprite)
    {
        Debug.Log("Changing sprite to " + newSprite.name);
        isChangingSprite = true;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Sprite originalSprite = spriteRenderer.sprite;
        spriteRenderer.sprite = newSprite;
        yield return new WaitForSeconds(2f);
        spriteRenderer.sprite = originalSprite;
        isChangingSprite = false;
    }
    IEnumerator BecomeInvisible()
    {
        Debug.Log("Becoming invisible");
        isInvisible = true;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f); // Set alpha to 0.0 for invisibility
        yield return new WaitForSeconds(3f);
        spriteRenderer.color = originalColor; // Restore original color
        isInvisible = false;
    }

}