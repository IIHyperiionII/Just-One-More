using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private PlayerData PlayerData;
    private Rigidbody2D Rigidbody;
    private Vector2 input;
    private Vector3 mousePosition;
    public bool MouseKeyHoldDown = false;
    private float nextAttackTime = 0f;
    public GameObject bulletPrefab;
    private bool isDashing = false;
    private bool dashReset = true;
    private Vector2 dashDir;
    private int numberOfDashes = 2;
    private Vector2 lastPosition;
    private Vector2 lastHandPosition;
    private int shieldRequests = 0;
    public Vector2 MovementVector => input * PlayerData.moveSpeed;

    void Start()
    {
        PlayerData = GameManager.Instance.runtimePlayerData; // Access the runtime player data from GameManager
        Rigidbody = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        GetMovementInput();
        if (PlayerData.dashLevel > 0)
        {
            GetDashInput();
        }
        GetAttackInput(); // Just for testing will be removed after weapons are implemented
        Attack(); // Just for testing will be removed after weapons are implemented
    }

    void FixedUpdate()
    {
        if (isDashing) return; // Skip normal movement while dashing
        Vector2 movement = input * Time.deltaTime * PlayerData.moveSpeed;
        Rigidbody.MovePosition(Rigidbody.position + movement);
        
    }

    void GetMovementInput()
    {
        input = new Vector2(0, 0);
        input.x = Input.GetAxis("Horizontal"); // Get horizontal input (A/D or Left/Right arrows)
        input.y = Input.GetAxis("Vertical"); // Get vertical input (W/S or Up/Down arrows)
        if (input.magnitude > 1) input.Normalize(); // Normalize to prevent faster diagonal movement
    }

    void GetDashInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isDashing && numberOfDashes > 0 && dashReset)
        {
            StartCoroutine(Dash());
            StartCoroutine(ResetDash());
        }
    }

    IEnumerator Dash()
    {
        lastPosition = Rigidbody.position;
        Transform handAnchor = transform.Find("HandAnchor");
        if (handAnchor == null)
        {
            Debug.LogError("HandAnchor not found!");
            yield return null;
        }

        Transform hand = handAnchor.Find("Hand");
        if (hand == null)
        {
            Debug.LogError("Hand not found under HandAnchor!");
            yield return null;
        }
        lastHandPosition = hand.position;

        isDashing = true;
        if (PlayerData.dashLevel == 4) numberOfDashes--;
        if (numberOfDashes > 0 && PlayerData.dashLevel == 4)
        {
            dashReset = true;
        }
        else
        {
            dashReset = false;
        }
        Vector2 start = Rigidbody.position;
        dashDir = input;
        if (dashDir == Vector2.zero)
            dashDir = Vector2.right; // default direction

        dashDir.Normalize();

        Vector2 target = start + dashDir * 6f; // Dash distance of 20 units
        float elapsed = 0f;

        GameObject dashClone1 = CreateDashClone(1f, lastPosition, lastHandPosition);
        dashClone1.SetActive(false);
        GameObject dashClone2 = CreateDashClone(2f, lastPosition + dashDir * 2f, lastHandPosition + dashDir * 2f);
        dashClone2.SetActive(false);
        GameObject dashClone3 = CreateDashClone(3f, lastPosition + dashDir * 4f, lastHandPosition + dashDir * 4f);
        dashClone3.SetActive(false);
        StartCoroutine(CloneGeneration(dashClone1, dashClone2, dashClone3));
        while (elapsed < 0.2f) // Dash duration of 0.2 seconds
        {
            elapsed += Time.deltaTime;
            float t = elapsed / 0.2f;
            Vector2 newPos = Vector2.Lerp(start, target, t);
            Rigidbody.MovePosition(newPos);
            yield return new WaitForFixedUpdate(); // match physics updates
        }
        isDashing = false;
    }

    GameObject CreateDashClone(float index, Vector2 position, Vector2 handPosition)
    {
        GameObject dashClone = new GameObject("DashClone" + index);
        dashClone.transform.position = position;
        dashClone.transform.rotation = transform.rotation;
        Transform handAnchor = transform.Find("HandAnchor");
        if (handAnchor == null)
        {
            Debug.LogError("HandAnchor not found!");
            return null;
        }

        Transform hand = handAnchor.Find("Hand");
        if (hand == null)
        {
            Debug.LogError("Hand not found under HandAnchor!");
            return null;
        }
        GameObject cloneHand = new GameObject("Hand" + index);
        cloneHand.transform.position = handPosition;
        cloneHand.transform.rotation = hand.rotation;
        SpriteRenderer originalSprite = GetComponent<SpriteRenderer>();
        SpriteRenderer cloneSprite = dashClone.AddComponent<SpriteRenderer>();
        cloneSprite.sprite = originalSprite.sprite;
        SpriteRenderer originalHandSprite = hand.GetComponent<SpriteRenderer>();
        SpriteRenderer cloneHandSprite = cloneHand.AddComponent<SpriteRenderer>();
        cloneHandSprite.sprite = originalHandSprite.sprite;
        cloneHand.transform.parent = dashClone.transform;

        cloneSprite.color = new Color(((index - 1f)/3f), ((index - 1f)/3f), ((index - 1f)/3f), 0.5f);
        cloneHandSprite.color = new Color(((index - 1f)/3f), ((index - 1f)/3f), ((index - 1f)/3f), 0.5f);

        return dashClone;
    }
    IEnumerator ResetDash()
    {
        if (PlayerData.dashLevel < 4)
        {
            yield return new WaitForSeconds(20f - PlayerData.dashLevel * 5f);
            dashReset = true;
        }
        else
        {
            yield return new WaitForSeconds(5f); // Dash cooldown of 5 seconds
            numberOfDashes++;
            dashReset = true;
        }
    }
    IEnumerator CloneGeneration(GameObject dashClone1, GameObject dashClone2, GameObject dashClone3)
    {
        dashClone1.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        dashClone2.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        dashClone3.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        Destroy(dashClone1);
        yield return new WaitForSeconds(0.05f);
        Destroy(dashClone2);
        yield return new WaitForSeconds(0.05f);
        Destroy(dashClone3);
        yield return null;
    }
    void GetAttackInput()
    {
        // Switch for when mouse button is held down
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            MouseKeyHoldDown = true;
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            MouseKeyHoldDown = false;
        }
    }
    void Attack()
    {
        if (MouseKeyHoldDown && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + 1f / PlayerData.attackSpeed;

            Quaternion rotation = UpdateAngle();
            GameObject bullet = Instantiate(bulletPrefab, transform.position, rotation); // Spawn bullet at player position with calculated rotation
            bullet.GetComponent<PlayerBulletControllerTest>().Initialize(PlayerData.bulletSpeed, PlayerData.damage); // Initialize bullet with player stats
        }
    }
    Quaternion UpdateAngle()
    {
        float distanceZ = Mathf.Abs(Camera.main.transform.position.z); // Distance from camera to player on Z axis
        mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceZ)); // Convert mouse position to world position
        Vector2 aimDirection = (mousePosition - transform.position).normalized; // Get normalized direction vector from player to mouse position
        float tmpAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg; // Calculate angle in degrees from radians
        Quaternion angle = Quaternion.Euler(0f, 0f, tmpAngle); // Create rotation quaternion from angle
        return angle;
    }

    public void takeDamage(int damage)
    {
        int blockChance = 0;
        if (PlayerData.blockLevel > 0 && PlayerData.blockLevel < 4)
        {
            blockChance = 2 + 3 * PlayerData.blockLevel;
        }
        else if (PlayerData.blockLevel >= 4)
        {
            blockChance = 15;
        }
        int randomValue = Random.Range(1, 101); // Random value between 1 and 100
        if (randomValue <= blockChance)
        {
            // Block successful, no damage taken 
            StartCoroutine(ShieldUp());
            return;
        }
        PlayerData.hp -= damage;
        if (PlayerData.hp <= 0) Die();
        CameraController.ShakeCamera();
        StartCoroutine(HitColor());
    }
    IEnumerator ShieldUp()
    {
        GameObject shield = transform.Find("Shield").gameObject;
        shield.SetActive(true);
        shieldRequests++;
        yield return new WaitForSeconds(0.2f);
        if (shieldRequests > 1)
        {
            shieldRequests--;
        }
        else
        {
            shield.SetActive(false);
            shieldRequests = 0;
        }
    }

    IEnumerator HitColor()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        Transform handAnchor = transform.Find("HandAnchor");
        if (handAnchor == null)
        {
            Debug.LogError("HandAnchor not found!");
            yield break;
        }

        Transform hand = handAnchor.Find("Hand");
        if (hand == null)
        {
            Debug.LogError("Hand not found under HandAnchor!");
            yield break;
        }

        SpriteRenderer handSpriteRenderer = hand.GetComponent<SpriteRenderer>();
        if (handSpriteRenderer == null)
        {
            Debug.LogError("Hand does not have a SpriteRenderer!");
            yield break;
        }

        Color originalColor = spriteRenderer.color;
        Color flashColor = new Color(1f, 0.4f, 0.4f);

        spriteRenderer.color = flashColor;
        handSpriteRenderer.color = flashColor;

        yield return new WaitForSeconds(0.2f);

        spriteRenderer.color = originalColor;
        handSpriteRenderer.color = originalColor;
    }


        void Die()
    {
        PlayerData.isDead = true;
        Destroy(gameObject);
        SceneManager.LoadScene("MainMenuScene");
    }
    public void GetCoin(int amount)
    {
        PlayerData.money += amount;
    }

    public void GetSaveData()
    {
        SaveData data = SaveSystem.Instance.currentSaveData;
        PlayerSaveData playerData = new PlayerSaveData();
        playerData.position = transform.position;
        playerData.hp = PlayerData.hp;
        playerData.moveSpeed = PlayerData.moveSpeed;
        playerData.attackSpeed = PlayerData.attackSpeed;
        playerData.damage = PlayerData.damage;
        playerData.money = PlayerData.money;
        playerData.bulletSpeed = PlayerData.bulletSpeed;
        playerData.knockback = PlayerData.knockback;
        playerData.isDead = PlayerData.isDead;
        playerData.isMelee = PlayerData.isMelee;
        playerData.piercingLevel = PlayerData.piercingLevel;
        playerData.dashLevel = PlayerData.dashLevel;
        playerData.hpRegenLevel = PlayerData.hpRegenLevel;
        playerData.blockLevel = PlayerData.blockLevel;
        playerData.freezeLevel = PlayerData.freezeLevel;
        data.players.Clear();
        data.players.Add(playerData);
    }
    public void ApplySaveData()
    {
        SaveData data = SaveSystem.Instance.currentSaveData;
        if (data.players.Count > 0)
        {
            PlayerSaveData playerData = data.players[0];
            transform.position = playerData.position;
            PlayerData.hp = playerData.hp;
            PlayerData.moveSpeed = playerData.moveSpeed;
            PlayerData.attackSpeed = playerData.attackSpeed;
            PlayerData.damage = playerData.damage;
            PlayerData.money = playerData.money;
            PlayerData.bulletSpeed = playerData.bulletSpeed;
            PlayerData.knockback = playerData.knockback;
            PlayerData.isDead = playerData.isDead;
            PlayerData.isMelee = playerData.isMelee;
            PlayerData.piercingLevel = playerData.piercingLevel;
            PlayerData.dashLevel = playerData.dashLevel;
            PlayerData.hpRegenLevel = playerData.hpRegenLevel;
            PlayerData.blockLevel = playerData.blockLevel;
            PlayerData.freezeLevel = playerData.freezeLevel;
        }
    }
}
