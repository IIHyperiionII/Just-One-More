using UnityEngine;

public class EnemyBulletBaseController : MonoBehaviour
{
    private Vector2 direction;
    private Rigidbody2D Rigidbody;
    public string type = "BaseEnemyBullet";
    public int speed;
    public int damage;
    public Quaternion initialRotation;
    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        direction = transform.right; // applying given rotation to world x axis
    }

    public void Initialize( int bulletSpeed, int bulletDamage , Quaternion rotation)
    {
        speed = bulletSpeed;
        damage = bulletDamage;
        initialRotation = rotation;
    }

    void FixedUpdate()
    {
        // Move the bullet in the set direction
        Rigidbody.MovePosition(Rigidbody.position + direction * speed * Time.fixedDeltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy")) return;
        if (other.gameObject.CompareTag("EnemyBullet")) return;
        if (other.gameObject.CompareTag("Coin")) return;
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController>().takeDamage(damage);
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
