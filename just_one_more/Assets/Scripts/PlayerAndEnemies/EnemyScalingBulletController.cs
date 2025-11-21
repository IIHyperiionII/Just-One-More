using UnityEngine;

public class EnemyScalingBulletController : MonoBehaviour
{
    private Vector2 direction;
    private Rigidbody2D Rigidbody;
    private int speed;
    private float growth;
    private int damage;
    public Transform spriteTransform;

    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        direction = transform.right; // applying given rotation to world x axis
    }

    public void Initialize(int bulletSpeed, int bulletDamage)
    {
        speed = bulletSpeed;
        damage = bulletDamage;
    }
    void Update()
    {
        if (GameModeManager.playerInCasino) return;
        ScaleSize();
    }
    void FixedUpdate()
    {
        Rigidbody.MovePosition(Rigidbody.position + (Vector2)direction * speed * Time.fixedDeltaTime);
    }

    void ScaleSize()
    {
        // Increase size over time
        float growSpeed = 0.5f;
        growth = growSpeed * Time.deltaTime;
        transform.localScale += new Vector3(growth, growth, 0); // Scale the object
        if (spriteTransform == null) // Check if object has a sprite transform assigned
        {
            Debug.LogWarning("Sprite Transform is not assigned in EnemyScalingBulletController.");
        }else
        {
            spriteTransform.localScale += new Vector3(growth, growth, 0); // Scale the sprite
        }
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box == null) // Check if BoxCollider2D component exists
        {
            Debug.LogWarning("BoxCollider2D component not found on the bullet.");
            box.size = Vector2.one;
        } else {
            box.size = Vector2.one; // Keep collider size constant towards object scale
        }
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
        }
        if (other.gameObject.CompareTag("Edge"))
        {
            Destroy(gameObject);
            return;
        }
    }
}
