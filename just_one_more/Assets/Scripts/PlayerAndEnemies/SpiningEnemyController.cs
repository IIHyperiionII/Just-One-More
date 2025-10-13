using UnityEngine;

public class SpiningEnemyController : MonoBehaviour
{
    

    private Vector2 playerPosition;
    
    private Vector2 lastPlayerPosition;
    private Vector2 enemyPosition;
    private Vector2 direction;

    private Vector2 playerDirection;
    public EnemiesData EnemiesData;
    private EnemiesData runtimeEnemiesData;

    private Rigidbody2D Rigidbody;

    public GameObject bulletPrefab;

    private float nextAttackTime = 0f;

    private float shootingAngle = 0f;
    private int spinDirection = 0;

    void Awake()
    {
        spinDirection = Random.Range(0, 2) * 2 - 1; // Randomly set to -1 or 1
    }
    void Start()
    {
        runtimeEnemiesData = Instantiate(EnemiesData);
        Rigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            GetPositions();
            Move();
            Attack();
        }

    }
    
    void GetPositions()
    {
        lastPlayerPosition = playerPosition;
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        enemyPosition = transform.position;
    }

    void Move()
    {
        // Move away from the player if they are moving towards the enemy, TODO: add a safe distance
        direction = (playerPosition - enemyPosition).normalized;
        playerDirection = (playerPosition - lastPlayerPosition).normalized;
        float dotProduct = Vector2.Dot(playerDirection, -direction);
        if (dotProduct > 0.5f)
        {
            Vector2 movement = -direction * Time.deltaTime * runtimeEnemiesData.moveSpeed;
            Rigidbody.MovePosition(Rigidbody.position + movement);
        } else if (dotProduct < -0.5f)
        {
            Vector2 movement = direction * Time.deltaTime * runtimeEnemiesData.moveSpeed;
            Rigidbody.MovePosition(Rigidbody.position + movement);
        }
    }
    
    void Attack()
    {
        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + 1/runtimeEnemiesData.attackSpeed;
            shootingAngle += 15f * spinDirection;
            if (shootingAngle >= 360f) shootingAngle = 0f;
            if (shootingAngle <= -360f) shootingAngle = 0f;
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0f, 0f, shootingAngle));
            // 10 is placeholder for bullet speed, change later if needed
            bullet.GetComponent<BulletControllerTest>().Initialize(runtimeEnemiesData.bulletSpeed, runtimeEnemiesData.damage);

        }
    }
}
