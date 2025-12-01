using UnityEngine;
// This script controls the behavior of the player's bullet only for testing in editor without weapons.
public class Bullet : MonoBehaviour
{
    private Vector2 bulletPosition;
    private Vector2 direction;
    private Rigidbody2D Rigidbody;
    private int speed;
    private int damage;
    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        direction = transform.right; // applying given rotation to world x axis
    }
    public void Initialize( int bulletSpeed, int bulletDamage)
    {
        speed = bulletSpeed;
        damage = bulletDamage;
    }
    void FixedUpdate()
    {
        if (GameModeManager.playerInCasino) return;
        Rigidbody.MovePosition(Rigidbody.position + direction * speed * Time.fixedDeltaTime);
        
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")) return;
        if (other.gameObject.CompareTag("PlayerBullet")) return;
        if (other.gameObject.CompareTag("Coin")) return;
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("EnemyBullet"))
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                DoDamage(other.gameObject);
            } else
            {
                Destroy(other.gameObject);
            }
            Destroy(gameObject);
            return;
        }
        if (other.gameObject.CompareTag("Edge"))
        {
            Destroy(gameObject);
            return;
        }
    }

    void DoDamage(GameObject target)
    {
        target.GetComponent<IEnemy>().TakeDamage(damage);
    }

}
