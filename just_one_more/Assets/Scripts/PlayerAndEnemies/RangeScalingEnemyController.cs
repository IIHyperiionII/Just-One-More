using UnityEngine;
using System.Collections;

public class RangedScalingEnemyController : MonoBehaviour, IEnemy
{
    private Vector2 playerPosition;
    private Vector2 lastPlayerPosition;
    private Vector2 enemyPosition;
    private Vector2 direction;
    private Vector2 playerDirection;
    public EnemyData EnemiesData;
    private EnemyData runtimeEnemiesData;
    private Rigidbody2D Rigidbody;
    public GameObject bulletPrefab;
    public GameObject coinPrefab;
    private float nextAttackTime = 0f;
    private string enemyType = "";
    private bool isChangingSprite = false;
    void Start()
    {
        runtimeEnemiesData = Instantiate(EnemiesData); // Create an instance of the EnemyData for this enemy only
        Rigidbody = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        if (GameModeManager.timeIsPaused) return;
        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            Debug.LogError("Player does not exist in the scene.");
            return;
        } else {
            GetPositions();
            Move();
            Attack();
        }

    }
    void GetPositions()
    {
        // Store last player position for direction calculation
        lastPlayerPosition = playerPosition;
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        enemyPosition = transform.position;
    }
    void Move()
    {
        // Calculate dot product to determine if player is moving towards or away from enemy and move accordingly
        float dotProduct = GetDotProduct();
        if (dotProduct > 0.5f)
        {
            UpdatePosition(-1);
        }
        else if (dotProduct < -0.5f)
        {
            UpdatePosition(1);
        }
    }
    float GetDotProduct()
    {
        direction = (playerPosition - enemyPosition).normalized; // Direction from enemy to player
        playerDirection = (playerPosition - lastPlayerPosition).normalized; // Player movement direction
        float dotProduct = Vector2.Dot(playerDirection, -direction); // Dot product to determine if player is moving towards or away from enemy ( 1 = towards, -1 = away)
        return dotProduct;
    }
    void UpdatePosition(int sign)
    {
        Vector2 movement = sign * direction * Time.deltaTime * runtimeEnemiesData.moveSpeed; 
        Rigidbody.MovePosition(Rigidbody.position + movement);
    }
    void Attack()
    {
        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + 1 / runtimeEnemiesData.attackSpeed; // Calculate next attack time based on attack speed
            Quaternion rotation = UpdateAngle();
            SpawnBullet(rotation); 

        }
    }
    Quaternion UpdateAngle()
    {
        Vector2 aimDirection = (playerPosition - enemyPosition).normalized; // Direction from enemy to player
        float tmpAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg; // Calculate angle in degrees from radians
        Quaternion angle = Quaternion.Euler(0f, 0f, tmpAngle); // Create rotation quaternion from angle
        return angle;
    }
    void SpawnBullet(Quaternion rotation)
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, rotation); // Spawn scaling bullet at enemy position with calculated rotation
        bullet.GetComponent<EnemyBulletScalingController>().Initialize(runtimeEnemiesData.bulletSpeed, runtimeEnemiesData.damage, rotation); // Initialize bullet with speed and damage
        bullet.transform.SetParent(GameObject.FindGameObjectWithTag("BulletParent").transform); // Set the parent of the spawned bullet for organization
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
