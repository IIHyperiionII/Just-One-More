using UnityEngine;
// This script controls the behavior of the player's bullet only for testing in editor without weapons.
public class PlayerBulletControllerTest : MonoBehaviour
{
    private Vector2 bulletPosition;
    private Vector2 direction;
    private Rigidbody2D Rigidbody;
    private int speed;
    private int damage;
    private int piercingLevel;
    private int freezeLevel;
    private int piercedEnemies = 0;
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
    public void Initialize( int bulletSpeed, int bulletDamage, int bulletPiercingLevel, int bulletFreezeLevel)
    {
        speed = bulletSpeed;
        damage = bulletDamage;
        piercingLevel = bulletPiercingLevel;
        freezeLevel = bulletFreezeLevel;
        currentSelection = ModeController.Instance.currentSelection;
    }
    void FixedUpdate()
    {
        if (GameModeManager.timeIsPaused) return;
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
                    if (freezeLevel > 0)
                    {
                        Freeze(other.gameObject);
                    }
                    DoDamage(other.gameObject);
                }
            } else
            {
                Destroy(other.gameObject);
            }
            piercedEnemies++;
            if (piercedEnemies > piercingLevel && piercingLevel != 4)
            {
                Destroy(gameObject);
            }
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
    void Freeze(GameObject target)
    {
        if (Random.Range(0,2) == 0)
        {
            target.GetComponent<IEnemy>().Freeze(0.5f * freezeLevel);
        }
    }

}
