using UnityEngine;

public class MeleeEnemyController : MonoBehaviour
{
    private Vector2 playerPosition;
    private Vector2 enemyPosition;
    private Vector2 direction;
    public EnemyData EnemiesData;
    private EnemyData runtimeEnemiesData;
    private Rigidbody2D Rigidbody;
    public GameObject coinPrefab;
    private float nextAttackTime = 0f;
    void Start()
    {
        runtimeEnemiesData = Instantiate(EnemiesData); // Create an instance of the EnemyData for this enemy only
        Rigidbody = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        Move();
    }
    void Move()
    {
        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            Debug.LogError("Player does not exist in the scene.");
        } else {
            GetDirections();
            Vector2 movement = direction * Time.deltaTime * runtimeEnemiesData.moveSpeed;
            Rigidbody.MovePosition(Rigidbody.position + movement);
        }
    }
    void GetDirections()
    {
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        enemyPosition = transform.position;
        direction = (playerPosition - enemyPosition).normalized; // Get the normalized (value is 1, it does not affect speed) direction vector towards the player
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        // Check if the collided object has the "Player" tag
        if (other.gameObject.CompareTag("Player") && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + runtimeEnemiesData.attackSpeed;

            other.gameObject.GetComponent<PlayerController>().takeDamage(runtimeEnemiesData.damage);
        }
    }
    void OnCollisionStay2D(Collision2D other)
    {
        // Check if the collided object that is staying in contact has the "Player" tag
        if (other.gameObject.CompareTag("Player") && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + runtimeEnemiesData.attackSpeed;
            other.gameObject.GetComponent<PlayerController>().takeDamage(runtimeEnemiesData.damage);
        }
    }
    void OnDestroy()
    {
        // Ensure the game object is still part of a loaded scene (scene is not ending) before instantiating the coin
        if (!gameObject.scene.isLoaded) return;
        Instantiate(coinPrefab, transform.position, Quaternion.identity); // Spawn a coin at the enemy's position upon destruction
    }
}
