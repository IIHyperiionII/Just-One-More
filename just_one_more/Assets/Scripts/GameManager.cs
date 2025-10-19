using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public PlayerData basePlayerData;
    public PlayerData runtimePlayerData;
    private GameObject background;
    private Bounds backgroundBounds;
    private GameObject cameraObject;
    private Bounds cameraBounds;
    private int wave = 0;
    private int map = 0;
    public GameObject[] OfficeEnemyPrefabs;
    public GameObject[] ToiletsEnemyPrefabs;
    public GameObject[] BossOfficeEnemyPrefabs;
    private GameObject[][] EnemiesPrefabs;
    private ArrayList enemiesToSpawn = new ArrayList();
    private Vector2 spawnPosition;
    private bool WavesIsSpawning = false;
    void Awake()
    {
        if (!Application.isPlaying) return; // Skip initialization in edit mode
        // If there is no instance of GameManager, set it to this and make it persist between scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            runtimePlayerData = Instantiate(basePlayerData); // Create a runtime copy of the player data, so we don't modify the base data
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        if (!Application.isPlaying) return; // Skip initialization in edit mode
        GetBackgroundSize();
        GetCamera();
        EnemiesPrefabs = new GameObject[3][];
        EnemiesPrefabs[0] = OfficeEnemyPrefabs;
        EnemiesPrefabs[1] = ToiletsEnemyPrefabs;
        EnemiesPrefabs[2] = BossOfficeEnemyPrefabs;
    }
    void GetBackgroundSize()
    {
        if (GameObject.FindGameObjectWithTag("Background") == null)
        {
            Debug.LogError("Background GameObject is not assigned in GameManager!");
        }
        else
        {
            background = GameObject.FindGameObjectWithTag("Background"); // Find the background object in the scene
            SpriteRenderer backgroundRenderer = background.GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component
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
                backgroundBounds = new Bounds(background.transform.position, new Vector3(backgroundWidth, backgroundHeight, 0)); // Set the background bounds
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
        if (cameraObject == null)
        {
            GetCamera();
        }
        else
        {
            GetCameraBounds();
        }
        // Check if there are no enemies left
        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0 && !WavesIsSpawning)
        {
            StartCoroutine(SpawnWave()); // Wait for sync
            wave++;
        }
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
        foreach (GameObject enemyPrefab in EnemiesPrefabs[map])
        {
            int enemyCount = UnityRandom.Range(0, 3); // Random number between 0 and 2
            for (int i = 0; i < enemyCount; i++)
            {
                enemiesToSpawn.Add(enemyPrefab);
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
            yield return new WaitForSeconds(0.5f); // Wait before spawning the next enemy
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
    void GetSpawnposition(GameObject enemyPrefab, int recursionDepth = 0)
    {
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
