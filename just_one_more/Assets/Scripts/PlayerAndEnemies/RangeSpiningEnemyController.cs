using System.Collections;
using UnityEngine;

public class RangeSpiningEnemyController : MonoBehaviour, IEnemy
{
    private Vector2 playerPosition;
    private Vector2 enemyPosition;
    private Vector2 direction;
    private Vector2 rotatedDirection;
    private bool isRunning = false;
    public EnemyData EnemiesData;
    private EnemyData runtimeEnemiesData;
    private Rigidbody2D Rigidbody;
    public GameObject bulletPrefab;
    private float nextAttackTime = 0f;
    private float shootingAngle = 0f;
    private int spinDirection = 0;
    private float shootingAngle2 = 180f; // Second bullet angle (opposite direction)
    public GameObject coinPrefab;
    private string enemyType = "";
    private bool isChangingSprite = false;
    private bool isFrozen = false;
    private bool isInvisible = false;
    private bool isKnockbacked = false;
    private GameObject player;
    private Transform target;
    private bool hitColorActive = false;
    private bool freezeColorActive = false;
    private Color flashColor = new Color(1f, 0.4f, 0.4f);
    private Color freezeFlashColor = new Color(0.4f, 0.4f, 1f);
    private Color originalColor = Color.white;
    public Sprite bulletSprite;
    void Awake()
    {
        spinDirection = Random.Range(0, 2) * 2 - 1; // Randomly set to -1 or 1
    }
    void Start()
    {
        runtimeEnemiesData = Instantiate(EnemiesData); // Create an instance of the EnemyData for this enemy only
        Rigidbody = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        target = player.transform.Find("WallBoundsCheck");
    }
    void FixedUpdate()
    {
        if (GameModeManager.timeIsPaused) return;
        if (isKnockbacked) return;
        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            Debug.LogError("Player does not exist in the scene.");
        } else {
            GetPositions();
            Move();
            Attack();
        }
    }
    void GetPositions()
    {
        // Get player and enemy positions for direction calculations
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        enemyPosition = transform.position;
    }
    void Move()
    {
        GetDirection();
        UpdatePosition(1);
    }
    void GetDirection()
    {
        direction = (playerPosition - enemyPosition).normalized; // Get the normalized (value is 1, it does not affect speed) direction vector towards the player
        if (!isRunning)
            StartCoroutine(UpdateMovementAngle()); // Start coroutine to update movement angle randomly
    }
    void UpdatePosition(int sign)
    {
        Vector2 movement = sign * rotatedDirection * Time.deltaTime * runtimeEnemiesData.moveSpeed; // Move in the rotated direction clockwise or counter-clockwise based on sign
        Rigidbody.MovePosition(Rigidbody.position + movement);
    }
    void Attack()
    {
        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + 2f / runtimeEnemiesData.attackSpeed; // Set next attack time based on attack speed
            shootingAngle = UpdateAngle(shootingAngle);
            shootingAngle2 = UpdateAngle(shootingAngle2);
            SpawnBullet();
        }
    }
    IEnumerator UpdateMovementAngle()
    {
        isRunning = true; // Set the flag to indicate the coroutine is running
        float randomAngle = Random.Range(-180f, 180f); // Random angle
        rotatedDirection = RotateVector(direction, randomAngle); // Rotate the movement direction vector by the random angle
        yield return new WaitForSeconds(Random.Range(0.1f, 1f)); // Wait for a random duration between 0.1 and 1 seconds (will be moving it the direction during this time)
        isRunning = false;
        yield return null; // End the coroutine
    }
    Vector2 RotateVector(Vector2 v, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad; // Convert degrees to radians
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);
        // Apply rotation matrix result is a new vector rotated by degrees counter-clockwise
        return new Vector2(
            v.x * cos - v.y * sin,
            v.x * sin + v.y * cos
        );
    }
    float UpdateAngle(float angle)
    {
        angle += 30f * spinDirection; // Increment angle by 15 degrees in the spin direction
        if (angle >= 360f) angle = 0f; // Reset angle if it exceeds 360 degrees
        if (angle <= -360f) angle = 0f; // Reset angle if it goes below -360 degrees
        return angle;
    }
    void SpawnBullet()
    {
        // Spawn the first bullet
        GameObject bullet = Instantiate(bulletPrefab, transform.position + Quaternion.Euler(0f, 0f, shootingAngle) * Vector3.right * 0.5f, Quaternion.Euler(0f, 0f, shootingAngle));
        bullet.GetComponent<EnemyBulletBaseController>().Initialize(runtimeEnemiesData.bulletSpeed, runtimeEnemiesData.damage, Quaternion.Euler(0f, 0f, shootingAngle), bulletSprite);
        bullet.transform.SetParent(GameObject.FindGameObjectWithTag("BulletParent").transform); // Set the parent of the spawned bullet for organization

        // Spawn the second bullet in the opposite direction
        GameObject bullet2 = Instantiate(bulletPrefab, transform.position + Quaternion.Euler(0f, 0f, shootingAngle2) * Vector3.right * 0.5f, Quaternion.Euler(0f, 0f, shootingAngle2));
        bullet2.GetComponent<EnemyBulletBaseController>().Initialize(runtimeEnemiesData.bulletSpeed, runtimeEnemiesData.damage, Quaternion.Euler(0f, 0f, shootingAngle2), bulletSprite);
        bullet2.transform.SetParent(GameObject.FindGameObjectWithTag("BulletParent").transform); // Set the parent of the spawned bullet for organization
    }
    void OnDestroy()
    {
        if (!gameObject.scene.isLoaded) return; // Ensure the game object is still part of a loaded scene (scene is not ending) before instantiating the coin
        GameObject coinInstance = Instantiate(coinPrefab, transform.position, Quaternion.identity);  // Spawn a coin at the enemy's position upon destruction
        coinInstance.GetComponent<CoinController>().SetValue(runtimeEnemiesData.value);
    }
    public EnemyData GetEnemyData()
    {
        if (runtimeEnemiesData == null)
        {
            runtimeEnemiesData = Instantiate(EnemiesData);
        }
        return runtimeEnemiesData;
    }
    public Transform GetTransform()
    {
        return transform;
    }
    public void SetEnemyType(string type)
    {
        enemyType = type;
    }
    public string GetEnemyType()
    {
        return enemyType;
    }
    public void TakeDamage(int damage)
    {
        if (ModeController.Instance != null && ModeController.Instance.currentSelection.selectedMode == GameMode.OneShot)
        {
            Destroy(gameObject);
            return;
        }
        runtimeEnemiesData.hp -= damage;
        if (runtimeEnemiesData.hp <= 0)
        {
            Destroy(gameObject);
        }
        HitColorChange();
        if (GameManager.Instance.runtimePlayerData.needToGamble > 70 && Random.Range(0, 100) < 20 && !isChangingSprite)
        {
            Sprite newSprite = GameManager.Instance.GetRandomSprite(GetComponent<SpriteRenderer>().sprite);
            StartCoroutine(SpriteChange(newSprite));
        }
        if (GameManager.Instance.runtimePlayerData.needToGamble > 70 && Random.Range(0, 100) < 20 && !isInvisible)
        {
            StartCoroutine(BecomeInvisible());
        }
    }
    void HitColorChange()
    {
        if (hitColorActive) return;
        StartCoroutine(HitColor());
    }
    IEnumerator HitColor()
    {
        hitColorActive = true;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = flashColor; // Change color to red on hit
        yield return new WaitForSeconds(0.2f);
        if (isFrozen)
        {
            spriteRenderer.color = freezeFlashColor; // Keep freeze color if frozen
        } else {
            spriteRenderer.color = originalColor; // Restore original color
        }
        hitColorActive = false;
    }
    void FreezeColorChange(float duration)
    {
        if (freezeColorActive) return;
        StartCoroutine(FreezeColor(duration));
    }
    IEnumerator FreezeColor(float duration)
    {
        freezeColorActive = true;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = freezeFlashColor; // Change color to blue on freeze
        yield return new WaitForSeconds(duration);
        spriteRenderer.color = originalColor; // Restore original color
        freezeColorActive = false;
    }
    public void Freeze(float duration)
    {
        if (isFrozen) return;
        StartCoroutine(FreezeCoroutine(duration));
    }
    IEnumerator FreezeCoroutine(float duration)
    {
        isFrozen = true;
        FreezeColorChange(duration);
        int original = runtimeEnemiesData.moveSpeed;
        if (duration == 2)
        {
            runtimeEnemiesData.moveSpeed = 0;
        } else
        {
            runtimeEnemiesData.moveSpeed /= 2;
        }
        
        yield return new WaitForSeconds(duration);

        if (duration == 2)
        {
            runtimeEnemiesData.moveSpeed = original;
        } else
        {
            runtimeEnemiesData.moveSpeed *= 2;
        }
        isFrozen = false;
    }
    public void Knockback(float time)
    {
        StartCoroutine(KnockbackCoroutine(time));
    }
    IEnumerator KnockbackCoroutine(float time)
    {
        isKnockbacked = true;
        GetDirections(0f);
        Vector2 knockbackDirection = GetDirectionToPlayer(); // Get direction away from player
        Rigidbody.AddForce(knockbackDirection * 100 * -1, ForceMode2D.Impulse);
        yield return new WaitForSeconds(time); // Duration of knockback effect
        Rigidbody.linearVelocity = Vector2.zero;
        isKnockbacked = false;
    }
    
    IEnumerator SpriteChange(Sprite newSprite)
    {
        Debug.Log("Changing sprite to " + newSprite.name);
        isChangingSprite = true;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Sprite originalSprite = spriteRenderer.sprite;
        spriteRenderer.sprite = newSprite;
        yield return new WaitForSeconds(2f);
        spriteRenderer.sprite = originalSprite;
        isChangingSprite = false;
    }
    IEnumerator BecomeInvisible()
    {
        Debug.Log("Becoming invisible");
        isInvisible = true;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f); // Set alpha to 0.0 for invisibility
        yield return new WaitForSeconds(3f);
        spriteRenderer.color = originalColor; // Restore original color
        isInvisible = false;
    }

    public Vector2 GetDirectionToPlayer()
    {
        return direction;
    }
    void GetDirections(float offset)
    {
        playerPosition = target.position;
        playerPosition.y -= offset;
        enemyPosition = transform.position;
        direction = (playerPosition - enemyPosition).normalized; // Get the normalized (value is 1, it does not affect speed) direction vector towards the player
    }



}
