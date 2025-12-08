using UnityEngine;
using UnityEngine.PlayerLoop;
using System.Collections;

public class SpawningEnemyController : MonoBehaviour, IEnemy
{
    private Vector2 playerPosition;
    private Vector2 lastPlayerPosition;
    private Vector2 enemyPosition;
    private Vector2 direction;
    private Vector2 playerDirection;
    public EnemyData EnemiesData;
    private EnemyData runtimeEnemiesData;
    private Rigidbody2D Rigidbody;
    public GameObject coinPrefab;
    public GameObject childEnemyPrefab;
    private Vector2 spawnPos;
    private float radius;
    private float nextAttackTime = 0f;
    private Transform enemiesParent;
    private string enemyType = "";
    private bool isChangingSprite = false;  
    private bool isFrozen = false;
    void Start()
    {
        runtimeEnemiesData = Instantiate(EnemiesData); // Create a runtime instance of the enemy data for this enemy only
        Rigidbody = GetComponent<Rigidbody2D>();
        GetSpawningRadius(); // Calculate the spawning radius based on the collider size
        enemiesParent = GameManager.enemiesParent;
    }

    void GetSpawningRadius()
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        float width = col.size.x * transform.localScale.x; // Get the width of the collider considering the scale
        float height = col.size.y * transform.localScale.y; // Get the height of the collider considering the scale
        radius = Mathf.Sqrt(Mathf.Pow(width / 2, 2) + Mathf.Pow(height / 2, 2)); // Calculate the radius as half the diagonal of the bounding box

    }
    void FixedUpdate()
    {
        if (GameModeManager.timeIsPaused) return;
        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            Debug.LogError("Player object not found in the scene.");
        } else
        {
            GetPositions();
            Move();
            Attack();
        }

    }
    void GetPositions()
    {
        // Update player and enemy positions for direction calculations
        lastPlayerPosition = playerPosition;
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        enemyPosition = transform.position;
    }

    void Move()
    {
        float dotProduct = GetDotProduct(); // Calculate the dot product between the player's movement direction and the direction to the enemy
        if (dotProduct > 0.5f)
        {
            UpdatePosition(-1);
        }
        else if (dotProduct < -0.5f)
        {
            UpdatePosition(1);
        }
    }
    void UpdatePosition(int sign)
    {
        Vector2 movement = sign * direction * Time.deltaTime * runtimeEnemiesData.moveSpeed;
        Rigidbody.MovePosition(Rigidbody.position + movement);
    }

    float GetDotProduct()
    {
        direction = (playerPosition - enemyPosition).normalized; // Direction vector towards the player
        playerDirection = (playerPosition - lastPlayerPosition).normalized; // Player's movement direction
        float dotProduct = Vector2.Dot(playerDirection, -direction); // Calculate the dot product between the two directions (1 = same direction, -1 = opposite directions)
        return dotProduct;
    }

    void Attack()
    {
        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + 1 / 0.5f; // Set the next attack time (spawning interval)
            SpawnChild(); // Spawn a child enemy
        }
    }
    void SpawnChild()
    {
        Vector2 aimDirection = (playerPosition - enemyPosition).normalized; // Direction vector towards the player
        spawnPos = enemyPosition + aimDirection * (radius + 0.2f); // Calculate spawn position just outside the enemy's collider
        Collider2D hit = Physics2D.OverlapCircle(spawnPos, radius - 1.0f); // Check for collisions at the spawn position
        if (hit == null)
        {
            GameObject childEnemy = Instantiate(childEnemyPrefab, spawnPos, Quaternion.identity); // Spawn the child enemy if the position is clear
            childEnemy.GetComponent<IEnemy>().SetEnemyType("office1");
            if (GameManager.enemiesParent == null)
            {
                Debug.LogError("Enemies Parent object not found in the GameManager.");
            }
            else
            {
                childEnemy.transform.parent = GameManager.enemiesParent; // Set the parent of the spawned child enemy for organization
            }
        }
    }
    void OnDestroy()
    {
        if (!gameObject.scene.isLoaded) return; // Ensure the game object is still part of a loaded scene (scene is not ending) before instantiating the coin
        Instantiate(coinPrefab, transform.position, Quaternion.identity); // Spawn a coin at the enemy's position upon destruction
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
    }
    public void Freeze(float duration)
    {
        if (isFrozen) return;
        StartCoroutine(FreezeCoroutine(duration));
    }
    IEnumerator FreezeCoroutine(float duration)
    {
        isFrozen = true;
        int original = runtimeEnemiesData.moveSpeed;
        if (duration == 2)
        {
            runtimeEnemiesData.moveSpeed = 0;
        } else
        {
            runtimeEnemiesData.moveSpeed /= 2;
        }
        
        yield return new WaitForSeconds(duration);

        if (duration == 2)
        {
            runtimeEnemiesData.moveSpeed = original;
        } else
        {
            runtimeEnemiesData.moveSpeed *= 2;
        }
        isFrozen = false;
    }
    IEnumerator SpriteChange(Sprite newSprite)
    {
        isChangingSprite = true;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Sprite originalSprite = spriteRenderer.sprite;
        spriteRenderer.sprite = newSprite;
        yield return new WaitForSeconds(2f);
        spriteRenderer.sprite = originalSprite;
        isChangingSprite = false;
    }

}
