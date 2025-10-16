using UnityEngine;
using UnityEngine.PlayerLoop;

public class RangedEnemyControllerTest : MonoBehaviour
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
    public GameObject coinPrefab;

    private float nextAttackTime = 0f;
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
        Vector2 movement = sign * direction * Time.deltaTime * runtimeEnemiesData.moveSpeed;
            Rigidbody.MovePosition(Rigidbody.position + movement);
    }

    float GetDotProduct()
    {
        direction = (playerPosition - enemyPosition).normalized;
        playerDirection = (playerPosition - lastPlayerPosition).normalized;
        float dotProduct = Vector2.Dot(playerDirection, -direction);
        return dotProduct;
    }

    void Attack()
    {
        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + 1 / runtimeEnemiesData.attackSpeed;
            Quaternion rotation = UpdateAngle();
            SpawnBullet(rotation);

        }
    }

    Quaternion UpdateAngle()
    {
        Vector2 aimDirection = (playerPosition - enemyPosition).normalized;
        float tmpAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        Quaternion angle = Quaternion.Euler(0f, 0f, tmpAngle);
        return angle;
    }
    void SpawnBullet(Quaternion rotation)
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, rotation);
        bullet.GetComponent<EnemyBulletControllerTest>().Initialize(runtimeEnemiesData.bulletSpeed, runtimeEnemiesData.damage);
    }
    void OnDestroy()
    {
        if (!gameObject.scene.isLoaded) return;
        Instantiate(coinPrefab, transform.position, Quaternion.identity);
    }
}
