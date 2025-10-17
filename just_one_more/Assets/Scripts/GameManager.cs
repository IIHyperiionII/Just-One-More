using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public PlayerData basePlayerData;
    public PlayerData runtimePlayerData;
    private int wave = 0;
    public GameObject[] EnemiesPrefabs;
    void Awake()
    {
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
            EditorApplication.isPlaying = false; // Stop play mode in the editor
            // Application.Quit(); // Uncomment this line to make it work in a build
        }
        // Check if there are no enemies left
        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            SpawnEnemies();
            wave += 1;
        }
    }
    void SpawnEnemies()
    {
        // Spawn a random number (0, 1, or 2) of each enemy type at random positions
        foreach (GameObject enemyPrefab in EnemiesPrefabs)
        {
            int enemyCount = Random.Range(0, 2);
            for (int i = 0; i < enemyCount; i++)
            {
                GameObject enemy = Instantiate(enemyPrefab);
                //TODO : Adjust spawn position logic for game map size and player position
                enemy.transform.position = new Vector2(Random.Range(-8f, 8f), Random.Range(-4f, 4f)); // Random position within a defined range
            }
        }
    }
    
}
