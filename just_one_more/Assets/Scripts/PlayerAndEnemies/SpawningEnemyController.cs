using UnityEngine;
using UnityEngine.PlayerLoop;

public class SpawningEnemyController : MonoBehaviour
{

    private Vector2 playerPosition;
    private Vector2 lastPlayerPosition;
    private Vector2 enemyPosition;
    private Vector2 direction;
    private Vector2 playerDirection;
    public EnemiesData EnemiesData;
    private EnemiesData runtimeEnemiesData;
    private Rigidbody2D Rigidbody;
    public GameObject coinPrefab;
    public GameObject childEnemyPrefab;
    private Vector2 spawnPos;
    private float radius;

    private float nextAttackTime = 0f;
    void Start()
    {
        runtimeEnemiesData = Instantiate(EnemiesData);
        Rigidbody = GetComponent<Rigidbody2D>();
        GetSpawningRadius();
    }

    void GetSpawningRadius()
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        float width = col.size.x * transform.localScale.x;
        float height = col.size.y * transform.localScale.y;
        radius = Mathf.Sqrt(Mathf.Pow(width / 2, 2) + Mathf.Pow(height / 2, 2));

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
            nextAttackTime = Time.time + 1 / 0.5f;
            SpawnChild();

        }
    }
    void SpawnChild()
    {
        Vector2 aimDirection = (playerPosition - enemyPosition).normalized;
        spawnPos = enemyPosition + aimDirection * (radius + 0.2f);
        Collider2D hit = Physics2D.OverlapCircle(spawnPos, radius - 1.0f);
        if (hit == null)
        {
            Instantiate(childEnemyPrefab, spawnPos, Quaternion.identity);
        }
    }
    void OnDestroy()
    {
        if (!gameObject.scene.isLoaded) return;
        Instantiate(coinPrefab, transform.position, Quaternion.identity);
    }

}
