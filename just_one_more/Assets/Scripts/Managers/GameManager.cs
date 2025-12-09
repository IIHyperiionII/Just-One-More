using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityRandom = UnityEngine.Random;
using System;
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
    private bool waveIsSpawning = false;
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
        if (GameModeManager.timeIsPaused) return;
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
        else
        {
            //isTeleporting = false;
        }
        // Check if there are no enemies left
        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0 && !waveIsSpawning && wave < 10 && !isTeleporting)
        {
            wave++;
            StartCoroutine(SpawnWave()); // Wait for sync
            if (wave > 1){
                SoundController.Instance.PlaySound(WaveCompleteSound, 0.3f, 1.0f);
            }
        } else if ( wave > 10 && !mapCompleted ) {
            mapCompleted = true;
        }
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
    IEnumerator Teleport()
    {
        isTeleporting = true;
        doorsEntered = false;
        // Time.timeScale = 0f; // Pause the game
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
        // Time.timeScale = 1f; // Resume the game
        yield return new WaitForSecondsRealtime(5f); // Wait a moment after teleporting before starting the next wave
        backgroundSet = false;
    }

    IEnumerator SpawnWave()
    {
        waveIsSpawning = true;
        yield return new WaitForSeconds(1f); // Wait a moment before starting the next wave for sync of coroutines
        StartCoroutine(SpawnEnemies());
        waveIsSpawning = false;
    }
    IEnumerator SpawnEnemies()
    {
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
            GetSpawnposition(enemyPrefab); // Get a valid spawn position
            string enemyType = GetEnemyType(enemyPrefab);
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity); // Spawn the enemy at the calculated position
            enemy.GetComponent<IEnemy>().SetEnemyType(enemyType);
            if (enemiesParent != null)
                enemy.transform.SetParent(enemiesParent); // Set the parent of the spawned enemy for better hierarchy organization
            yield return new WaitForSeconds(0.5f); // Wait before spawning the next enemy
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
        UpdateBounds();
        if (spawnBounds.Any(bounds => bounds.Contains(spawnPosition)) && !cameraBounds.Contains(spawnPosition))
        {
            return;
        }
        else
        {
            GetSpawnposition(enemyPrefab, recursionDepth + 1); // Recursively find a new position if out of bounds
        }
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
        if (enemyPrefab == officePrefabs[0] || enemyPrefab == toiletPrefabs[0] || enemyPrefab == bossOfficePrefabs[0])
        {
            enemyPrefix = "1";
        }
        else if (enemyPrefab == officePrefabs[1] || enemyPrefab == toiletPrefabs[1] || enemyPrefab == bossOfficePrefabs[1])
        {
            enemyPrefix = "2";
        }
        else if (enemyPrefab == officePrefabs[2] || enemyPrefab == toiletPrefabs[2] || enemyPrefab == bossOfficePrefabs[2])
        {
            enemyPrefix = "3";
        }
        return mapType + enemyPrefix;
    }
    void GetCameraBounds()
    {
        float cameraHeight = cameraObject.GetComponent<Camera>().orthographicSize * 2f; // Calculate camera height based on orthographic size
        float cameraWidth = cameraHeight * cameraObject.GetComponent<Camera>().aspect; // Calculate camera width based on aspect ratio
        cameraHeight += 1f; // Add some padding
        cameraWidth += 1f;  // Add some padding
        cameraBounds = new Bounds(cameraObject.transform.position, new Vector3(cameraWidth, cameraHeight, 0)); // Set the camera bounds
    }

    public void GetSaveData()
    {
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
        if (GameObject.FindGameObjectWithTag("BulletParent").transform.childCount == 0) return;
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
            data.projectiles.Add(projectileData);
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
            switch (projectileData.projectileType)
            {
                case "BaseEnemyBullet":
                    projectilePrefab = bulletPrefabs[0];
                    GameObject enemyBullet = Instantiate(projectilePrefab, projectileData.position, projectileData.initialRotation);
                    enemyBullet.GetComponent<EnemyBulletBaseController>().Initialize(projectileData.speed, projectileData.damage, projectileData.initialRotation);
                    break;
                case "WaveEnemyBullet":
                    projectilePrefab = bulletPrefabs[1];
                    GameObject enemyBulletWave = Instantiate(projectilePrefab, projectileData.position, projectileData.initialRotation);
                    enemyBulletWave.GetComponent<EnemyBulletWaveController>().Initialize(projectileData.speed, projectileData.damage, projectileData.sign, projectileData.initialRotation);
                    break;
                case "ScalingEnemyBullet":
                    projectilePrefab = bulletPrefabs[2];
                    GameObject enemyBulletScaling = Instantiate(projectilePrefab, projectileData.position, projectileData.initialRotation);
                    enemyBulletScaling.GetComponent<EnemyBulletScalingController>().Initialize(projectileData.speed, projectileData.damage, projectileData.initialRotation);
                    break;
                default:
                    Debug.LogWarning("Unknown projectile type: " + projectileData.projectileType);
                    continue;
            }
        }
        Debug.Log("GameManager applied save data ");
    }

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
