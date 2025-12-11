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
    private ModeAndWeaponSelection currentSelection;
    private GameObject player;
    private Transform target;
    private bool isFrozen = false;
    private bool isKnockbacked = false;
    private bool hitColorActive = false;
    private bool freezeColorActive = false;
    private Color flashColor = new Color(1f, 0.4f, 0.4f);
    private Color freezeFlashColor = new Color(0.4f, 0.4f, 1f);
    private Color originalColor = Color.white;

    void Start()
    {
        if (runtimeEnemiesData == null){
        runtimeEnemiesData = Instantiate(EnemiesData); // Create an instance of the EnemyData for this enemy only
        }
        Debug.Log("Enemy AS: " + runtimeEnemiesData.attackSpeed);
        Rigidbody = GetComponent<Rigidbody2D>();
        if (currentSelection == null)
        {
            currentSelection = ModeController.Instance.currentSelection;
        }
        player = GameObject.FindGameObjectWithTag("Player");
        target = player.transform.Find("WallBoundsCheck");
    }
    void FixedUpdate()
    {
        if (GameModeManager.timeIsPaused) return;
        if (isKnockbacked) return;
        Move();
        
        Debug.Log("Is frozen:" + isFrozen);
    }
    void Move()
    {
        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            Debug.LogError("Player does not exist in the scene.");
        } else {
            GetDirections(0.8f);
            Vector2 movement = direction * Time.deltaTime * runtimeEnemiesData.moveSpeed;
            Rigidbody.MovePosition(Rigidbody.position + movement);
        }
    }

    void GetDirections(float offset)
    {
        playerPosition = target.position;
        playerPosition.y -= offset;
        enemyPosition = transform.position;
        direction = (playerPosition - enemyPosition).normalized; // Get the normalized (value is 1, it does not affect speed) direction vector towards the player
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        // Check if the collided object has the "Player" tag
        if (other.gameObject.CompareTag("Player") && Time.time >= nextAttackTime)
        {    
            nextAttackTime = Time.time + 2f / runtimeEnemiesData.attackSpeed;
            if (currentSelection.selectedMode == GameMode.OneShot)
            {
                other.gameObject.GetComponent<PlayerController>().Die();
            } else {
                other.gameObject.GetComponent<PlayerController>().takeDamage(runtimeEnemiesData.damage);
            }
        }
    }
    void OnCollisionStay2D(Collision2D other)
    {
        // Check if the collided object that is staying in contact has the "Player" tag
        if (other.gameObject.CompareTag("Player") && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + 2f / runtimeEnemiesData.attackSpeed;
            if (currentSelection.selectedMode == GameMode.OneShot)
            {
                other.gameObject.GetComponent<PlayerController>().Die();
            } else {
                other.gameObject.GetComponent<PlayerController>().takeDamage(runtimeEnemiesData.damage);
            }
        }
    }
    void OnDestroy()
    {
        // Ensure the game object is still part of a loaded scene (scene is not ending) before instantiating the coin
        if (!gameObject.scene.isLoaded) return;
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
