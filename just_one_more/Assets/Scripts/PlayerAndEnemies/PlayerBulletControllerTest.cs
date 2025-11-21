using UnityEngine;
// This script controls the behavior of the player's bullet only for testing in editor without weapons.
public class PlayerBulletControllerTest : MonoBehaviour
{
    private Vector2 bulletPosition;
    private Vector2 direction;
    private Rigidbody2D Rigidbody;
    private int speed;
    private int damage;
    private ModeAndWeaponSelection currentSelection;
    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        direction = transform.right; // applying given rotation to world x axis
        if (currentSelection == null)
        {
            currentSelection = ModeController.Instance.currentSelection;
        }
    }
    public void Initialize( int bulletSpeed, int bulletDamage)
    {
        speed = bulletSpeed;
        damage = bulletDamage;
        currentSelection = ModeController.Instance.currentSelection;
    }
    void FixedUpdate()
    {
        
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
                if (currentSelection.selectedMode == GameMode.OneShot)
                {
                    Destroy(other.gameObject);
                } else {
                    DoDamage(other.gameObject);
                }
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
