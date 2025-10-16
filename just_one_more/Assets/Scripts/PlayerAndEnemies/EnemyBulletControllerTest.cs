using UnityEngine;

public class EnemyBulletControllerTest : MonoBehaviour
{
    private Vector2 playerPosition;

    private Vector2 bulletPosition;
    private Vector2 direction;
    private Rigidbody2D Rigidbody;
    private float speed;

    private int damage;

    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        direction = transform.right;
    }

    public void Initialize( float bulletSpeed, int bulletDamage)
    {
        speed = bulletSpeed;
        damage = bulletDamage;
    }

    void FixedUpdate()
    {
        if (playerPosition != null)
        {
            Rigidbody.MovePosition(Rigidbody.position + (Vector2)direction * speed * Time.fixedDeltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy")) return;
        if (other.gameObject.CompareTag("EnemyBullet")) return;
        if (other.gameObject.CompareTag("Coin")) return;
        if (other.gameObject.CompareTag("Player"))
        {

            other.gameObject.GetComponent<PlayerController>().takeDamage(10);
            Destroy(gameObject);
        }
        if (other.gameObject.CompareTag("Edge"))
        {
            Destroy(gameObject);
            return;
        }
    }

}
