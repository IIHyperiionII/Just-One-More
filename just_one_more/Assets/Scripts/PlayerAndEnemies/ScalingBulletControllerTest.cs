using UnityEngine;

public class ScalingBulletControllerTest : MonoBehaviour
{
    private Vector2 playerPosition;

    private Vector2 bulletPosition;
    private Vector2 direction;
    private Rigidbody2D Rigidbody;
    private float speed;
    private float growth;
    private int damage;
    public Transform spriteTransform; 

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
    void Update()
    {
        float growSpeed = 0.5f;
        growth = growSpeed * Time.deltaTime;
        transform.localScale += new Vector3(growth, growth, 0);
        if (spriteTransform != null)
            spriteTransform.localScale += new Vector3(growth, growth, 0);
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box != null)
        {
            box.size = Vector2.one;
        }
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
