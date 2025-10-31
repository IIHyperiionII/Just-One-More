using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityRandom = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public PlayerData basePlayerData;
    public PlayerData runtimePlayerData;
    private GameObject[] background = new GameObject[3];
    public Bounds backgroundBounds;
    private GameObject cameraObject;
    private Bounds cameraBounds;
    public int wave = 1;
    private int map = 0;
    private bool mapCompleted = false;
    private GameObject[][] EnemiesPrefabs;
    public GameObject[] officePrefabs;
    public GameObject[] toiletPrefabs;
    public GameObject[] bossOfficePrefabs;
    private List<GameObject> enemiesToSpawn = new List<GameObject>();
    private int[] KindMinCount = new int[] { 1, 1, 1 };
    public static Transform enemiesParent;
    private Vector2 spawnPosition;
    private bool WavesIsSpawning = false;
    //private bool isTeleporting = false;
    public bool doorsEntered = false;
    private Collider2D doorsCollider;
    private Collider2D playerCollider;
    void Awake()
    {
        if (!Application.isPlaying) return; // Skip initialization in edit mode
        // If there is no instance of GameManager, set it to this and make it persist between scenes
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
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
    }
    void Start()
    {
        if (!Application.isPlaying) return; // Skip initialization in edit mode
        GetBackgroundSize();
        GetCamera();
        if (GameObject.FindGameObjectWithTag("BoundsCheckDoors").GetComponent<Collider2D>() == null)
        {
            Debug.LogError("Doors GameObject does not exist!");
        }
        else
        {
            doorsCollider = GameObject.FindGameObjectWithTag("BoundsCheckDoors").GetComponent<Collider2D>();
        }
        if (GameObject.FindGameObjectWithTag("BoundsCheckPlayer").GetComponent<Collider2D>() == null)
        {
            Debug.LogError("Player bounds GameObject does not exist!");
        }
        else
        {
            playerCollider = GameObject.FindGameObjectWithTag("BoundsCheckPlayer").GetComponent<Collider2D>();
        }
    }
    void GetBackgroundSize()
    {
        if (GameObject.FindGameObjectWithTag("Background") == null)
        {
            Debug.LogError("Background GameObject is not assigned in GameManager!");
        }
        else
        {
            background[0] = GameObject.FindGameObjectWithTag("Background"); // Find the background object in the scene
            background[1] = GameObject.FindGameObjectWithTag("Background2"); // Find the second background object in the scene
            background[2] = GameObject.FindGameObjectWithTag("Background3"); // Find the third background object in the scene
            background[1].SetActive(false); // Disable the second background at start
            background[2].SetActive(false); // Disable the third background at start
            SpriteRenderer backgroundRenderer = background[map].GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component
            if (backgroundRenderer == null)
            {
                Debug.LogError("Background GameObject does not have a SpriteRenderer component!");
            }
            else
            {
                float backgroundWidth = backgroundRenderer.bounds.size.x;
                float backgroundHeight = backgroundRenderer.bounds.size.y;
                backgroundWidth -= 1f; // Add some padding
                backgroundHeight -= 1f; // Add some padding
                backgroundBounds = new Bounds(background[map].transform.position, new Vector3(backgroundWidth, backgroundHeight, 0)); // Set the background bounds
            }
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
        if (runtimePlayerData == null)
        {
            Debug.LogError("Runtime Player Data is missing!");
            return;
        }
        // Check if the player is dead
        if (runtimePlayerData.isDead)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in the editor
#else
                //Application.Quit(); // Uncomment this line to quit the application in a build
#endif
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
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
        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0 && !WavesIsSpawning && wave < 10)
        {
            wave++;
            StartCoroutine(SpawnWave()); // Wait for sync
        } else if ( wave >= 10 && !mapCompleted ) {
            mapCompleted = true;
        }
    }
    IEnumerator Teleport()
    {
        //isTeleporting = true;
        doorsEntered = false;
        Time.timeScale = 0f; // Pause the game
        CameraController.isTeleporting = true;
        yield return StartCoroutine(CameraController.TeleportMoveUp()); // Wait a moment before teleporting for sync of coroutines
        yield return new WaitForSecondsRealtime(0.5f); // Hold at the top position for a moment
        CameraController.isTeleporting = false;
        wave = 1;
        background[map].SetActive(false);
        map++;
        background[map].SetActive(true);
        foreach (GameObject bullet in GameObject.FindGameObjectsWithTag("BulletParent"))
        {
            Destroy(bullet); // Clear all remaining bullets
        }
        yield return StartCoroutine(CameraController.TeleportMoveDown());
        Time.timeScale = 1f; // Resume the game
        yield return new WaitForSecondsRealtime(5f); // Wait a moment after teleporting before starting the next wave
        mapCompleted = false;
        //isTeleporting = false;
    }
    IEnumerator SpawnWave()
    {
        WavesIsSpawning = true;
        yield return new WaitForSeconds(1f); // Wait a moment before starting the next wave for sync of coroutines
        StartCoroutine(SpawnEnemies());
        WavesIsSpawning = false;
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
            GetSpawnposition(enemyPrefab); // Get a valid spawn position
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity); // Spawn the enemy at the calculated position
            if (enemiesParent != null)
                enemy.transform.SetParent(enemiesParent); // Set the parent of the spawned enemy for better hierarchy organization
            yield return new WaitForSeconds(0.5f); // Wait before spawning the next enemy
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
        switch (side)
        {
            case 0: // Top
                spawnPosition = new Vector2(Mathf.Clamp(UnityRandom.Range(backgroundBounds.min.x, backgroundBounds.max.x), backgroundBounds.min.x, backgroundBounds.max.x), Mathf.Clamp(UnityRandom.Range(cameraBounds.max.y, backgroundBounds.max.y), cameraBounds.max.y, backgroundBounds.max.y));
                break;
            case 1: // Right
                spawnPosition = new Vector2(Mathf.Clamp(UnityRandom.Range(cameraBounds.max.x, backgroundBounds.max.x), backgroundBounds.min.x, backgroundBounds.max.x), UnityRandom.Range(backgroundBounds.min.y, backgroundBounds.max.y));
                break;
            case 2: // Bottom
                spawnPosition = new Vector2(Mathf.Clamp(UnityRandom.Range(backgroundBounds.min.x, backgroundBounds.max.x), backgroundBounds.min.x, backgroundBounds.max.x), Mathf.Clamp(UnityRandom.Range(backgroundBounds.min.y, cameraBounds.min.y), backgroundBounds.min.y, cameraBounds.min.y));
                break;
            case 3: // Left
                spawnPosition = new Vector2(Mathf.Clamp(UnityRandom.Range(backgroundBounds.min.x, cameraBounds.min.x), backgroundBounds.min.x, cameraBounds.min.x), Mathf.Clamp(UnityRandom.Range(backgroundBounds.min.y, backgroundBounds.max.y), backgroundBounds.min.y, backgroundBounds.max.y));
                break;
        }
        if (backgroundBounds.Contains(spawnPosition) == false || cameraBounds.Contains(spawnPosition) == true)
        {
            GetSpawnposition(enemyPrefab, recursionDepth + 1); // Recursively find a new position if out of bounds
        }
    }
    void GetCameraBounds()
    {
        float cameraHeight = cameraObject.GetComponent<Camera>().orthographicSize * 2f; // Calculate camera height based on orthographic size
        float cameraWidth = cameraHeight * cameraObject.GetComponent<Camera>().aspect; // Calculate camera width based on aspect ratio
        cameraHeight += 1f; // Add some padding
        cameraWidth += 1f;  // Add some padding
        cameraBounds = new Bounds(cameraObject.transform.position, new Vector3(cameraWidth, cameraHeight, 0)); // Set the camera bounds
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
}
