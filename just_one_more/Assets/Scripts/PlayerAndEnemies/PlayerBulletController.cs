using UnityEngine;
// This script controls the behavior of the player's bullet only for testing in editor without weapons.
public class PlayerBulletController : MonoBehaviour, IBulletPlayer
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
    private Sprite bulletSprite;
    private SpriteRenderer spriteRenderer;
    private string spriteName;
    private BoxCollider2D boxCollider;
    private bool isPistol;
    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        direction = transform.right; // applying given rotation to world x axis
        if (currentSelection == null)
        {
            currentSelection = ModeController.Instance.currentSelection;
        }
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.transform.rotation = transform.rotation * Quaternion.Euler(0, 0, 90);
        if (bulletSprite != null)
        {
            spriteRenderer.sprite = bulletSprite;
        }
    }

    public void Initialize( int bulletSpeed, int bulletDamage, int bulletPiercingLevel, int bulletFreezeLevel, Quaternion rotation, Sprite sprite)
    {
        speed = bulletSpeed;
        damage = bulletDamage;
        piercingLevel = bulletPiercingLevel;
        freezeLevel = bulletFreezeLevel;
        transform.rotation = rotation;
        bulletSprite = sprite;
        spriteName = sprite.name;
        boxCollider.size = bulletSprite.bounds.size;
    }
    void FixedUpdate()
    {
        if (GameModeManager.timeIsPaused) return;
        if (currentSelection == null)
        {
            currentSelection = ModeController.Instance.currentSelection;
        }
        if (spriteRenderer.sprite != bulletSprite && bulletSprite != null)
        {
            spriteRenderer.sprite = bulletSprite;
            spriteName = bulletSprite.name;
            spriteRenderer.transform.rotation = transform.rotation * Quaternion.Euler(0, 0, 90);
            boxCollider.size = bulletSprite.bounds.size;
        }
        float moveSpeed = speed;
        if (speed > 10)
        {
            moveSpeed = 10 + 2 * Mathf.Log(speed - 10, 2);
        }
        Rigidbody.MovePosition(Rigidbody.position + direction * moveSpeed * Time.fixedDeltaTime);
        
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")) return;
        if (other.gameObject.CompareTag("PlayerBullet")) return;
        if (other.gameObject.CompareTag("Coin")) return;
        if (other.gameObject.CompareTag("EdgeBulletsFree")) return;
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
    public int GetSpeed() { return speed; }
    public int GetDamage() { return damage; }
    public Quaternion GetInitialRotation() { return transform.rotation; }
    public int GetFreezeLevel() { return freezeLevel; }
    public int GetPiercingLevel() { return piercingLevel; }
    public string GetSpriteName() { return spriteName; }

}
