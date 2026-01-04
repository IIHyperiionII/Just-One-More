using System.Security.Cryptography;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyBulletWaveController : MonoBehaviour, IBullet
{
    private Vector2 waveMovement;
    private Vector2 motionForward;
    private Vector2 startPosition;
    private Vector2 direction;
    private Rigidbody2D Rigidbody;
    public string type = "WaveEnemyBullet";
    public int speed;
    public int damage;
    public Quaternion initialRotation;
    private float timeAlive = 0f;
    public int sign;
    private float frequency = 0f;
    private float amplitude = 0f;
    public string GetBulletType() { return type; }
    public Quaternion GetInitialRotation() { return initialRotation; }
    public int GetSpeed() { return speed; }
    public int GetDamage() { return damage; }
    public int GetSign() { return sign; }
    private ModeAndWeaponSelection currentSelection;
    private SpriteRenderer spriteRenderer;
    private Sprite bulletSprite;
    private string spriteName;
    private BoxCollider2D boxCollider;

    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        direction = transform.right; // applying given rotation to world x axis
        startPosition = Rigidbody.position;
        frequency = UnityEngine.Random.Range(5f, 15f); // random frequency for wave motion
        amplitude = UnityEngine.Random.Range(0.5f, 5f); // random amplitude for wave motion
        if (currentSelection == null)
        {
            currentSelection = ModeController.Instance.currentSelection;
        }
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.transform.rotation = initialRotation * Quaternion.Euler(0, 0, 90); // Adjust sprite orientation
        if (bulletSprite != null)
        {
            spriteRenderer.sprite = bulletSprite;
        }
    }

    void Start()
    {
        spriteRenderer.transform.rotation = initialRotation * Quaternion.Euler(0, 0, 90); // Adjust sprite orientation
    }

    // Initialize bullet with speed, damage, sign, rotation, and sprite
    public void Initialize(int bulletSpeed, int bulletDamage, int bulletSign, Quaternion rotation, Sprite sprite)
    {
        speed = bulletSpeed;
        damage = bulletDamage;
        sign = bulletSign;
        initialRotation = rotation;
        currentSelection = ModeController.Instance.currentSelection;
        bulletSprite = sprite;
        spriteName = sprite.name;
        boxCollider.size = bulletSprite.bounds.size;
    }
    void FixedUpdate()
    {
        if (GameModeManager.timeIsPaused) return;
        // Update sprite if it has changed
        if (spriteRenderer.sprite != bulletSprite && bulletSprite != null)
        {
            spriteRenderer.sprite = bulletSprite;
            spriteName = bulletSprite.name;
            spriteRenderer.transform.rotation = initialRotation * Quaternion.Euler(0, 0, 90);
            boxCollider.size = bulletSprite.bounds.size;
        }
        GetWaveMovement();
            Rigidbody.MovePosition(motionForward + waveMovement);
    }
    void GetWaveMovement()
    {
        // calculate functional value of sine wave based on time alive
        timeAlive += Time.deltaTime;
        motionForward = direction * speed * timeAlive + startPosition; // position of linear forward motion
        float offset = Mathf.Sin(timeAlive * frequency) * amplitude; // offset from linear motion based on sine wave
        Vector2 sideAxis = new Vector2(-direction.y, direction.x); // perpendicular (90˚) axis to direction of travel, for wave motion
        waveMovement = sideAxis * offset * sign; // apply offset to side axis, multiplied by sign to determine left/right wave motion
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
