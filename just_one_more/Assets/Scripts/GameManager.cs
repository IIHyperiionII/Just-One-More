using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public PlayerData basePlayerData;
    public PlayerData runtimePlayerData;

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
}
