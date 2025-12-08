using UnityEngine;

public class EnemyBulletScalingController : MonoBehaviour, IBullet
{
    private Vector2 direction;
    private Rigidbody2D Rigidbody;
    public string type = "ScalingEnemyBullet";
    public int speed;
    private float growth;
    public int damage;
    public Quaternion initialRotation;
    public Transform spriteTransform;
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
        if (currentSelection == null)
        {
            currentSelection = ModeController.Instance.currentSelection;
        }
    }

    public void Initialize(int bulletSpeed, int bulletDamage, Quaternion rotation)
    {
        speed = bulletSpeed;
        damage = bulletDamage;
        initialRotation = rotation;
        currentSelection = ModeController.Instance.currentSelection;
    }
    void Update()
    {
        if (GameModeManager.timeIsPaused) return;
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
}
