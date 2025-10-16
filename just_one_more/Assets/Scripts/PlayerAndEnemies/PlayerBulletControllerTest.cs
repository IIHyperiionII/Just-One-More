using UnityEngine;

public class PlayerBulletControllerTest : MonoBehaviour
{
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
        
        Rigidbody.MovePosition(Rigidbody.position + (Vector2)direction * speed * Time.fixedDeltaTime);
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")) return;
        if (other.gameObject.CompareTag("PlayerBullet")) return;
        if (other.gameObject.CompareTag("Coin")) return;
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("EnemyBullet"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
            return;
        }
        if (other.gameObject.CompareTag("Edge"))
        {
            Destroy(gameObject);
            return;
        }
    }

}
