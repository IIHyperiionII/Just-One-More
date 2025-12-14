using UnityEngine;

public class EnemyBulletBaseController : MonoBehaviour, IBullet
{
    private Vector2 direction;
    private Rigidbody2D Rigidbody;
    public string type = "BaseEnemyBullet";
    public int speed;
    public int damage;
    public Quaternion initialRotation;
    public string GetBulletType() { return type; }
    public Quaternion GetInitialRotation() { return initialRotation; }
    public int GetSpeed() { return speed; }
    public int GetDamage() { return damage; }
    public int GetSign() { return 0; }
    private ModeAndWeaponSelection currentSelection;
    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        direction = transform.right; // applying given rotation to world x axis
        if (ModeController.Instance != null && currentSelection == null)
        {
            currentSelection = ModeController.Instance.currentSelection;
        }
    }

    public void Initialize( int bulletSpeed, int bulletDamage , Quaternion rotation)
    {
        speed = bulletSpeed;
        damage = bulletDamage;
        initialRotation = rotation;
        currentSelection = ModeController.Instance.currentSelection;
    }

    void FixedUpdate()
    {
        if (GameModeManager.timeIsPaused) return;
        // Move the bullet in the set direction
        Rigidbody.MovePosition(Rigidbody.position + direction * speed * Time.fixedDeltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy")) return;
        if (other.gameObject.CompareTag("EnemyBullet")) return;
        if (other.gameObject.CompareTag("Coin")) return;
        if (other.gameObject.CompareTag("EdgeBulletsFree")) return;
        if (other.gameObject.CompareTag("Player"))
        {
            if (currentSelection.selectedMode == GameMode.OneShot)
            {
                other.gameObject.GetComponent<PlayerController>().Die();
            } else {
                other.gameObject.GetComponent<PlayerController>().takeDamage(damage);
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

}
