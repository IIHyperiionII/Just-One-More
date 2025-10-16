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
        // Singleton pattern (simple and effective for small projects)
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            runtimePlayerData = Instantiate(basePlayerData);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (runtimePlayerData.isDead)
        {
            EditorApplication.isPlaying = false;
            // Application.Quit(); // Uncomment this line to make it work in a build

        }
        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            SpawnEnemies();
            wave += 1;
        }
    }

    void SpawnEnemies()
    {
        foreach (GameObject enemyPrefab in EnemiesPrefabs)
        {
            int enemyCount = Random.Range(0, 2);
            for (int i = 0; i < enemyCount; i++)
            {
                GameObject enemy = Instantiate(enemyPrefab);
                enemy.transform.position = new Vector2(Random.Range(-8f, 8f), Random.Range(-4f, 4f));
            }
        }
    }
}
