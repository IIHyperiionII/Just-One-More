using UnityEngine;
using UnityEngine.PlayerLoop;

public class RangeBaseEnemyController : MonoBehaviour
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
    void Start()
    {
        runtimeEnemiesData = Instantiate(EnemiesData); // Create an instance of the EnemyData for this enemy only
        Rigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            Debug.LogError("Player does not exist in the scene.");
        } else {
            GetPositions();
            Move();
            Attack();
        }

    }
    
    void GetPositions()
    {
        // Store last player position for calculating directions
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
    void UpdatePosition(int sign)
    {
        // Move enemy towards or away from player based on sign
        Vector2 movement = sign * direction * Time.deltaTime * runtimeEnemiesData.moveSpeed;
        Rigidbody.MovePosition(Rigidbody.position + movement);
    }
    float GetDotProduct()
    {
        direction = (playerPosition - enemyPosition).normalized; // Get the normalized (value is 1, it does not affect speed) direction vector towards the player
        playerDirection = (playerPosition - lastPlayerPosition).normalized; // Get the normalized direction vector of the player's movement
        float dotProduct = Vector2.Dot(playerDirection, -direction); // Calculate dot product (1 = moving directly towards enemy, -1 = moving directly away from enemy)
        return dotProduct;
    }
    void Attack()
    {
        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + 1 / runtimeEnemiesData.attackSpeed; // Set next attack time based on attack speed
            Quaternion rotation = UpdateAngle();
            SpawnBullet(rotation); // Spawn bullet towards player
        }
    }
    Quaternion UpdateAngle()
    {
        Vector2 aimDirection = (playerPosition - enemyPosition).normalized; // Get the normalized direction vector towards the player
        float tmpAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg; // Calculate angle in degrees from radians
        Quaternion angle = Quaternion.Euler(0f, 0f, tmpAngle); // Create rotation quaternion from angle
        return angle;
    }
    void SpawnBullet(Quaternion rotation)
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, rotation); // Spawn bullet at enemy position with calculated rotation
        bullet.GetComponent<EnemyBulletBaseController>().Initialize(runtimeEnemiesData.bulletSpeed, runtimeEnemiesData.damage, rotation); // Initialize bullet with speed and damage
        bullet.transform.SetParent(GameObject.FindGameObjectWithTag("BulletParent").transform); // Set the parent of the spawned bullet for organization
    } 
    void OnDestroy()
    {
        if (!gameObject.scene.isLoaded) return; // Ensure the game object is still part of a loaded scene (scene is not ending) before instantiating the coin
        Instantiate(coinPrefab, transform.position, Quaternion.identity); // Spawn coin at enemy position
    }
}
