using System.Collections;
using UnityEngine;

public class MeleeEnemyController : MonoBehaviour, IEnemy
{
    private Vector2 playerPosition;
    private Vector2 enemyPosition;
    private Vector2 direction;
    public EnemyData EnemiesData;
    private EnemyData runtimeEnemiesData;
    private Rigidbody2D Rigidbody;
    public GameObject coinPrefab;
    private float nextAttackTime = 0f;
    private string enemyType;
    private bool isChangingSprite = false;
    private bool isInvisible = false;
    void Start()
    {
        if (runtimeEnemiesData == null){
        runtimeEnemiesData = Instantiate(EnemiesData); // Create an instance of the EnemyData for this enemy only
        }
        Rigidbody = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        if (GameModeManager.playerInCasino) return;
        Move();
    }
    void Move()
    {
        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            Debug.LogError("Player does not exist in the scene.");
        } else {
            GetDirections();
            Vector2 movement = direction * Time.deltaTime * runtimeEnemiesData.moveSpeed;
            Rigidbody.MovePosition(Rigidbody.position + movement);
        }
    }
    void GetDirections()
    {
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        enemyPosition = transform.position;
        direction = (playerPosition - enemyPosition).normalized; // Get the normalized (value is 1, it does not affect speed) direction vector towards the player
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        // Check if the collided object has the "Player" tag
        if (other.gameObject.CompareTag("Player") && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + runtimeEnemiesData.attackSpeed;

            other.gameObject.GetComponent<PlayerController>().takeDamage(runtimeEnemiesData.damage);
        }
    }
    void OnCollisionStay2D(Collision2D other)
    {
        // Check if the collided object that is staying in contact has the "Player" tag
        if (other.gameObject.CompareTag("Player") && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + runtimeEnemiesData.attackSpeed;
            other.gameObject.GetComponent<PlayerController>().takeDamage(runtimeEnemiesData.damage);
        }
    }
    void OnDestroy()
    {
        // Ensure the game object is still part of a loaded scene (scene is not ending) before instantiating the coin
        if (!gameObject.scene.isLoaded) return;
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
        if (GameManager.Instance.runtimePlayerData.needToGamble > 70 && Random.Range(0, 100) < 20 && !isInvisible)
        {
            StartCoroutine(BecomeInvisible());
        }
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


}
