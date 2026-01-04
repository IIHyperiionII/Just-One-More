using UnityEngine;
using UnityEngine.PlayerLoop;
using System.Collections;

public class RangeWaveEnemyController : MonoBehaviour, IEnemy
{
    private Vector2 playerPosition;
    private Vector2 lastPlayerPosition;
    private Vector2 enemyPosition;
    private Vector2 direction;
    private Vector2 playerDirection;
    public EnemyData EnemiesData;
    private EnemyData runtimeEnemiesData;
    private Rigidbody2D Rigidbody;
    public GameObject bulletPrefab;
    public GameObject coinPrefab;
    private float nextAttackTime = 0f;
    private int sign = 0;
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
    public Sprite[] bulletSprites;
    private int bulletSpriteIndex = 0;
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
        // Get and store the player and enemy positions for direction calculations
        lastPlayerPosition = playerPosition;
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        enemyPosition = transform.position;
    }

    void Move()
    {
        float dotProduct = GetDotProduct(); // Calculate the dot product between the player's movement direction and the direction to the enemy
        if (dotProduct > 0.5f)
        {
            UpdatePosition(-1);
        }
        else if (dotProduct < -0.5f)
        {
            UpdatePosition(1);
        }
    }
    void UpdatePosition(int sign)
    {
        float distanceToPlayer = Vector2.Distance(playerPosition, enemyPosition);
        if (distanceToPlayer < 10f && sign == 1) return; // Do not move closer if within 10 units
        if (distanceToPlayer > 20f && sign == -1) return; // Do not move away if beyond 20 units
        Vector2 movement = sign * direction * Time.deltaTime * runtimeEnemiesData.moveSpeed;
        Rigidbody.MovePosition(Rigidbody.position + movement);
    }
    float GetDotProduct()
    {
        direction = (playerPosition - enemyPosition).normalized; // Direction vector towards the player
        playerDirection = (playerPosition - lastPlayerPosition).normalized; // Player's movement direction
        float dotProduct = Vector2.Dot(playerDirection, -direction); // Calculate the dot product between the two directions (1 = same direction, -1 = opposite directions)
        return dotProduct;
    }
    void Attack()
    {
        if (Time.time >= nextAttackTime)
        { 
            nextAttackTime = Time.time + 1.5f / runtimeEnemiesData.attackSpeed; // Set the next attack time based on attack speed
            Quaternion rotation = UpdateAngle();
            SpawnBullet(rotation);
        }
    }
    Quaternion UpdateAngle()
    {
        Vector2 aimDirection = (playerPosition - enemyPosition).normalized; // Direction vector towards the player
        float tmpAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg; // Calculate the angle in degrees from radians
        Quaternion angle = Quaternion.Euler(0f, 0f, tmpAngle); // Create a quaternion representing the rotation
        return angle;
    }
    void SpawnBullet(Quaternion rotation)
    {
        sign = Random.Range(0, 2) * 2 - 1; // Randomly choose between -1 and 1 for bullet wave direction starting sign
        GameObject bullet = Instantiate(bulletPrefab, transform.position + rotation * Vector3.right * 0.5f, rotation);
        bullet.GetComponent<EnemyBulletWaveController>().Initialize(runtimeEnemiesData.bulletSpeed, runtimeEnemiesData.damage, sign, rotation, bulletSprites[bulletSpriteIndex]); // Initialize the wave bullet with speed, damage, wave direction sign, and rotation
        bullet.transform.SetParent(GameObject.FindGameObjectWithTag("BulletParent").transform); // Set the parent of the spawned bullet for organization
        bulletSpriteIndex = (bulletSpriteIndex + 1) % bulletSprites.Length; // Cycle through bullet sprites for variety
    }
    void OnDestroy()
    {
        if (!gameObject.scene.isLoaded) return; // Ensure the game object is still part of a loaded scene (scene is not ending) before instantiating the coin
        GameObject coinInstance = Instantiate(coinPrefab, transform.position, Quaternion.identity); // Spawn a coin at the enemy's position upon destruction
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
        // Handle OneShot mode: destroy enemy immediately
        if (ModeController.Instance != null && ModeController.Instance.currentSelection.selectedMode == GameMode.OneShot)
        {
            Destroy(gameObject);
            return;
        }
        runtimeEnemiesData.hp -= damage;
        if (runtimeEnemiesData.hp <= 0)
        {
            EnemySoundHelper.PlayDeathSound(enemyType);
            
            Destroy(gameObject);
        }
        HitColorChange();
        // Chance to change sprite or become invisible based on player's needToGamble stat
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
    // Handle color change on hit
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
    // Handle color change on freeze
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
    // Freeze the enemy for a duration
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
        // If duration is 2 seconds, completely freeze the enemy, else slow down by half
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
    // Apply knockback effect to the enemy
    public void Knockback(float time)
    {
        if (isKnockbacked) return;
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
    // Change sprite temporarily
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
    // Become invisible temporarily
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

