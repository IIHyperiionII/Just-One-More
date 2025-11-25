using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using NUnit.Framework;
using UnityEngine.Rendering.PostProcessing;
using Unity.Collections.LowLevel.Unsafe;

public class PlayerController : MonoBehaviour
{
    private PlayerData PlayerData;
    private Rigidbody2D Rigidbody;
    private Vector2 input;
    private Vector3 mousePosition;
    public bool MouseKeyHoldDown = false;
    private float nextAttackTime = 0f;
    public GameObject bulletPrefab;
    public bool isAttacking = false; // for animation testing 
    private bool isDashing = false;
    private bool dashReset = true;
    private Vector2 dashDir;
    private int numberOfDashes = 2;
    private Vector2 lastPosition;
    private Vector2 lastHandPosition;
    private int shieldRequests = 0;
    public Vector2 MovementVector => input * PlayerData.moveSpeed;
    public bool isReadyToLoad = false;
    private bool isRed = false;
    private float startTimer = 0f;
    public PostProcessVolume postProcessVolume;
    private Vignette vignetteEffect;
    private float multiplier = 1f; // Default multiplier value
    private int pulseCount = 4;
    private int sign = 1;
    private bool isReversed = false;
    
    void Start()
    {
        if (PlayerData == null){
            PlayerData = GameManager.Instance.runtimePlayerData; // Access the runtime player data from GameManager
        }
        Rigidbody = GetComponent<Rigidbody2D>();
        Debug.Log("PlayerData initialized: " + (PlayerData != null));
        isReadyToLoad = true;
        if (postProcessVolume != null)
        {
            postProcessVolume.enabled = false;
            postProcessVolume.profile.TryGetSettings(out vignetteEffect);
        }
        startTimer = Time.time;
        PlayerData.damage = 1; // For testing purposes only
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
        if (Time.time - startTimer >= 1f) {
            startTimer = Time.time;
            PlayerData.needToGamble ++;
            NeedToGambleEffect();
        }
    }

    void FixedUpdate()
    {
        if (isDashing) return; // Skip normal movement while dashing
        Vector2 movement = input * Time.deltaTime * (PlayerData.moveSpeed * multiplier) * sign;
        Rigidbody.MovePosition(Rigidbody.position + movement);
        
    }

    void NeedToGambleEffect()
    {
        if (PlayerData.needToGamble >= 70)
        {
            if (vignetteEffect != null)
            {
                multiplier = 0.3f;
                pulseCount++;
                vignetteEffect.intensity.value = 0.60f;
                if (pulseCount % 3 == 0){
                    StartCoroutine(CameraController.PulseCamera(0.3f, 0.1f));
                    StartCoroutine(VignettePulse());
                    pulseCount = 0;
                }
                postProcessVolume.enabled = true;
            }
            if (!isReversed && Random.Range(0, 100) < 30){
                    StartCoroutine(ReverseInputs());
                }
            if (!isDashing && numberOfDashes > 0 && dashReset && Random.Range(0, 100) < 20)
            {
                StartCoroutine(Dash(true));
                StartCoroutine(ResetDash());
            }
        } else if (PlayerData.needToGamble >= 60)
        {
            if (vignetteEffect != null)
            {
                multiplier = 0.5f;
                pulseCount++;
                vignetteEffect.intensity.value = 0.55f + 0.005f*(PlayerData.needToGamble - 60);
                if (pulseCount % 4 == 0){
                    StartCoroutine(CameraController.PulseCamera(0.3f, 0.1f));
                    StartCoroutine(VignettePulse());
                    pulseCount = 0;
                }
                postProcessVolume.enabled = true;
            }
            if (!isReversed && Random.Range(0, 100) < 15){
                    StartCoroutine(ReverseInputs());
                }
        } else if (PlayerData.needToGamble >= 50)
        {
            if (vignetteEffect != null)
            {
                pulseCount++;
                vignetteEffect.intensity.value = 0.45f + 0.005f*(PlayerData.needToGamble - 50);
                if (pulseCount % 5 == 0){
                    StartCoroutine(CameraController.PulseCamera(0.3f, 0.1f));
                    StartCoroutine(VignettePulse());
                    pulseCount = 0;
                }
                postProcessVolume.enabled = true;
            }
        } else
        {
            if (vignetteEffect != null)
            {
                vignetteEffect.intensity.value = 0f;
                postProcessVolume.enabled = false;
            }

        }
    }

    IEnumerator ReverseInputs()
    {
        isReversed = true;
        sign = -1;
        yield return new WaitForSeconds(3f);
        sign = 1;
        isReversed = false;
    }

    public IEnumerator VignettePulse()
    {
        float init = vignetteEffect.intensity.value;
        float target = Mathf.Lerp(init, init + 0.2f, 1f);

        yield return new WaitForSeconds(0.1f);
        vignetteEffect.intensity.value = target;
        yield return new WaitForSeconds(0.05f);

        // fade back down
        float t = 0f;
        float start = target;
        while (t < 0.35f)
        {
            t += Time.deltaTime;
            vignetteEffect.intensity.value = Mathf.Lerp(start, init, t / 0.35f);
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        vignetteEffect.intensity.value = target;
        yield return new WaitForSeconds(0.05f);

        // fade back down
        t = 0f;
        start = target;
        while (t < 0.35f)
        {
            t += Time.deltaTime;
            vignetteEffect.intensity.value = Mathf.Lerp(start, init, t / 0.35f);
            yield return null;
        }
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
            StartCoroutine(Dash(false));
            StartCoroutine(ResetDash());
        }
    }

    IEnumerator Dash(bool isRandom)
    {
        isDashing = true;
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
        if (isRandom)
        {
            dashDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        }
        else
        {
            dashDir = input;
        }
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
        cloneSprite.sortingOrder = 1;
        cloneHand.transform.parent = dashClone.transform;
        SpriteRenderer originalHandSprite = hand.GetComponent<SpriteRenderer>();
        SpriteRenderer cloneHandSprite = cloneHand.AddComponent<SpriteRenderer>();
        cloneHandSprite.sprite = originalHandSprite.sprite;
        cloneHandSprite.sortingOrder = 1;
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
            nextAttackTime = Time.time + 1f / (PlayerData.attackSpeed * multiplier);
            isAttacking = true; // for animation testing

            Quaternion rotation = UpdateAngle();
            GameObject bullet = Instantiate(bulletPrefab, transform.position, rotation); // Spawn bullet at player position with calculated rotation
            bullet.GetComponent<PlayerBulletControllerTest>().Initialize(PlayerData.bulletSpeed, (int)(PlayerData.damage * multiplier)); // Initialize bullet with player stats
        }

        //for animation testing
        if (isAttacking && Time.time >= nextAttackTime - (1f / PlayerData.attackSpeed) + 0.1f)
        {
            isAttacking = false;
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
        if (Time.timeScale > 0){
            CameraController.ShakeCamera();
        }
        if (!isRed)
        {
            StartCoroutine(HitColor());
        }
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
        isRed = true;
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
        isRed = false;
    }

    public void Die()
    {
        PlayerData.isDead = true;
        PlayerData.hp = 0;
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
        playerData.needToGamble = PlayerData.needToGamble;
        data.players.Clear();
        data.players.Add(playerData);
    }
    public void ApplySaveData()
    {
        PlayerData = GameManager.Instance.runtimePlayerData;
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
            PlayerData.needToGamble = playerData.needToGamble;
        }
    }
}
