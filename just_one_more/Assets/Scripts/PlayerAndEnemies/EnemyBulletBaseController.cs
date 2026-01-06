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
    private Sprite bulletSprite;
    private SpriteRenderer spriteRenderer;
    private string spriteName;
    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        direction = transform.right; // applying given rotation to world x axis
        if (ModeController.Instance != null && currentSelection == null)
        {
            currentSelection = ModeController.Instance.currentSelection;
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.transform.rotation = initialRotation * Quaternion.Euler(0, 0, 90); // Adjust sprite orientation
        if (bulletSprite != null)
        {
            spriteRenderer.sprite = bulletSprite;
        }
    }

    // Initialize bullet with speed, damage, rotation, and sprite
    public void Initialize( int bulletSpeed, int bulletDamage , Quaternion rotation, Sprite sprite)
    {
        speed = bulletSpeed;
        damage = bulletDamage;
        initialRotation = rotation;
        currentSelection = ModeController.Instance.currentSelection;
        bulletSprite = sprite;
        spriteName = sprite.name;
    }
    void Start()
    {
        spriteRenderer.transform.rotation = initialRotation * Quaternion.Euler(0, 0, 90); // Adjust sprite orientation
    }

    void FixedUpdate()
    {
        if (GameModeManager.timeIsPaused) return;
        // Update sprite if it has changed
        if (spriteRenderer.sprite != bulletSprite && bulletSprite != null)
        {
            spriteRenderer.sprite = bulletSprite;
            spriteRenderer.transform.rotation = initialRotation * Quaternion.Euler(0, 0, 90);
            spriteName = bulletSprite.name;
        }
        // Move the bullet in the set direction
        Rigidbody.MovePosition(Rigidbody.position + direction * speed * Time.fixedDeltaTime);
    }

    // Handle collisions
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

    public string GetSpriteName()
    {
        return spriteName;
    }

}
