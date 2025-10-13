using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

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
            nextAttackTime = Time.time + runtimeEnemiesData.attackSpeed;
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            // 10 is placeholder for bullet speed, change later if needed
            bullet.GetComponent<BulletControllerTest>().Initialize(5f, runtimeEnemiesData.damage);

        }
    }

}
