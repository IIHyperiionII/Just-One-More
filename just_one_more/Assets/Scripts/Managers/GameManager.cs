using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityRandom = UnityEngine.Random;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public PlayerData basePlayerData;
    public PlayerData runtimePlayerData;
    public GameObject[] background = new GameObject[3];
    public GameObject[] backgroundOpen = new GameObject[3];
    public bool isOpen = false;
    private  Bounds[] spawnBounds;
    private GameObject cameraObject;
    private Bounds cameraBounds;
    public int wave = 1;
    public int map = 0;
    public bool mapCompleted = false;
    private GameObject[][] EnemiesPrefabs;
    public GameObject[] officePrefabs;
    public GameObject[] toiletPrefabs;
    public GameObject[] bossOfficePrefabs;
    public GameObject[] bulletPrefabs;
    private List<GameObject> enemiesToSpawn = new List<GameObject>();
    private int[] KindMinCount = new int[] { 1, 1, 1 };
    public static Transform enemiesParent;
    private Vector2 spawnPosition;
    public bool waveIsSpawning = false;
    //private bool isTeleporting = false;
    public bool doorsEntered = false;
    private SaveData saveData;
    public bool isGameReadyToLoad = false;
    private bool backgroundSet = false;
    public GameObject doorClosed;
    public GameObject doorClosedLight;
    public GameObject casinoButton;
    private ModeAndWeaponSelection currentSelection;
    public bool gameWon = false;
    public bool isTeleporting = false;
    public float time = 0f;   
    public AudioClip  ElevatorOpenSound;
    public AudioClip WaveCompleteSound;
    public GameObject coin;
    public GameObject playerBullet;
    public Sprite[] bulletSprites;
    void Awake()
    {
        if (!Application.isPlaying) return; // Skip initialization in edit mode
        // If there is no instance of GameManager, set it to this and make it persist between scenes
        if (Instance == null)
        {
            Instance = this;
            runtimePlayerData = Instantiate(basePlayerData); // Create a runtime copy of the player data, so we don't modify the base data
        }
        else
        {
            Destroy(gameObject);
        }
        EnemiesPrefabs = new GameObject[][]
        {
            officePrefabs,
            toiletPrefabs,
            bossOfficePrefabs
        };
        if (GameObject.FindGameObjectWithTag("EnemiesParent") != null)
        {
            enemiesParent = GameObject.FindGameObjectWithTag("EnemiesParent").transform; // Create a new GameObject to hold enemies   
        }
        if (saveData == null)
        {
            saveData = new SaveData();
        }
        isGameReadyToLoad = true;
        Debug.Log("GameManager is ready to load: " + isGameReadyToLoad);
        if (ModeController.Instance != null){
            currentSelection = ModeController.Instance.currentSelection;
            Debug.Log("Current weapon selection: " + currentSelection.selectedWeapon);
        }
        if (currentSelection != null && currentSelection.selectedWeapon == WeaponType.Melee){
            Debug.Log("Setting runtime player data to melee based on current selection");
            runtimePlayerData.isMelee = true;
        }
    }

    void Start()
    {
        if (!Application.isPlaying) return; // Skip initialization in edit mode
        GetCamera();
        if (SaveSystem.Instance.toLoad)
        {
            SaveSystem.Instance.LoadGame();
            Debug.Log("GameManager loaded save data");
            SaveSystem.Instance.toLoad = false;
        } 
        // Set the correct background based on the current map and whether it's open or closed
        if (backgroundOpen[map] != null || background[map] != null)
        {
            for (int i = 0; i < background.Length; i++)
            {
                if (i == map)
                {
                    if (isOpen){
                        backgroundOpen[i].SetActive(true);
                        background[i].SetActive(false);
                    } else {
                        background[i].SetActive(true);
                        backgroundOpen[i].SetActive(false);
                    }
                }
                else if (background[i] != null)
                {
                    background[i].SetActive(false);
                    backgroundOpen[i].SetActive(false);
                }
            }
        }
        if (currentSelection == null)
        {
            currentSelection = ModeController.Instance.currentSelection;
            if (currentSelection.selectedWeapon == WeaponType.Melee){
                runtimePlayerData.isMelee = true;
            }
        }
        if (currentSelection != null && currentSelection.selectedWeapon == WeaponType.Melee){
            runtimePlayerData.isMelee = true;
        }
    }
    void GetCamera()
    {
        if (GameObject.FindGameObjectWithTag("MainCamera") == null)
        {
            Debug.LogError("Main Camera GameObject is not assigned in GameManager!");
        }
        else
        {
            cameraObject = GameObject.FindGameObjectWithTag("MainCamera"); // Find the main camera object in the scene
        }
    }
    void Update()
    {
        // Pause the update if the game is paused
        if (GameModeManager.timeIsPaused) return;
        // Check if the elevator should open
        if (mapCompleted && !backgroundSet && enemiesParent.childCount == 0)
        {
            Debug.Log("Switching background from map " + map + " to map " + (map + 1));
            isOpen = true;
            background[map].SetActive(false);
            backgroundOpen[map].SetActive(true);
            backgroundSet = true;
            SoundController.Instance.PlaySound(ElevatorOpenSound, 0.3f, 1.0f);
        }
        if (runtimePlayerData == null)
        {
            Debug.LogError("Runtime Player Data is missing!");
            return;
        }
        if (doorsEntered == true && mapCompleted == true)
        {
            Debug.Log("Teleporting...");
            StartCoroutine(Teleport());
        }
        // Check if there are no enemies left
        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0 && !waveIsSpawning && wave < 10 && !isTeleporting)
        {
            wave++;
            StartCoroutine(SpawnWave()); // Wait for sync
            if (wave > 1){
                SoundController.Instance.PlaySound(WaveCompleteSound, 0.1f, 1.0f);
            }
        } else if ( wave > 10 && !mapCompleted ) {
            mapCompleted = true;
        }
        // Update the door and casino button states based on player's money
        if (runtimePlayerData.money >= 100)
        {
            doorClosed.SetActive(false);
            doorClosedLight.SetActive(true);
            casinoButton.SetActive(true);
        }
        else
        {
            doorClosed.SetActive(true);
            doorClosedLight.SetActive(false);
            casinoButton.SetActive(false);
        }
        // Check for game win condition
        if ((wave == 10 && mapCompleted && map == 2) || runtimePlayerData.isDead == true)
        {
            if (wave == 10 && mapCompleted && map == 2){
                if (!currentSelection.basicDeifficultyCompleted && currentSelection.selectedMode == GameMode.none){
                    currentSelection.basicDeifficultyCompleted = true;
                }
                gameWon = true;
            }
        } else {
            time += Time.deltaTime;
        }
    }

    // Teleport the player to the next map
    IEnumerator Teleport()
    {
        isTeleporting = true;
        doorsEntered = false;
        GameModeManager.timeIsPaused = true;
        CameraController.isTeleporting = true;
        yield return StartCoroutine(CameraController.TeleportMoveUp()); // Wait a moment before teleporting for sync of coroutines
        CameraController.isTeleporting = false;
        wave = 1;
        backgroundOpen[map].SetActive(false);
        map ++;
        backgroundOpen[map].SetActive(true);
        mapCompleted = false;
        foreach (Transform child in GameObject.FindGameObjectWithTag("BulletParent").transform)
        {
            Destroy(child.gameObject); // Clear all remaining bullets
        }
        yield return StartCoroutine(CameraController.TeleportMoveDown());
        backgroundSet = false;
        GameModeManager.timeIsPaused = false;
    }

    IEnumerator SpawnWave()
    {
        waveIsSpawning = true;
        yield return new WaitForSeconds(1f); // Wait a moment before starting the next wave for sync of coroutines
        SpawnEnemies();
        waveIsSpawning = false;
    }
    void SpawnEnemies()
    {
        Debug.Log("Spawning wave " + wave + " on map " + map);
        enemiesToSpawn.Clear(); // Clear the list before spawning enemies
    
        // Spawn a random number (0, 1, or 2) of each enemy type at random positions
        for (int i = 0; i < EnemiesPrefabs[map].Length; i++)
        {
            if (wave < 4 && i == 1) continue; // Skip spawning the second enemy type in the first three waves
            if (wave < 7 && i == 2) continue; // Skip spawning the third enemy type in the first six waves
            if (wave % 3 == 0)
            {
                KindMinCount[wave % 3] = 10;
            }
            int enemyCount = UnityRandom.Range(KindMinCount[i], KindMinCount[i] + 2); // Random number between 1 and 5
            if (wave >= 10)
            {
                enemyCount = 10;
            }
            for (int j = 0; j < enemyCount; j++)
            {
                enemiesToSpawn.Add(EnemiesPrefabs[map][i]);
            }
            if (KindMinCount[i] > 6)
            {
                KindMinCount[i] = 6;
            } else {
                KindMinCount[i]+= 2; // Increase the minimum count for the next wave
            }
        }

        // Shuffle the list to randomize spawn order
        for (int i = 0; i < enemiesToSpawn.Count; i++)
        {
            int randomIndex = UnityRandom.Range(0, enemiesToSpawn.Count);
            (enemiesToSpawn[i], enemiesToSpawn[randomIndex]) = (enemiesToSpawn[randomIndex], enemiesToSpawn[i]); // Swap elements to shuffle the list
        }
        foreach (GameObject enemyPrefab in enemiesToSpawn)
        {
            if (enemyPrefab == null) continue;
            Debug.Log("Spawning enemy of type: " + enemyPrefab.name);
            GetSpawnposition(enemyPrefab); // Get a valid spawn position
            string enemyType = GetEnemyType(enemyPrefab);
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity); // Spawn the enemy at the calculated position
            enemy.GetComponent<IEnemy>().SetEnemyType(enemyType);
            if (enemiesParent != null)
                enemy.transform.SetParent(enemiesParent); // Set the parent of the spawned enemy for better hierarchy organization
        }
        if (wave == 10)
        {
            mapCompleted = true;
        }
    }
    void GetSpawnposition(GameObject enemyPrefab, int recursionDepth = 0)
    {
        if (cameraObject == null)
        {
            GetCamera();
        }
        else
        {
            GetCameraBounds();
        }
        if (recursionDepth > 10)
        {
            Debug.LogWarning("Failed to find a valid spawn position after multiple attempts.");
            return;
        }
        int side = UnityRandom.Range(0, 4); // 0: top, 1: right, 2: bottom, 3: left
        spawnPosition = Vector2.zero;
        // Determine spawn position based on the selected side and ensure it's outside the camera bounds but within the background bounds
        float x, y;
        switch (side)
        {
            case 0: // Top
                x = UnityRandom.Range(-38f, 38f);
                if ((-38f < x && x < -15f) || (15f < x && x < 38f))
                {
                    y   = UnityRandom.Range(cameraBounds.max.y, 16f);
                }
                else
                {
                    y = UnityRandom.Range(cameraBounds.max.y, 13.5f);
                }
                spawnPosition = new Vector2(x, y);
                break;
            case 1: // Right
                x = UnityRandom.Range(cameraBounds.max.x, 38f);
                if ((-38f < x && x < -15f) || (15f < x && x < 38f))
                {
                    y = UnityRandom.Range(-20.5f, 16f);
                }
                else
                {
                    y = UnityRandom.Range(-20.5f, 13.5f);
                }
                spawnPosition = new Vector2(x, y);
                break;
            case 2: // Bottom
                x = UnityRandom.Range(-38f, 38f);
                y = UnityRandom.Range(-20.5f, cameraBounds.min.y);
                spawnPosition = new Vector2(x, y);
                break;
            case 3: // Left
                x = UnityRandom.Range(-38f, cameraBounds.min.x);
                if ((-38f < x && x < -15f) || (15f < x && x < 38f))
                {
                    y = UnityRandom.Range(-20.5f, 16f);
                }
                else
                {
                    y = UnityRandom.Range(-20.5f, 13.5f);
                }
                spawnPosition = new Vector2(x, y);
                break;
        }
        // Validate the spawn position
        UpdateBounds();
        if (spawnBounds.Any(bounds => bounds.Contains(spawnPosition)) && !cameraBounds.Contains(spawnPosition) && isFreeOfObstacles(spawnPosition, enemyPrefab))
        {
            return;
        }
        else
        {
            GetSpawnposition(enemyPrefab, recursionDepth + 1); // Recursively find a new position if out of bounds
        }
    }

    // Check if the spawn position is free of obstacles
    bool isFreeOfObstacles(Vector2 position, GameObject enemyPrefab)
    {
        SpriteRenderer enemySpriteRenderer = enemyPrefab.GetComponent<SpriteRenderer>();
        
        if (enemySpriteRenderer == null)
        {
            Debug.LogWarning("Enemy prefab does not have a SpriteRenderer component.");
            return true; // Assume it's free of obstacles if no collider is present
        }
        Vector2 size = enemySpriteRenderer.bounds.size;
        Collider2D hit = Physics2D.OverlapBox(position, size + new Vector2(0.1f, 0.1f), 0f, LayerMask.GetMask("Default")); // Slightly increase size to avoid edge collisions
        return hit == null;
    }

    void UpdateBounds()
    {
        spawnBounds = new Bounds[4];

        // top
        Vector3 topMin = new Vector3(-38f, cameraBounds.max.y, 0);
        Vector3 topMax = new Vector3(38f, 16f, 0);
        spawnBounds[0] = CreateBounds(topMin, topMax);

        // right
        Vector3 rightMin = new Vector3(cameraBounds.max.x, -20.5f, 0);
        Vector3 rightMax = new Vector3(38f, 16f, 0);
        spawnBounds[1] = CreateBounds(rightMin, rightMax);

        // bottom
        Vector3 bottomMin = new Vector3(-38f, -20.5f, 0);
        Vector3 bottomMax = new Vector3(38f, cameraBounds.min.y, 0);
        spawnBounds[2] = CreateBounds(bottomMin, bottomMax);

        // left
        Vector3 leftMin = new Vector3(-38f, -20.5f, 0);
        Vector3 leftMax = new Vector3(cameraBounds.min.x, 16f, 0);
        spawnBounds[3] = CreateBounds(leftMin, leftMax);
    }

    Bounds CreateBounds(Vector3 min, Vector3 max)
    {
        Vector3 center = (min + max) * 0.5f;
        Vector3 size = max - min;
        return new Bounds(center, size);
    }

    string GetEnemyType(GameObject enemyPrefab)
    {
        string mapType = "";
        if (map == 0)
        {
            mapType = "office";
        }
        else if (map == 1)
        {
            mapType = "toilet";
        }
        else
        {
            mapType = "bossOffice";
        }
        string enemyPrefix = "";
        string prefabName = enemyPrefab.name;
        
        // Compare by name instead of reference (more reliable in builds)
        if ((officePrefabs[0] != null && prefabName == officePrefabs[0].name) || 
            (toiletPrefabs[0] != null && prefabName == toiletPrefabs[0].name) || 
            (bossOfficePrefabs[0] != null && prefabName == bossOfficePrefabs[0].name))
        {
            enemyPrefix = "1";
        }
        else if ((officePrefabs[1] != null && prefabName == officePrefabs[1].name) || 
                 (toiletPrefabs[1] != null && prefabName == toiletPrefabs[1].name) || 
                 (bossOfficePrefabs[1] != null && prefabName == bossOfficePrefabs[1].name))
        {
            enemyPrefix = "2";
        }
        else if ((officePrefabs[2] != null && prefabName == officePrefabs[2].name) || 
                 (toiletPrefabs[2] != null && prefabName == toiletPrefabs[2].name) || 
                 (bossOfficePrefabs[2] != null && prefabName == bossOfficePrefabs[2].name))
        {
            enemyPrefix = "3";
        }
        string result = mapType + enemyPrefix;
        return result;
    }
    void GetCameraBounds()
    {
        float cameraHeight = cameraObject.GetComponent<Camera>().orthographicSize * 2f; // Calculate camera height based on orthographic size
        float cameraWidth = cameraHeight * cameraObject.GetComponent<Camera>().aspect; // Calculate camera width based on aspect ratio
        cameraHeight += 1f; // Add some padding
        cameraWidth += 1f;  // Add some padding
        cameraBounds = new Bounds(cameraObject.transform.position, new Vector3(cameraWidth, cameraHeight, 0)); // Set the camera bounds
    }

    // Save and Load functionality
    public void GetSaveData()
    {
        Debug.Log("Getting GameManager Save Data...");
        SaveData data = SaveSystem.Instance.currentSaveData;
        data.map = map;
        data.isOpen = isOpen;
        data.wave = wave;
        data.mapCompleted = mapCompleted;
        data.time = time;
        data.enemies.Clear();
        foreach (Transform child in enemiesParent.transform)
        {
            Debug.Log("Processing enemy for save data...");
            GameObject enemy = child.gameObject;
            IEnemy enemyController = enemy.GetComponent<IEnemy>();
            EnemyData dataInstance = enemyController.GetEnemyData();
            EnemySaveData enemyData = new EnemySaveData();
            enemyData.position = enemy.transform.position;
            enemyData.enemyType = enemyController.GetEnemyType();
            enemyData.hp = dataInstance.hp;
            enemyData.moveSpeed = dataInstance.moveSpeed;
            enemyData.damage = dataInstance.damage;
            enemyData.attackSpeed = dataInstance.attackSpeed;
            enemyData.bulletSpeed = dataInstance.bulletSpeed;
            data.enemies.Add(enemyData);
        }
        data.projectiles.Clear();
        foreach (Transform child in GameObject.FindGameObjectWithTag("BulletParent").transform)
        {
            GameObject bullet = child.gameObject;
            IBullet bulletController = bullet.GetComponent<IBullet>();
            ProjectileSaveData projectileData = new ProjectileSaveData();
            projectileData.position = bullet.transform.position;
            projectileData.projectileType = bulletController.GetBulletType();
            projectileData.initialRotation = bulletController.GetInitialRotation();
            projectileData.speed = bulletController.GetSpeed();
            projectileData.damage = bulletController.GetDamage();
            projectileData.sign = bulletController.GetSign();
            projectileData.spriteName = bulletController.GetSpriteName();
            data.projectiles.Add(projectileData);
        }
        data.coins.Clear();
        foreach (Transform child in GameObject.FindGameObjectWithTag("MoneyParent").transform)
        {
            GameObject coin = child.gameObject;
            CoinsSaveData coinData = new CoinsSaveData();
            coinData.position = coin.transform.position;
            coinData.value = coin.GetComponent<ICoin>().GetValue();
            data.coins.Add(coinData);
        }
        data.playerProjectiles.Clear();
        foreach (Transform child in GameObject.FindGameObjectWithTag("BulletsPlayerParent").transform)
        {
            GameObject bullet = child.gameObject;
            IBulletPlayer bulletController = bullet.GetComponent<IBulletPlayer>();
            ProjectilePlayerSaveData projectilePlayerData = new ProjectilePlayerSaveData();
            projectilePlayerData.position = bullet.transform.position;
            projectilePlayerData.initialRotation = bulletController.GetInitialRotation();
            projectilePlayerData.speed = bulletController.GetSpeed();
            projectilePlayerData.damage = bulletController.GetDamage();
            projectilePlayerData.freezeLevel = bulletController.GetFreezeLevel();
            projectilePlayerData.piercingLevel = bulletController.GetPiercingLevel();
            projectilePlayerData.spriteName = bulletController.GetSpriteName();
            data.playerProjectiles.Add(projectilePlayerData);
        }
    }
    public void ApplySaveData()
    {
        SaveData data = SaveSystem.Instance.currentSaveData;
        map = data.map;
        isOpen = data.isOpen;
        wave = data.wave;
        mapCompleted = data.mapCompleted;
        time = data.time;
        foreach (EnemySaveData enemyData in data.enemies)
        {
            Debug.Log("Restoring enemy of type: " + enemyData.enemyType);
            GameObject enemyPrefab;
            // Determine the correct prefab based on enemy type
            switch (enemyData.enemyType)
            {
                case "office1":
                    enemyPrefab = officePrefabs[0];
                    break;
                case "office2":
                    enemyPrefab = officePrefabs[1];
                    break;
                case "office3":
                    enemyPrefab = officePrefabs[2];
                    break;
                case "toilet1":
                    enemyPrefab = toiletPrefabs[0];
                    break;
                case "toilet2":
                    enemyPrefab = toiletPrefabs[1];
                    break;
                case "toilet3":
                    enemyPrefab = toiletPrefabs[2];
                    break;
                case "bossOffice1":
                    enemyPrefab = bossOfficePrefabs[0];
                    break;
                case "bossOffice2":
                    enemyPrefab = bossOfficePrefabs[1];
                    break;
                case "bossOffice3":
                    enemyPrefab = bossOfficePrefabs[2];
                    break;
                default:
                    Debug.LogWarning("Unknown enemy type: " + enemyData.enemyType);
                    continue;
            }
            GameObject enemy = Instantiate(enemyPrefab, enemyData.position, Quaternion.identity);
            IEnemy enemyController = enemy.GetComponent<IEnemy>();
            EnemyData dataInstance = enemyController.GetEnemyData();
            Debug.Log("Enemy data: " + (dataInstance != null ? "found" : "null"));
            enemyController.SetEnemyType(enemyData.enemyType);
            dataInstance.hp = enemyData.hp;
            dataInstance.moveSpeed = enemyData.moveSpeed;
            dataInstance.damage = enemyData.damage;
            dataInstance.attackSpeed = enemyData.attackSpeed;
            dataInstance.bulletSpeed = enemyData.bulletSpeed;
            if (enemiesParent != null)
                enemy.transform.SetParent(enemiesParent);
        }
        foreach (ProjectileSaveData projectileData in data.projectiles)
        {
            GameObject projectilePrefab;
            Sprite bulletSprite = null;
            for (int i = 0; i < bulletSprites.Length; i++){
                if (bulletSprites[i].name == projectileData.spriteName){
                    bulletSprite = bulletSprites[i];
                    break;
                }
            }
            // Determine the correct prefab based on projectile type
            switch (projectileData.projectileType)
            {
                case "BaseEnemyBullet":
                    projectilePrefab = bulletPrefabs[0];
                    GameObject enemyBullet = Instantiate(projectilePrefab, projectileData.position, projectileData.initialRotation);
                    enemyBullet.GetComponent<EnemyBulletBaseController>().Initialize(projectileData.speed, projectileData.damage, projectileData.initialRotation, bulletSprite);
                    break;
                case "WaveEnemyBullet":
                    projectilePrefab = bulletPrefabs[1];
                    GameObject enemyBulletWave = Instantiate(projectilePrefab, projectileData.position, projectileData.initialRotation);
                    enemyBulletWave.GetComponent<EnemyBulletWaveController>().Initialize(projectileData.speed, projectileData.damage, projectileData.sign, projectileData.initialRotation, bulletSprite);
                    break;
                case "ScalingEnemyBullet":
                    projectilePrefab = bulletPrefabs[2];
                    GameObject enemyBulletScaling = Instantiate(projectilePrefab, projectileData.position, projectileData.initialRotation);
                    enemyBulletScaling.GetComponent<EnemyBulletScalingController>().Initialize(projectileData.speed, projectileData.damage, projectileData.initialRotation, bulletSprite);
                    break;
                default:
                    Debug.LogWarning("Unknown projectile type: " + projectileData.projectileType);
                    continue;
            }
        }
        foreach (CoinsSaveData coinData in data.coins)
        {
            GameObject coinInstance = Instantiate(coin, coinData.position, Quaternion.identity);
            coinInstance.GetComponent<ICoin>().SetValue(coinData.value);
            coinInstance.transform.SetParent(GameObject.FindGameObjectWithTag("MoneyParent").transform);
        }
        foreach (ProjectilePlayerSaveData projectilePlayerData in data.playerProjectiles)
        {
            Sprite bulletSprite = null;
            for (int i = 0; i < bulletSprites.Length; i++){
                if (bulletSprites[i].name == projectilePlayerData.spriteName){
                    bulletSprite = bulletSprites[i];
                    break;
                }
            }
            GameObject playerBulletInstance = Instantiate(playerBullet, projectilePlayerData.position, projectilePlayerData.initialRotation);
            
            playerBulletInstance.GetComponent<PlayerBulletController>().Initialize(projectilePlayerData.speed, projectilePlayerData.damage, projectilePlayerData.piercingLevel, projectilePlayerData.freezeLevel, projectilePlayerData.initialRotation, bulletSprite);
            
            playerBulletInstance.transform.SetParent(GameObject.FindGameObjectWithTag("BulletsPlayerParent").transform);
        }
        if (runtimePlayerData != null && currentSelection != null)
        {
            runtimePlayerData.isMelee = currentSelection.selectedWeapon == WeaponType.Melee;
        }
        Debug.Log("GameManager applied save data ");
    }

    // Get a random sprite that is different from the initial sprite
    public Sprite GetRandomSprite(Sprite initSprite)
    {
        int tmpMap = UnityEngine.Random.Range(0, 3);  
        int tmpVariant = UnityEngine.Random.Range(0, 3);
        Debug.Log("Selected random sprite from map " + tmpMap + " variant " + tmpVariant);
        Sprite newSprite = EnemiesPrefabs[tmpMap][tmpVariant].GetComponent<SpriteRenderer>().sprite;
        if (newSprite == initSprite)
        {
            return GetRandomSprite(initSprite); // Recursively get a new sprite if it's the same as the initial one
        }
        return newSprite;
    }

    // Reset the GameManager to its initial state
    public void ResetGameManager()
    {
        wave = 0;
        map = 0;
        mapCompleted = false;
        isOpen = false;
        gameWon = false;
        time = 0f;
        enemiesToSpawn.Clear();
        foreach (Transform child in enemiesParent.transform)
        {
            Destroy(child.gameObject); // Clear all remaining enemies
        }
        foreach (Transform child in GameObject.FindGameObjectWithTag("BulletParent").transform)
        {
            Destroy(child.gameObject); // Clear all remaining bullets
        }
        backgroundSet = false;
        isTeleporting = false;
        KindMinCount = new int[] { 1, 1, 1 };
        waveIsSpawning = false;
        runtimePlayerData = Instantiate(basePlayerData);


    }
    
    /*
    void OnDrawGizmos()
    {
        if (background != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(backgroundBounds.center, backgroundBounds.size);
        }
        if (cameraObject != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(cameraBounds.center, cameraBounds.size);
        }
        Gizmos.DrawWireCube(spawnPosition, tmpEnemy != null ? tmpEnemy.GetComponent<Collider2D>().bounds.extents * 2 : Vector3.one);
    }
    */
    private void OnDrawGizmos()
    {
        if (spawnBounds == null) return;

        Gizmos.color = new Color(1f, 0f, 0f, 0.3f); // Semi-transparent red

        foreach (var b in spawnBounds)
        {
            Gizmos.DrawCube(b.center, b.size);
        }

        Gizmos.color = Color.red;
        foreach (var b in spawnBounds)
        {
            Gizmos.DrawWireCube(b.center, b.size);
        }
    }

}
