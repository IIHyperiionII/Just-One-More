using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.PostProcessing;

public class PlayerController : MonoBehaviour
{
    private PlayerData PlayerData;
    private Rigidbody2D Rigidbody;
    private Vector2 input;
    private Vector3 mousePosition;
    public bool MouseKeyHoldDown = false;
    private float nextAttackTime = 0f;
    public GameObject bulletPrefab;
    public bool isAttacking = false; // for animation
    float attackAnimationDuration = 0.2f; // 
    private float attackStartTime = 0f; //
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
    public float slowMultiplier = 1f;
    public int numberOfSaves = 0;
    public int hp = 100;
    private bool isMoving = false;
    public AudioClip hurtSound;
    public AudioClip footstepClip;
    public AudioClip paperFootstepClip;
    public AudioClip waterFootstepClip;
    public AudioClip dashSound;
    public AudioClip deathSound;
    public AudioClip coinSound;
    public AudioClip shieldSound;
    public AudioClip hearthBeatSound;
    private float stepTimer = 0f;
    public GameObject WeaponControllerObject;
    private WeaponController weaponController;
    public int needToGamble = 0;
    
    void Start()
    {
        if (PlayerData == null){
            PlayerData = GameManager.Instance.runtimePlayerData; // Access the runtime player data from GameManager
        }
        if (WeaponControllerObject != null)
        {
            weaponController = WeaponControllerObject.GetComponent<WeaponController>();
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
        numberOfSaves = PlayerData.numberOfSaves;
        if (ModeController.Instance.currentSelection.selectedMode == GameMode.MoneyLife && PlayerData.money == 0)
        {
            PlayerData.money = 100;
        }
    }
    void Update()
    {
        if (GameModeManager.timeIsPaused) return;
        PlayerData = GameManager.Instance.runtimePlayerData; // Ensure PlayerData is always up-to-date
        GetMovementInput();
        if (PlayerData.dashLevel > 0)
        {
            GetDashInput();
        }
        GetAttackInput(); 
        Attack(); 
        if (Time.time - startTimer >= 2f) {
            startTimer = Time.time;
            if (PlayerData.needToGamble < 100){
                GameManager.Instance.runtimePlayerData.needToGamble ++;
            }
            NeedToGambleEffect();
        }
        if (ModeController.Instance.currentSelection.selectedMode == GameMode.MoneyLife)
        {
            GameManager.Instance.runtimePlayerData.hp = GameManager.Instance.runtimePlayerData.money;
        }
        if (isMoving)
        
        {
            HandleFootsteps();
        }
        else
        {
            stepTimer = 0.3f; // reset when stopping
        }
        needToGamble = PlayerData.needToGamble;
    }

    void HandleFootsteps()
    {

        stepTimer += Time.deltaTime * multiplier * slowMultiplier;

        if (stepTimer >= 0.4f && slowMultiplier == 1f)
        {
            float pitch = Random.Range(0.95f, 1.1f);
            SoundController.Instance.PlaySound(footstepClip, 0.85f, pitch);

            stepTimer = 0f;
        }
        else if (stepTimer >= 0.4f && slowMultiplier < 1f)
        {
            float pitch = Random.Range(0.95f, 1.1f);
            if (GameManager.Instance.map == 0) // paper
            {
                SoundController.Instance.PlaySound(paperFootstepClip, 0.35f, pitch);
            }
            else
            {
                SoundController.Instance.PlaySound(waterFootstepClip, 0.35f, pitch);
            }

            stepTimer = 0f;
        }
    }

    void FixedUpdate()
    {
        if (GameModeManager.timeIsPaused) return;
        if (isDashing) return; // Skip normal movement while dashing
        float speed = PlayerData.moveSpeed;
        if (PlayerData.moveSpeed > 5){
            speed = 5 + Mathf.Log(PlayerData.moveSpeed, 2);
        }
        Vector2 movement = input * Time.deltaTime * (speed * multiplier * slowMultiplier) * sign;
        if (movement.magnitude > 0)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
        Rigidbody.MovePosition(Rigidbody.position + movement);
        
    }

    void NeedToGambleEffect()
    {
        if (PlayerData.needToGamble >= 80)
        {
            if (vignetteEffect != null)
            {
                multiplier = 0.3f;
                pulseCount++;
                vignetteEffect.intensity.value = 0.60f;
                if (pulseCount % 3 == 0){
                    StartCoroutine(CameraController.PulseCamera(0.3f, 0.1f));
                    StartCoroutine(VignettePulse());
                    SoundController.Instance.PlaySound(hearthBeatSound, 4f, 1.0f);
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
        } else if (PlayerData.needToGamble >= 70)
        {
            if (vignetteEffect != null)
            {
                multiplier = 0.5f;
                pulseCount++;
                vignetteEffect.intensity.value = 0.55f + 0.005f*(PlayerData.needToGamble - 60);
                if (pulseCount % 4 == 0){
                    StartCoroutine(CameraController.PulseCamera(0.3f, 0.1f));
                    StartCoroutine(VignettePulse());
                    SoundController.Instance.PlaySound(hearthBeatSound, 4f, 1.0f);
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
                    SoundController.Instance.PlaySound(hearthBeatSound, 4f, 1.0f);
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
        input.x = ControlsManager.Instance.GetAxisHorizontal(); // Get horizontal input (A/D or Left/Right arrows)
        input.y = ControlsManager.Instance.GetAxisVertical(); // Get vertical input (W/S or Up/Down arrows)
        if (input.magnitude > 1) input.Normalize(); // Normalize to prevent faster diagonal movement
    }

    void GetDashInput()
    {
        if (ControlsManager.Instance.GetDashInputDown() && !isDashing && numberOfDashes > 0 && dashReset)
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

        Vector2 safePosition = target;
        Vector2 safePositionHand = lastHandPosition + dashDir * 6f;
        Vector2 finalLength = target - start;
        Vector2 finalLengthHand = lastHandPosition + dashDir * 6f - lastHandPosition;
        float distance = Vector2.Distance(Rigidbody.position, target);

        RaycastHit2D[] hits = new RaycastHit2D[1];
        int count = Rigidbody.Cast(dashDir, hits, distance);


        if (count > 0)
        {
            safePosition = Rigidbody.position + dashDir * (hits[0].distance - 0.01f);
            safePositionHand = lastHandPosition + dashDir * (hits[0].distance - 0.01f);
            finalLength = safePosition - start;
            finalLengthHand = safePositionHand - lastHandPosition;
            Debug.Log("Dash collision detected, adjusting target position. Length: " + finalLength.magnitude);

        } else {
            safePosition = target;
            safePositionHand = lastHandPosition + dashDir * 6f;
        }

        GameObject dashClone1 = CreateDashClone(1f, lastPosition, lastHandPosition);
        dashClone1.SetActive(false);
        GameObject dashClone2 = CreateDashClone(2f, lastPosition + dashDir * finalLength.magnitude/3, lastHandPosition + dashDir * finalLengthHand.magnitude/3);
        dashClone2.SetActive(false);
        GameObject dashClone3 = CreateDashClone(3f, lastPosition + dashDir * finalLength.magnitude * 2f/3, lastHandPosition + dashDir * finalLengthHand.magnitude * 2f/3);
        dashClone3.SetActive(false);
        StartCoroutine(CloneGeneration(dashClone1, dashClone2, dashClone3));
        SoundController.Instance.PlaySound(dashSound, 0.3f, 1.0f);
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
        if (ControlsManager.Instance.GetAttackInput())
        {
            MouseKeyHoldDown = true;
        }
        else
        {
            MouseKeyHoldDown = false;
        }
    }
    void Attack()
    {
        if (MouseKeyHoldDown && Time.time >= nextAttackTime)
        {
            float attackTime = PlayerData.attackSpeed;
            if (PlayerData.attackSpeed > 1){
                attackTime = Mathf.Log(PlayerData.attackSpeed, 2);
            }
            nextAttackTime = Time.time + 1f / (attackTime * multiplier);

            isAttacking = true; // for animation
            attackStartTime = Time.time; //

            switch (ModeController.Instance.currentSelection.selectedWeapon)
            {
                case WeaponType.Melee:
                    float rawKnockback = PlayerData.knockback;
                    if (PlayerData.knockback > 1000)
                    {
                        rawKnockback = 1000;
                    }
                    float knockback =Mathf.Lerp(0.1f, 1f,Mathf.Sqrt(Mathf.InverseLerp(1f, 1000f, rawKnockback)));
                    Debug.Log("Calculated knockback: " + knockback);
                    if (knockback < 0.1f) knockback = 0.1f;
                    if (knockback > 1f) knockback = 1f;
                    weaponController.AttackSword((int)(PlayerData.damage * multiplier), knockback);
                    break;
                case WeaponType.Pistol:
                    weaponController.AttackGun(PlayerData.bulletSpeed, (int)(PlayerData.damage * multiplier), PlayerData.piercingLevel, PlayerData.freezeLevel);
                    break;
                case WeaponType.Shotgun:
                    weaponController.AttackShotgun(PlayerData.bulletSpeed, (int)(PlayerData.damage * multiplier), PlayerData.piercingLevel, PlayerData.freezeLevel);
                break;
                default:
                    break;
            }
        }

        //for animation
        if (isAttacking && Time.time >= attackStartTime + attackAnimationDuration)
        {
            isAttacking = false;
        }
    }

    public void takeDamage(int damage)
    {
        if (PlayerData.isDead) return;
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
        if (ModeController.Instance.currentSelection.selectedMode == GameMode.MoneyLife)
        {
            GameManager.Instance.runtimePlayerData.money -= damage;
            if (PlayerData.money < 0) PlayerData.money = 0;
        } else {
            GameManager.Instance.runtimePlayerData.hp -= damage;
        }
        if (PlayerData.hp <= 0)
        {
            Die();
        } else {
            SoundController.Instance.PlaySound(hurtSound, 0.4f, 1.0f);  
        }
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
        SoundController.Instance.PlaySound(shieldSound, 0.3f, 1.0f);
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
        SoundController.Instance.PlaySound(deathSound, 0.4f, 1.0f);
        GameManager.Instance.runtimePlayerData.isDead = true;
        if (ModeController.Instance.currentSelection.selectedMode == GameMode.MoneyLife)
        {
            GameManager.Instance.runtimePlayerData.money = 0;
        } else {
            GameManager.Instance.runtimePlayerData.hp = 0;
        }
    }
    public void GetCoin(int amount)
    {
        Debug.Log("Collected coin worth: " + amount);
        GameManager.Instance.runtimePlayerData.money += amount;
        SoundController.Instance.PlaySound(coinSound, 0.6f, 1.0f);
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
        playerData.blockLevel = PlayerData.blockLevel;
        playerData.freezeLevel = PlayerData.freezeLevel;
        playerData.needToGamble = PlayerData.needToGamble;
        playerData.numberOfSaves = PlayerData.numberOfSaves;
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
            PlayerData.blockLevel = playerData.blockLevel;
            PlayerData.freezeLevel = playerData.freezeLevel;
            PlayerData.needToGamble = playerData.needToGamble;
            PlayerData.numberOfSaves = playerData.numberOfSaves;
        }
        GameManager.Instance.runtimePlayerData = PlayerData;
    }
}
