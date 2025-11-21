using System.Collections;
using UnityEngine;

public class RangeSpiningEnemyController : MonoBehaviour
{
    private Vector2 playerPosition;
    private Vector2 enemyPosition;
    private Vector2 direction;
    private Vector2 rotatedDirection;
    private bool isRunning = false;
    public EnemyData EnemiesData;
    private EnemyData runtimeEnemiesData;
    private Rigidbody2D Rigidbody;
    public GameObject bulletPrefab;
    private float nextAttackTime = 0f;
    private float shootingAngle = 0f;
    private int spinDirection = 0;
    private float shootingAngle2 = 180f; // Second bullet angle (opposite direction)
    public GameObject coinPrefab;
    void Awake()
    {
        spinDirection = Random.Range(0, 2) * 2 - 1; // Randomly set to -1 or 1
    }
    void Start()
    {
        runtimeEnemiesData = Instantiate(EnemiesData); // Create an instance of the EnemyData for this enemy only
        Rigidbody = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        if (GameModeManager.playerInCasino) return;
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
        // Get player and enemy positions for direction calculations
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        enemyPosition = transform.position;
    }
    void Move()
    {
        GetDirection();
        UpdatePosition(1);
    }
    void GetDirection()
    {
        direction = (playerPosition - enemyPosition).normalized; // Get the normalized (value is 1, it does not affect speed) direction vector towards the player
        if (!isRunning)
            StartCoroutine(UpdateMovementAngle()); // Start coroutine to update movement angle randomly
    }
    void UpdatePosition(int sign)
    {
        Vector2 movement = sign * rotatedDirection * Time.deltaTime * runtimeEnemiesData.moveSpeed; // Move in the rotated direction clockwise or counter-clockwise based on sign
        Rigidbody.MovePosition(Rigidbody.position + movement);
    }
    void Attack()
    {
        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + 1 / runtimeEnemiesData.attackSpeed; // Set next attack time based on attack speed
            shootingAngle = UpdateAngle(shootingAngle);
            shootingAngle2 = UpdateAngle(shootingAngle2);
            SpawnBullet();
        }
    }
    IEnumerator UpdateMovementAngle()
    {
        isRunning = true; // Set the flag to indicate the coroutine is running
        float randomAngle = Random.Range(-180f, 180f); // Random angle
        rotatedDirection = RotateVector(direction, randomAngle); // Rotate the movement direction vector by the random angle
        yield return new WaitForSeconds(Random.Range(0.1f, 1f)); // Wait for a random duration between 0.1 and 1 seconds (will be moving it the direction during this time)
        isRunning = false;
        yield return null; // End the coroutine
    }
    Vector2 RotateVector(Vector2 v, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad; // Convert degrees to radians
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);
        // Apply rotation matrix result is a new vector rotated by degrees counter-clockwise
        return new Vector2(
            v.x * cos - v.y * sin,
            v.x * sin + v.y * cos
        );
    }
    float UpdateAngle(float angle)
    {
        angle += 15f * spinDirection; // Increment angle by 15 degrees in the spin direction
        if (angle >= 360f) angle = 0f; // Reset angle if it exceeds 360 degrees
        if (angle <= -360f) angle = 0f; // Reset angle if it goes below -360 degrees
        return angle;
    }
    void SpawnBullet()
    {
        // Spawn the first bullet
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0f, 0f, shootingAngle));
        bullet.GetComponent<EnemyBaseBulletController>().Initialize(runtimeEnemiesData.bulletSpeed, runtimeEnemiesData.damage);
        bullet.transform.SetParent(GameObject.FindGameObjectWithTag("BulletParent").transform); // Set the parent of the spawned bullet for organization

        // Spawn the second bullet in the opposite direction
        GameObject bullet2 = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0f, 0f, shootingAngle2));
        bullet2.GetComponent<EnemyBaseBulletController>().Initialize(runtimeEnemiesData.bulletSpeed, runtimeEnemiesData.damage);
        bullet2.transform.SetParent(GameObject.FindGameObjectWithTag("BulletParent").transform); // Set the parent of the spawned bullet for organization
    }
    void OnDestroy()
    {
        if (!gameObject.scene.isLoaded) return; // Ensure the game object is still part of a loaded scene (scene is not ending) before instantiating the coin
        Instantiate(coinPrefab, transform.position, Quaternion.identity);  // Spawn a coin at the enemy's position upon destruction
    }
}
