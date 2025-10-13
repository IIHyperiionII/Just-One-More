using UnityEngine;

public class BulletControllerTest : MonoBehaviour
{
    private Vector2 playerPosition;

    private Vector2 bulletPosition;
    private Vector2 direction;
    private Rigidbody2D Rigidbody;
    private float speed;

    private int damage;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        bulletPosition = transform.position;
        direction = (playerPosition - bulletPosition).normalized;
    }

    public void Initialize( float bulletSpeed, int bulletDamage)
    {
        speed = bulletSpeed;
        damage = bulletDamage;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (playerPosition != null)
        {
            // Move the bullet towards the player's position
            Vector2 movement = direction * Time.deltaTime * speed;
            Rigidbody.MovePosition(Rigidbody.position + movement);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy")) return;
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController>().takeDamage(10);
        }
        
        Destroy(gameObject);
    }

}
