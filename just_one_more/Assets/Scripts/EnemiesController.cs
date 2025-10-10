using UnityEngine;

public class EnemiesController : MonoBehaviour
{
    private Vector2 playerPosition;
    private Vector2 enemyPosition;
    private Vector2 direction;
    public EnemiesData EnemiesData;
    private EnemiesData runtimeEnemiesData;

    private Rigidbody2D Rigidbody;

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
            
            playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
            enemyPosition = transform.position;
            direction = (playerPosition - enemyPosition).normalized;
            Vector2 movement = direction * Time.fixedDeltaTime * runtimeEnemiesData.moveSpeed;
            Debug.Log("Enemy Movement: " + movement);
            Rigidbody.MovePosition(Rigidbody.position + movement);
        }

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
}
