using System.Collections;
using UnityEngine;

public class SpiningEnemyController : MonoBehaviour
{
    

    private Vector2 playerPosition;
    
    private Vector2 lastPlayerPosition;
    private Vector2 enemyPosition;
    private Vector2 direction;
    private Vector2 rotatedDirection;
    private bool isRunning = false;
    public EnemiesData EnemiesData;
    private EnemiesData runtimeEnemiesData;

    private Rigidbody2D Rigidbody;

    public GameObject bulletPrefab;

    private float nextAttackTime = 0f;

    private float shootingAngle = 0f;
    private int spinDirection = 0;
    private float shootingAngle2 = 180f;
    public GameObject coinPrefab;

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
        GetDirection();
        UpdatePosition(1);
    }
    void UpdatePosition(int sign)
    {
        Vector2 movement = sign * rotatedDirection * Time.deltaTime * runtimeEnemiesData.moveSpeed;
        Rigidbody.MovePosition(Rigidbody.position + movement);
    }

    void GetDirection()
    {
        direction = (playerPosition - enemyPosition).normalized;
        if (!isRunning)
            StartCoroutine(UpdateMovementAngle());
    }

    void Attack()
    {
        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + 1 / runtimeEnemiesData.attackSpeed;
            shootingAngle = UpdateAngle(shootingAngle);
            shootingAngle2 = UpdateAngle(shootingAngle2);
            SpawnBullet();
        }
    }

    IEnumerator UpdateMovementAngle()
    {
        isRunning = true;
        float randomAngle = Random.Range(-180f, 180f);
        rotatedDirection = RotateVector(direction, randomAngle);
        yield return new WaitForSeconds(Random.Range(0.1f, 1f));
        isRunning = false;
        yield return null;
    }

    Vector2 RotateVector(Vector2 v, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);

        return new Vector2(
            v.x * cos - v.y * sin,
            v.x * sin + v.y * cos
        );
    }


    float UpdateAngle(float angle)
    {
        angle += 15f * spinDirection;
        if (angle >= 360f) angle = 0f;
        if (angle <= -360f) angle = 0f;
        return angle;
    }
    void SpawnBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0f, 0f, shootingAngle));
        bullet.GetComponent<EnemyBulletControllerTest>().Initialize(runtimeEnemiesData.bulletSpeed, runtimeEnemiesData.damage);

        GameObject bullet2 = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0f, 0f, shootingAngle2));
        bullet2.GetComponent<EnemyBulletControllerTest>().Initialize(runtimeEnemiesData.bulletSpeed, runtimeEnemiesData.damage);
    }
    void OnDestroy()
    {
        if (!gameObject.scene.isLoaded) return;
        Instantiate(coinPrefab, transform.position, Quaternion.identity);
    }
}
