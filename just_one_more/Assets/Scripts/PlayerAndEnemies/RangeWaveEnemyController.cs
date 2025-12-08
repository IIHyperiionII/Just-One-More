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
    void Start()
    {
        runtimeEnemiesData = Instantiate(EnemiesData); // Create an instance of the EnemyData for this enemy only
        Rigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (GameModeManager.timeIsPaused) return;
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
            nextAttackTime = Time.time + 1 / runtimeEnemiesData.attackSpeed; // Set the next attack time based on attack speed
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
        GameObject bullet = Instantiate(bulletPrefab, transform.position, rotation);
        bullet.GetComponent<EnemyBulletWaveController>().Initialize(runtimeEnemiesData.bulletSpeed, runtimeEnemiesData.damage, sign, rotation); // Initialize the wave bullet with speed, damage, wave direction sign, and rotation
        bullet.transform.SetParent(GameObject.FindGameObjectWithTag("BulletParent").transform); // Set the parent of the spawned bullet for organization
    }
    void OnDestroy()
    {
        if (!gameObject.scene.isLoaded) return; // Ensure the game object is still part of a loaded scene (scene is not ending) before instantiating the coin
        Instantiate(coinPrefab, transform.position, Quaternion.identity); // Spawn a coin at the enemy's position upon destruction
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
        runtimeEnemiesData.hp -= damage;
        if (runtimeEnemiesData.hp <= 0)
        {
            Destroy(gameObject);
        }
        if (GameManager.Instance.runtimePlayerData.needToGamble > 70 && Random.Range(0, 100) < 20 && !isChangingSprite)
        {
            Sprite newSprite = GameManager.Instance.GetRandomSprite(GetComponent<SpriteRenderer>().sprite);
            StartCoroutine(SpriteChange(newSprite));
        }
    }
    public void Freeze(float duration)
    {
        if (isFrozen) return;
        StartCoroutine(FreezeCoroutine(duration));
    }
    IEnumerator FreezeCoroutine(float duration)
    {
        isFrozen = true;
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
    IEnumerator SpriteChange(Sprite newSprite)
    {
        isChangingSprite = true;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Sprite originalSprite = spriteRenderer.sprite;
        spriteRenderer.sprite = newSprite;
        yield return new WaitForSeconds(2f);
        spriteRenderer.sprite = originalSprite;
        isChangingSprite = false;
    }
}
