using UnityEngine;

public class EnemiesController : MonoBehaviour
{
    private Vector2 playerPosition;
    private Vector2 enemyPosition;
    private Vector2 direction;
    public EnemiesData EnemiesData;
    private EnemiesData runtimeEnemiesData;

    private Rigidbody2D Rigidbody;
    public GameObject coinPrefab;
    private float nextAttackTime = 0f;
    void Start()
    {
        runtimeEnemiesData = Instantiate(EnemiesData);
        Rigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Move();

    }

    void Move()
    {
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            GetDirections();
            Vector2 movement = direction * Time.deltaTime * runtimeEnemiesData.moveSpeed;
            Rigidbody.MovePosition(Rigidbody.position + movement);
        }
    }
    
    void GetDirections()
    {
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
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
}
